using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class EmotionDetector : MonoBehaviour
{
    public GeminiClient geminiClient;
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
    public Fase2Controller faseEmocoes;
    private WebCamTexture webcamTexture;
    public CameraCapture cameraCapture;
    private bool isProcessing = false;
    public Button btnDetectEmotion;

    [Serializable]
    public class GeminiResponse
    {
        public Candidate[] candidates;
    }

    [Serializable]
    public class Candidate
    {
        public Content content;
        public string finishReason;
        public float avgLogprobs;
    }

    [Serializable]
    public class Content
    {
        public Part[] parts;
        public string role;
    }

    [Serializable]
    public class Part
    {
        public string text;
    }

    public void Start()
    {
        webcamTexture = cameraCapture.GetCameraTexture();

        btnDetectEmotion.onClick.AddListener(() => StartCoroutine(PredictEmotion()));
    }

    public IEnumerator PredictEmotion()
    {
        if (isProcessing)
            yield break;

        isProcessing = true;
        btnDetectEmotion.interactable = false;

        // yield return new WaitForSeconds(10f);
        string apiKey = geminiClient.GetApiKey();

        Texture2D tempTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
        tempTexture.SetPixels(webcamTexture.GetPixels());
        tempTexture.Apply();

        // 1. Converte a textura para bytes JPEG e depois para base64
        byte[] imageBytes = tempTexture.EncodeToJPG();
        string base64Image = System.Convert.ToBase64String(imageBytes);

        // 2. Cria o prompt de texto
        string prompt = "Com base nesta imagem, descreva a emoção visível no rosto da pessoa. "
        + "Responda apenas com o nome da emoção. As emoções podem ser: raiva, alegria, tristeza, surpresa, medo ou nojo."
        + "Caso nenhuma pessoa seja detectada, responda apenas: 'Neutro'";

        // 3. Cria o corpo da requisição JSON com texto e imagem
        string requestBody = "{" +
            "\"contents\": [" +
                "{" +
                    "\"parts\": [" +
                        "{\"text\": \"" + prompt + "\"}," +
                        "{\"inline_data\": {" +
                            "\"mime_type\": \"image/jpeg\"," +
                            "\"data\": \"" + base64Image + "\"" +
                        "}}" +
                    "]" +
                "}" +
            "]" +
        "}";

        using (UnityWebRequest www = new UnityWebRequest(apiUrl + "?key=" + apiKey, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(requestBody);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Erro na requisição: " + www.error);
                faseEmocoes.VerificarEmocao(TipoEmocao.Neutro);
                // onComplete?.Invoke("Erro");
            }
            else
            {
                string responseText = www.downloadHandler.text;

                try
                {
                    GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(responseText);

                    if (response != null && response.candidates != null && response.candidates.Length > 0)
                    {
                        string emotion = response.candidates[0].content.parts[0].text;
                        Debug.Log("Emoção detectada: " + emotion);

                        faseEmocoes.VerificarEmocao(conversorEmocaoEnum(emotion));
                    }
                    else
                    {
                        Debug.LogError("Resposta da API não contém um resultado válido.");
                        faseEmocoes.VerificarEmocao(TipoEmocao.Neutro);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Erro ao processar a resposta JSON: " + e.Message);
                    faseEmocoes.VerificarEmocao(TipoEmocao.Neutro);
                }
            }

            isProcessing = false;
            btnDetectEmotion.interactable = true;
        }
    }
    
    TipoEmocao conversorEmocaoEnum(string emocao)
    {
        switch (emocao)
        {
            case "Alegria":
                return TipoEmocao.Alegria;
            case "Surpresa":
                return TipoEmocao.Surpresa;
            case "Tristeza":
                return TipoEmocao.Tristeza;
            case "Raiva":
                return TipoEmocao.Raiva;
            case "Nojo":
                return TipoEmocao.Nojo;
            case "Medo":
                return TipoEmocao.Medo;
            default:
                return TipoEmocao.Neutro;
        }
    }
}
/*
using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using Unity.Barracuda;
using System;
using System.Collections;
using System.Linq;
using System.IO;


public class EmotionDetector : MonoBehaviour
{
    public Fase3Controller faseEmocoes; // arraste no Inspector a referência
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;
    private WebCamTexture webcamTexture; // Não inicializa aqui
    public int imageSizeX = 64;
    public int imageSizeY = 64;
    string[] emotionLabels = new string[] {
        "Neutro", "Alegria", "Surpresa", "Tristeza", "Raiva", "Desgosto", "Medo", "Desprezo"
    };

    public TextMeshProUGUI labelEmocao;
    public TextMeshProUGUI labelNeutro;
    public TextMeshProUGUI labelAlegria;
    public TextMeshProUGUI labelTristeza;
    public TextMeshProUGUI labelSurpresa;
    public TextMeshProUGUI labelRaiva;
    public TextMeshProUGUI labelDesgosto;
    public TextMeshProUGUI labelMedo;
    public TextMeshProUGUI labelDesprezo;

    void Start()
    {
        // Inicializa o modelo
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.Auto, runtimeModel);

        // Usa a câmera compartilhada
        webcamTexture = CameraCapture.instance.GetCameraTexture();

        StartCoroutine(WaitForCamera());
    }

    IEnumerator WaitForCamera()
    {
        while (CameraCapture.instance == null || CameraCapture.instance.GetCameraTexture() == null)
        {
            yield return null; // Aguarda um frame
        }

        StartCoroutine(PredictEmotionRoutine());
    }

    IEnumerator PredictEmotionRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            PredictEmotion();
        }
    }

    void PredictEmotion()
    {
        if (webcamTexture.width > 100) // Garante que a câmera já iniciou
        {
            Texture2D tempTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
            tempTexture.SetPixels(webcamTexture.GetPixels());
            tempTexture.Apply();

            // Criar uma textura com o tamanho esperado pelo modelo
            Texture2D texture = ResizeTexture(tempTexture, imageSizeX, imageSizeY);

            // Converter a imagem para escala de cinza
            Color[] pixels = texture.GetPixels();
            float[] grayscalePixels = new float[pixels.Length];

            for (int i = 0; i < pixels.Length; i++)
            {
                grayscalePixels[i] = pixels[i].grayscale*255; // Calcula a média RGB para obter tons de cinza
            }

            // SavePixelsToCSV(grayscalePixels, "gray.csv", imageSizeX);

            // float[] grayscalePixels = ResizeToGrayArray(tempTexture, 64, 64);
            
            // SavePixelsToCSV(grayscalePixels, "gray.csv", imageSizeX);

            Tensor inputTensor = new Tensor(1, imageSizeX, imageSizeY, 1, grayscalePixels);

            // Fazer a inferência
            worker.Execute(inputTensor);
            Tensor outputTensor = worker.PeekOutput();

            // Obter os valores do tensor de saída
            float[] predictions = new float[8]; // Para 8 emoções
            for (int i = 0; i < 8; i++)
            {
                predictions[i] = outputTensor[0, 0, 0, i]; // Cada valor para cada emoção
            }

            // Teste para não detectar neutro.
            predictions[0] = -1;

            int maxIndex = Array.IndexOf(predictions, predictions.Max());

            // Exibir o resultado
            if(maxIndex == 0)
                labelEmocao.text = "Não detectado";
            else
                labelEmocao.text = emotionLabels[maxIndex];

            // ATUALIZA A FASE -> só se não for neutro
            if (maxIndex != 0 && faseEmocoes != null)
            {
                faseEmocoes.VerificarEmocao(conversorEmocaoEnum(maxIndex));
            }

            labelNeutro.text = predictions[0].ToString();
            labelAlegria.text = predictions[1].ToString();
            labelSurpresa.text = predictions[2].ToString();
            labelTristeza.text = predictions[3].ToString();
            labelRaiva.text = predictions[4].ToString();
            labelDesgosto.text = predictions[5].ToString();
            labelMedo.text = predictions[6].ToString();
            labelDesprezo.text = predictions[7].ToString();

            inputTensor.Dispose();
            outputTensor.Dispose();
        }
    }

    public float[] ResizeToGrayArray(Texture2D source, int width, int height)
    {
        float[] grayData = new float[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // pega cor da imagem original redimensionada
                Color c = source.GetPixelBilinear((float)x / width, (float)y / height);

                // converte para escala de cinza
                float gray = (float)Math.Round(c.grayscale * 255, 0);

                // salva no array (linha major order)
                grayData[y * width + x] = gray;
            }
        }

        return grayData;
    }

    Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        // Criar RenderTexture com o novo tamanho
        RenderTexture rt = new RenderTexture(width, height, 24);
        RenderTexture.active = rt;

        // Renderizar a textura original para a RenderTexture
        Graphics.Blit(source, rt);

        // Criar uma nova Texture2D e copiar os pixels da RenderTexture
        Texture2D resizedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        resizedTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        resizedTexture.Apply();

        // Liberar recursos
        RenderTexture.active = null;
        rt.Release();

        return resizedTexture;
    }

    void SavePixelsToCSV(float[] grayscalePixels, string fileName, int width)
    {
        // Criar o caminho do arquivo
        string path = Application.persistentDataPath + "/" + fileName;
        CultureInfo ci = CultureInfo.InvariantCulture;  // Garante o uso de ponto decimal

        // Abrir o arquivo para escrita
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            for (int i = 0; i < grayscalePixels.Length; i++)
            {
                writer.Write(grayscalePixels[i].ToString(ci));
                if ((i + 1) % width == 0)
                    writer.WriteLine();
                else
                    writer.Write(";");
            }
        }

        Debug.Log("Arquivo CSV salvo em: " + path);
    }

    void OnDestroy()
    {
        worker.Dispose();
    }
}
*/