using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fase3Controller : MonoBehaviour
{
    public TextMeshProUGUI textoEmocaoAlvo; // UI para mostrar a emoção alvo
    public TextMeshProUGUI textoFim; // UI para mostrar mensagem final
    public TextMeshProUGUI textoEmocaoDetectada; // UI para mostrar a emoção alvo
    private TipoEmocao emocaoAlvo;
    List<TipoEmocao> emocoesAtivas = new List<TipoEmocao>();
    private ImagensFaseManager imagensManager;
    public Image imagemPergunta; // arraste no Inspector
    private int indiceAtual = 0;
    private int pontos = 0;

    void Start()
    {
        textoFim.gameObject.SetActive(false);
        emocoesAtivas = ObterEmocoesAtivas();
        imagensManager = FindFirstObjectByType<ImagensFaseManager>();
        definirNovaEmocao();
    }

    // Essa função deve ser chamada pela sua rotina de detecção a cada 1 segundo
    public void VerificarEmocao(TipoEmocao emocaoDetectada)
    {
        textoEmocaoDetectada.text = emocaoDetectada.ToString();
        if (emocaoDetectada == emocaoAlvo)
        {
            proximaEmocao();
        }
    }

    void definirNovaEmocao()
    {
        if (indiceAtual < emocoesAtivas.Count)
        {
            emocaoAlvo = emocoesAtivas[Random.Range(0, emocoesAtivas.Count)];
            textoEmocaoAlvo.text = "Mostre: " + emocaoAlvo;
            Sprite imagem = imagensManager.ObterImagemAleatoria(emocaoAlvo);

            if (imagem != null)
            imagemPergunta.sprite = imagem;
        }
        else
        {
            textoEmocaoAlvo.gameObject.SetActive(false);
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
            if (!desativadas.Contains((int)emocao) && emocao != TipoEmocao.Neutro)
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
