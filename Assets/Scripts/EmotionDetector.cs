using UnityEngine;
using UnityEngine.UI;
using System.Globalization;
using Unity.Barracuda;
using System;
using System.Collections;
using System.Linq;
using System.IO;
using TMPro;

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

            /*
            // Criar uma textura com o tamanho esperado pelo modelo
            Texture2D texture = ResizeTexture(tempTexture, imageSizeX, imageSizeY);

            // Converter a imagem para escala de cinza
            Color[] pixels = texture.GetPixels();
            float[] grayscalePixels = new float[pixels.Length];

            for (int i = 0; i < pixels.Length; i++)
            {
                grayscalePixels[i] = pixels[i].grayscale*255; // Calcula a média RGB para obter tons de cinza
            }
            */

            float[] grayscalePixels = ResizeToGrayArray(tempTexture, 64, 64);
            
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

            // Teste
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

    TipoEmocao conversorEmocaoEnum(int emocao)
    {
        switch (emocao)
        {
            case 1:
                return TipoEmocao.Alegria;
            case 2:
                return TipoEmocao.Surpresa;
            case 3:
                return TipoEmocao.Tristeza;
            case 4:
                return TipoEmocao.Raiva;
            case 5:
                return TipoEmocao.Nojo;
            case 6:
                return TipoEmocao.Medo;
            default:
                return TipoEmocao.Neutro;
        }
    }

    void OnDestroy()
    {
        worker.Dispose();
    }
}
