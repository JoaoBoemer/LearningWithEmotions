using UnityEngine;
using UnityEngine.UI;

public class ConfiguracoesManager : MonoBehaviour
{
    public Toggle toggleAlegria;
    public Toggle toggleTristeza;
    public Toggle toggleMedo;
    public Toggle toggleRaiva;
    public Toggle toggleSurpresa;
    public Toggle toggleNojo;

    void Start()
    {
        GameSettings.Carregar();

        toggleAlegria.isOn = !GameSettings.EmocaoDesativada(TipoEmocao.Alegria);
        toggleTristeza.isOn = !GameSettings.EmocaoDesativada(TipoEmocao.Tristeza);
        toggleMedo.isOn = !GameSettings.EmocaoDesativada(TipoEmocao.Medo);
        toggleRaiva.isOn = !GameSettings.EmocaoDesativada(TipoEmocao.Raiva);
        toggleSurpresa.isOn = !GameSettings.EmocaoDesativada(TipoEmocao.Surpresa);
        toggleNojo.isOn = !GameSettings.EmocaoDesativada(TipoEmocao.Nojo);

        toggleAlegria.onValueChanged.AddListener((ativo) => AlterarEmocao(TipoEmocao.Alegria, ativo));
        toggleTristeza.onValueChanged.AddListener((ativo) => AlterarEmocao(TipoEmocao.Tristeza, ativo));
        toggleMedo.onValueChanged.AddListener((ativo) => AlterarEmocao(TipoEmocao.Medo, ativo));
        toggleRaiva.onValueChanged.AddListener((ativo) => AlterarEmocao(TipoEmocao.Raiva, ativo));
        toggleSurpresa.onValueChanged.AddListener((ativo) => AlterarEmocao(TipoEmocao.Surpresa, ativo));
        toggleNojo.onValueChanged.AddListener((ativo) => AlterarEmocao(TipoEmocao.Nojo, ativo));
    }

    void AlterarEmocao(TipoEmocao emocao, bool ativa)
    {
        if (ativa)
            GameSettings.RemoverEmocaoDesativada(emocao);
        else
            GameSettings.AdicionarEmocaoDesativada(emocao);

        GameSettings.Salvar();
    }
}
