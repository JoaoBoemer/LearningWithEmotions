using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;
using System.Diagnostics;

public class EmotionDetector : MonoBehaviour
{
    public GeminiClient geminiClient;
    private string apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent";
    public Fase2Controller faseEmocoes;
    public Fase4Controller fase4Emocoes;
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
        btnDetectEmotion.onClick.AddListener(() => StartCoroutine(PredictEmotion()));
    }

    public IEnumerator PredictEmotion()
    {
        if (isProcessing)
            yield break;

        Stopwatch cronometro = new Stopwatch();

        cronometro.Start();

        isProcessing = true;
        btnDetectEmotion.interactable = false;

        // yield return new WaitForSeconds(10f);
        string apiKey = geminiClient.GetApiKey();

        string base64Image = cameraCapture.GetBase64();

        // 2. Cria o prompt de texto
        string prompt = @"Instrução: Você é um sistema especialista em classificação de emoções faciais. Com base nesta imagem, analise a emoção visível no rosto da pessoa. Sua resposta DEVE ser **APENAS** e **EXCLUSIVAMENTE** um dos seguintes termos, sem qualquer outra palavra, explicação ou pontuação, sempre em minusculo: raiva, alegria, tristeza, surpresa, medo, nojo. Em casos de ambiguidade ou mistura de emoções, escolha a emoção que for **MAIS DOMINANTE**. Caso nenhuma pessoa seja detectada, ou a expressão for completamente neutra, responda apenas: 'Neutro'";

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
                UnityEngine.Debug.LogError("Erro na requisição: " + www.error);
                if (faseEmocoes != null)
                    faseEmocoes.VerificarEmocao(TipoEmocao.Neutro);
                else if (fase4Emocoes != null)
                    fase4Emocoes.VerificarEmocao(TipoEmocao.Neutro);
            }
            else
            {
                string responseText = www.downloadHandler.text;

                try
                {
                    GeminiResponse response = JsonUtility.FromJson<GeminiResponse>(responseText);

                    TempoCronometro(cronometro);
                    if (response != null && response.candidates != null && response.candidates.Length > 0)
                    {
                        string emotion = response.candidates[0].content.parts[0].text;
                        UnityEngine.Debug.Log(emotion);

                        if (faseEmocoes != null)
                            faseEmocoes.VerificarEmocao(conversorEmocaoEnum(emotion));
                        else if (fase4Emocoes != null)
                            fase4Emocoes.VerificarEmocao(conversorEmocaoEnum(emotion));
                    }
                    else
                    {
                        UnityEngine.Debug.LogError("Resposta da API não contém um resultado válido.");
                        UnityEngine.Debug.Log("Resposta da API não contém um resultado válido.");
                        if (faseEmocoes != null)
                            faseEmocoes.VerificarEmocao(TipoEmocao.Neutro);
                        else if (fase4Emocoes != null)
                            fase4Emocoes.VerificarEmocao(TipoEmocao.Neutro);
                    }
                }
                catch (Exception e)
                {
                    UnityEngine.Debug.LogError("Erro ao processar a resposta JSON: " + e.Message);
                    UnityEngine.Debug.Log("Erro ao processar a resposta JSON: " + e.Message);
                    TempoCronometro(cronometro);
                    if (faseEmocoes != null)
                        faseEmocoes.VerificarEmocao(TipoEmocao.Neutro);
                    else if (fase4Emocoes != null)
                        fase4Emocoes.VerificarEmocao(TipoEmocao.Neutro);
                }
            }

            isProcessing = false;
            btnDetectEmotion.interactable = true;
        }
    }

    public void TempoCronometro(Stopwatch cronometro)
    {
        cronometro.Stop();

        TimeSpan tempoDecorrido = cronometro.Elapsed;

        UnityEngine.Debug.Log(tempoDecorrido);
    }
    
    TipoEmocao conversorEmocaoEnum(string emocao)
    {
        switch (emocao.ToLower() )
        {
            case "alegria":
                return TipoEmocao.Alegria;
            case "surpresa":
                return TipoEmocao.Surpresa;
            case "tristeza":
                return TipoEmocao.Tristeza;
            case "raiva":
                return TipoEmocao.Raiva;
            case "nojo":
                return TipoEmocao.Nojo;
            case "medo":
                return TipoEmocao.Medo;
            default:
                return TipoEmocao.Neutro;
        }
    }
}