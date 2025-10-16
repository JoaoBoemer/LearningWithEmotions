using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class TelaVitoria : MonoBehaviour
{
    public int Fase = 0;
    private List<TipoEmocao> emocoesCorretas = new List<TipoEmocao>();
    private List<TipoEmocao> emocoesIncorretas = new List<TipoEmocao>();

    [Header("UI References")]
    public GameObject resultRowPrefab; 
    public Transform contentParent;     

    [Header("Sprite References")]
    // Sprite da estrela preenchida (Dourada)
    public Sprite goldStar;     
    // Sprite da estrela vazia/opaca (Cinza)
    public Sprite grayStar;

    public void VoltarMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ProximaFase()
    {
        if (Fase > 0)
            SceneManager.LoadScene("Fase" + Fase);
    }

    public void AddCorreta(TipoEmocao emocao)
    {
        emocoesCorretas.Add(emocao);
    }

    public void AddIncorreta(TipoEmocao emocao)
    {
        emocoesIncorretas.Add(emocao);
    }

    /// <summary>
    /// Itera sobre a lista de resultados e instancia o prefab da linha de emoção.
    /// </summary>
    public void DisplayResults()
    {
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var result in emocoesCorretas)
        {
            // 1. INSTANCIAÇÃO
            // Cria uma nova linha de resultado e a anexa ao contentParent (GradeAcertos)
            GameObject newRow = Instantiate(resultRowPrefab, contentParent);

            // 2. ACESSO AOS COMPONENTES E LÓGICA DE EXIBIÇÃO
            
            // A. Texto da Emoção (Filho 0: Texto)
            TextMeshProUGUI emotionText = newRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            emotionText.text = result.ToString(); // Coloca em caixa alta para clareza

            // B. Imagem da Estrela (Filho 1: Imagem)
            Image starImage = newRow.transform.GetChild(2).GetComponent<Image>();
            
            starImage.sprite = goldStar;
        }
        
        foreach (var result in emocoesIncorretas)
        {
            // 1. INSTANCIAÇÃO
            // Cria uma nova linha de resultado e a anexa ao contentParent (GradeAcertos)
            GameObject newRow = Instantiate(resultRowPrefab, contentParent);

            // 2. ACESSO AOS COMPONENTES E LÓGICA DE EXIBIÇÃO

            // A. Texto da Emoção (Filho 0: Texto)
            TextMeshProUGUI emotionText = newRow.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            emotionText.text = result.ToString(); // Coloca em caixa alta para clareza

            // B. Imagem da Estrela (Filho 1: Imagem)
            Image starImage = newRow.transform.GetChild(2).GetComponent<Image>();

            starImage.sprite = grayStar;
        }
    }
}
