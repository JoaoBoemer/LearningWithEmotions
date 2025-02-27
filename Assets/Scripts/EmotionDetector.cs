using UnityEngine;
using UnityEngine.UI;
using Unity.Barracuda;
using System;
using System.Collections;
using System.Linq;

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

        // Inicia a previsão de emoções a cada 1 segundo
        // StartCoroutine(PredictEmotionRoutine());
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
            // Criar uma textura com o tamanho esperado pelo modelo (48x48)
            Texture2D texture = new Texture2D(48, 48);
            texture.SetPixels(webcamTexture.GetPixels());
            texture.Apply();

            // Converter a imagem para escala de cinza
            Color[] pixels = texture.GetPixels();
            float[] grayscalePixels = new float[pixels.Length];
            
            for (int i = 0; i < pixels.Length; i++)
            {
                grayscalePixels[i] = pixels[i].grayscale; // Calcula a média RGB para obter tons de cinza
            }

            // Criar um tensor com 1 canal (48x48x1)
            Tensor inputTensor = new Tensor(new TensorShape(1, 48, 48, 1), grayscalePixels);

            // Fazer a inferência
            worker.Execute(inputTensor);
            Tensor outputTensor = worker.PeekOutput();

            // Obter os valores do tensor de saída
            float[] predictions = new float[7]; // Para 7 emoções
            for (int i = 0; i < 7; i++)
            {
                predictions[i] = outputTensor[0, 0, 0, i]; // Cada valor para cada emoção
            }

            // Encontrar o índice da maior probabilidade
            int maxIndex = Mathf.FloorToInt(Array.IndexOf(predictions, predictions.Max()));

            // Exibir o resultado
            Debug.Log("Emoção Detectada: " + emotionLabels[maxIndex]);

            inputTensor.Dispose();
            outputTensor.Dispose();
        }
    }


    void OnDestroy()
    {
        worker.Dispose();
    }
}
