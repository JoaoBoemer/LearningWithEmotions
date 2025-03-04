using UnityEngine;
using System.Globalization;
using Unity.Barracuda;
using System;
using System.Collections;
using System.Linq;
using System.IO;

public class EmotionDetector : MonoBehaviour
{
    public NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;
    private WebCamTexture webcamTexture; // Não inicializa aqui
    string[] emotionLabels = new string[] {
        "Feliz", "Triste", "Raiva", "Surpresa", "Medo", "Desprezo", "Neutral"
    };

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

        webcamTexture = CameraCapture.instance.GetCameraTexture();

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

            // Criar uma textura com o tamanho esperado pelo modelo (48x48)
            Texture2D texture = ResizeTexture(tempTexture, 48, 48);

            // Converter a imagem para escala de cinza
            Color[] pixels = texture.GetPixels();
            float[] grayscalePixels = new float[pixels.Length];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                grayscalePixels[i] = pixels[i].grayscale; // Calcula a média RGB para obter tons de cinza
            }

            SavePixelsToCSV(grayscalePixels, "grayscalePixels.csv");

            Tensor inputTensor = new Tensor(new TensorShape(1, 48, 48, 1), grayscalePixels);

            // Fazer a inferência
            worker.Execute(inputTensor);
            Tensor outputTensor = worker.PeekOutput();

            Debug.Log("Saída do modelo (Unity): " + outputTensor.ToString());

            // Obter os valores do tensor de saída
            float[] predictions = new float[7]; // Para 7 emoções
            for (int i = 0; i < 7; i++)
            {
                predictions[i] = outputTensor[0, 0, 0, i]; // Cada valor para cada emoção
            }

            // Exibir todas as probabilidades de emoções
            for (int i = 0; i < predictions.Length; i++)
            {
                Debug.Log(emotionLabels[i] + ": " + predictions[i]);
            }

            // Encontrar o índice da maior probabilidade
            int maxIndex = Array.IndexOf(predictions, predictions.Max());

            // Exibir o resultado
            Debug.Log("Emoção Detectada: " + emotionLabels[maxIndex]);

            inputTensor.Dispose();
            outputTensor.Dispose();
        }
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
    
    void SavePixelsToCSV(float[] grayscalePixels, string fileName, int width = 48)
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
                if ((i + 1) % width == 0) // A cada 48 pixels (tamanho da imagem 48x48), pular uma linha
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
