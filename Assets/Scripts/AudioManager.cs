    using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // Carregar volumes salvos (ou valor padrão caso não exista)
        float music = PlayerPrefs.GetFloat("musicaVolume", 0.75f);
        float sfx = PlayerPrefs.GetFloat("sfxVolume", 0.75f);

        // Atualizar mixer
        SetMusicVolume(music);
        SetSFXVolume(sfx);

        // Atualizar sliders na UI
        if (musicSlider != null)
            musicSlider.value = music;

        if (sfxSlider != null)
            sfxSlider.value = sfx;

        // Adicionar eventos nos sliders
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(SetSFXVolume);
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
}
