using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    // Tornamos a chave serializada para que você possa alterá-la no Inspector!
    [Header("Configuração")]
    [Tooltip("Chave única usada no PlayerPrefs para esta fase (ex: Fase1Visto)")]
    [SerializeField] private string tutorialCompletedKey = "TutorialVistoPadrao";

    [Header("Componentes")]
    [SerializeField] private GameObject tutorialPanel; // O painel de fundo (Overlay)
    [SerializeField] private GameObject gamePanel; // O painel de fundo (Overlay)

    void Start()
    {
        // 1. Verifica se o tutorial já foi visto usando a chave definida no Inspector
        bool tutorialCompleto = PlayerPrefs.GetInt(tutorialCompletedKey, 0) == 1;

        if (!tutorialCompleto)
        {
            ShowTutorial();
        }
        else
        {
            HideTutorial();
        }
    }

    private void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        gamePanel.SetActive(false);
        Time.timeScale = 0f; // Pausa o jogo
    }

    // Método público chamado pelo botão "ENTENDI"
    public void OnTutorialComplete()
    {
        // 1. Salva o estado usando a chave definida no Inspector
        PlayerPrefs.SetInt(tutorialCompletedKey, 1);
        PlayerPrefs.Save(); 

        // 2. Fecha e despausa
        HideTutorial();
    }

    private void HideTutorial()
    {
        tutorialPanel.SetActive(false);
        gamePanel.SetActive(true);
        Time.timeScale = 1f; // Despausa o jogo
    }
}