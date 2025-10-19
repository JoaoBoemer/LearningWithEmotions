using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Awake()
    {
        // Implementação do Singleton
        if (Instance == null)
        {
            Instance = this;
            // Impede que este objeto seja destruído ao carregar novas cenas
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Se já existir outro AudioManager, destrói este (duplicado)
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadVolumes();
    }
    private void LoadVolumes()
    {
        // 1. Carregar volumes salvos (ou valor padrão)
        float music = PlayerPrefs.GetFloat("musicaVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", 0.75f);

        // 2. Aplicar os valores LIDOS diretamente ao AudioMixer
        // A UI (OptionsUI) fará sua própria leitura e configuração do Slider
        SetMusicVolume(music);
        SetSFXVolume(sfx);
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("musicaVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("musicaVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20);
        PlayerPrefs.SetFloat("sfxVolume", volume);
    }

    // Adicione este método ao seu script AudioManager (abaixo dos métodos SetVolume)

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;

        // Cria um objeto temporário para tocar o som
        GameObject tempAudio = new GameObject("TempAudio");
        AudioSource audioSource = tempAudio.AddComponent<AudioSource>();
        
        // Configura para usar o grupo de SFX do seu AudioMixer
        // Certifique-se de que a variável 'audioMixer' está configurada na Unity
        // e que o grupo de SFX tem o nome correto (Ex: "SFX")
        AudioMixerGroup[] groups = audioMixer.FindMatchingGroups("SFX");
        if (groups.Length > 0)
        {
            audioSource.outputAudioMixerGroup = groups[0];
        }
        // else, ele toca no volume padrão, o que é OK se o mixer estiver configurado
        
        audioSource.PlayOneShot(clip);
        
        // Destrói o objeto após o som terminar de tocar
        Destroy(tempAudio, clip.length);
    }
}
