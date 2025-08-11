using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string nomeLevelJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelSelecaoFase;

    void Awake()
    {
        // DontDestroyOnLoad(gameObject);
        GameSettings.Carregar();
    }

    public void AbrirFase(int faseId)
    {
        string nomeFase = "Fase" + faseId;
        SceneManager.LoadScene(nomeFase);
    }
    public void Jogar()
    {
        painelMenuInicial.SetActive(false);
        painelSelecaoFase.SetActive(true);
    }

    public void FecharSelecaoFase()
    {
        painelSelecaoFase.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void AbrirOpcoes()
    {
        painelMenuInicial.SetActive(false);
        painelOpcoes.SetActive(true);
    }

    public void FecharOpcoes()
    {
        painelOpcoes.SetActive(false);
        painelMenuInicial.SetActive(true);
    }

    public void SairJogo()
    {
        Application.Quit();
    }
}
