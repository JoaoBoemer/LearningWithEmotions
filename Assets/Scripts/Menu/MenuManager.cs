using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private string nomeLevelJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelOpcoes;
    [SerializeField] private GameObject painelSelecaoFase;
    public Slider musicSlider;
    public Slider sfxSlider;

    void Awake()
    {
        // DontDestroyOnLoad(gameObject);
        GameSettings.Carregar();
        PlayerPrefs.SetInt("TutorialVisto1", 0);
        PlayerPrefs.SetInt("TutorialVisto2", 0);
        PlayerPrefs.SetInt("TutorialVisto3", 0);
        PlayerPrefs.SetInt("TutorialVisto4", 0);
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

    // Use o OnEnable() para garantir que a conexão ocorra
    // sempre que o painel de opções for ativado (ou no Start, se for sempre visível)
    void OnEnable()
    {
        // 1. Garante que o AudioManager existe
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager não encontrado. Verifique se ele está na cena inicial.");
            return;
        }

        // 2. Carrega os valores salvos (do PlayerPrefs) e aplica aos Sliders
        float musicVolume = PlayerPrefs.GetFloat("musicaVolume", 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 0.75f);
        
        musicSlider.value = musicVolume;
        sfxSlider.value = sfxVolume;

        // 3. Reconecta os eventos dos Sliders ao Singleton persistente
        // Primeiro, remova ouvintes antigos para evitar duplicação (se você usar OnEnable)
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        // Adiciona os novos ouvintes
        musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);

        // Opcional: Se os Sliders não estiverem em 0/1, force a chamada para aplicar os valores iniciais (embora o .value = já o faça)
        AudioManager.Instance.SetMusicVolume(musicVolume);
        AudioManager.Instance.SetSFXVolume(sfxVolume);
    }
}
