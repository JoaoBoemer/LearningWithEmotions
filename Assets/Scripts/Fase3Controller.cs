using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Fase3Controller : MonoBehaviour
{
    public TextMeshProUGUI textoEmocaoAtual; // UI para mostrar a emoção alvo
    public TextMeshProUGUI textoPontuacao; // UI para mostrar a pontuação atual
    public TextMeshProUGUI textoFim; // UI para mostrar mensagem final
    List<TipoEmocao> emocoesAtivas = new List<TipoEmocao>();
    private List<string> emocoes = new List<string> { "Alegria", "Surpresa", "Tristeza", "Raiva", "Nojo", "Medo" };
    private int indiceAtual = 0;
    private int pontos = 0;
    private TipoEmocao emocaoAtual;

    void Start()
    {
        textoFim.gameObject.SetActive(false);
        emocoesAtivas = ObterEmocoesAtivas();
        definirNovaEmocao();
    }

    // Essa função deve ser chamada pela sua rotina de detecção a cada 1 segundo
    public void VerificarEmocao(TipoEmocao emocaoDetectada)
    {
        if (emocaoDetectada == emocaoAtual)
        {
            pontos++;
            textoPontuacao.text = "Pontos: " + pontos;

            if (pontos >= 5)
            {
                proximaEmocao();
            }
        }
    }

    void definirNovaEmocao()
    {
        if (indiceAtual < emocoesAtivas.Count)
        {
            emocaoAtual = emocoesAtivas[Random.Range(0, emocoesAtivas.Count)];
            pontos = 0;
            textoPontuacao.text = "Pontos: 0";
            textoEmocaoAtual.text = "Mostre: " + emocaoAtual;
        }
        else
        {
            textoEmocaoAtual.gameObject.SetActive(false);
            textoPontuacao.gameObject.SetActive(false);
            textoFim.gameObject.SetActive(true);
            textoFim.text = "Parabéns! Você completou todas as emoções.";
        }
    }

    private List<TipoEmocao> ObterEmocoesAtivas()
    {
        var emocoesAtivas = new List<TipoEmocao>();

        // Recupera a string salva no PlayerPrefs (emoções desativadas)
        string desativadasStr = PlayerPrefs.GetString("EmocoesDesativadas", "");
        var desativadas = new HashSet<int>();

        if (!string.IsNullOrEmpty(desativadasStr))
        {
            // Transforma "0,1,2" em lista de ints
            desativadas = desativadasStr
                .Split(',')
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(int.Parse)
                .ToHashSet();
        }

        // Agora percorre todas as emoções do enum
        foreach (TipoEmocao emocao in System.Enum.GetValues(typeof(TipoEmocao)))
        {
            if (!desativadas.Contains((int)emocao))
                emocoesAtivas.Add(emocao);
        }

        return emocoesAtivas;
    }

    void proximaEmocao()
    {
        indiceAtual++;
        definirNovaEmocao();
    }
}
