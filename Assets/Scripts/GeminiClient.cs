using System.IO;
using UnityEngine;

// Classe para mapear o JSON
[System.Serializable]
public class ApiKeys
{
    public string gemini_api_key;
}

public class GeminiClient : MonoBehaviour
{
    private string apiKey;

    void Awake()
    {
        LoadApiKey();
    }

    void LoadApiKey()
    {
        string filePath = Path.Combine(Application.dataPath, "api_keys.json");
        
        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            ApiKeys keys = JsonUtility.FromJson<ApiKeys>(jsonContent);
            apiKey = keys.gemini_api_key;
            Debug.Log("Chave da API carregada com sucesso.");
        }
        else
        {
            Debug.LogError("Arquivo de chaves de API não encontrado. Por favor, crie 'api_keys.json' na pasta Assets.");
        }
    }

    public string GetApiKey()
    {
        return apiKey;
    }
}