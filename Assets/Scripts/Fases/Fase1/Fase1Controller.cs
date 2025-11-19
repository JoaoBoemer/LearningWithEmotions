using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Fase1Controller : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject prefabEmoji;          // Emoji arrastável
    public GameObject prefabDropZone;       // Área onde solta o emoji
    
    [Header("Organização na UI")]
    public Transform areaEmojis;            // Grid onde aparecem os 6 emojis
    public Transform areaDropZones;         // Grid das áreas de soltura

    [Header("Sprites")]
    // public Sprite[] spritesEmocoes;         // 1 sprite por TipoEmocao
    // public Sprite[] spritesAreas;           // 1 sprite por TipoEmocao (área sombra)
    
    [Header("Painéis")]
    public GameObject painelTelaVitoria;
    public GameObject painelJogo;
    private List<TipoEmocao> emocoesAtivas = new List<TipoEmocao>();
    private List<TipoEmocao> lista6Emocoes = new List<TipoEmocao>();
    private int perguntasRespondidas = 0;
    public StarManager starManager;
    public TelaVitoria telaVitoria;
    public FeedbackManager feedbackManager;
    private ImagensFaseManager imagensManager;
    public float padding = 80f; // distância mínima da borda

    public void Start()
    {
        emocoesAtivas = ObterEmocoesAtivas();
        imagensManager = FindFirstObjectByType<ImagensFaseManager>();

        // starManager = FindFirstObjectByType<StarManager>();
        starManager.ResetStars(); // Reseta as estrelas no início da fase
        
        CriarListaDe6Emocoes();
        CriarEmojisNaTela();
        CriarDropZones();
    }

    private List<TipoEmocao> ObterEmocoesAtivas()
    {
        var lista = new List<TipoEmocao>();

        string desativadasStr = PlayerPrefs.GetString("EmocoesDesativadas", "");
        var desativadas = new HashSet<int>();

        if (!string.IsNullOrEmpty(desativadasStr))
        {
            desativadas = desativadasStr
                .Split(',')
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(int.Parse)
                .ToHashSet();
        }

        foreach (TipoEmocao e in System.Enum.GetValues(typeof(TipoEmocao)))
        {
            if (e != TipoEmocao.Neutro && !desativadas.Contains((int)e))
                lista.Add(e);
        }

        return lista;
    }

    private void CriarListaDe6Emocoes()
    {
        lista6Emocoes = new List<TipoEmocao>(emocoesAtivas);

        // Se tiver menos de 6, duplica até ter 6
        while (lista6Emocoes.Count < 6)
        {
            lista6Emocoes.Add(
                emocoesAtivas[Random.Range(0, emocoesAtivas.Count)]
            );
        }

        // Embaralhar
        lista6Emocoes = lista6Emocoes
            .OrderBy(x => Random.value)
            .ToList();
    }

    private void CriarEmojisNaTela()
    {
        foreach (var emocao in lista6Emocoes)
        {
            GameObject obj = Instantiate(prefabEmoji, areaEmojis);

            var drag = obj.GetComponent<DraggableEmoji>();
            drag.emotion = emocao;
            drag.emotionTag = emocao.ToString();

            var sprite = imagensManager.ObterImagemAleatoria(emocao);
            if (sprite != null)
                obj.GetComponent<Image>().sprite = sprite;
        }
    }

    private void CriarDropZones()
    {
        RectTransform area = areaDropZones.GetComponent<RectTransform>();

        List<Vector2> posicoesFixas = CalcularPosicoesFixas(area);

        int index = 0;

        foreach (var emocao in emocoesAtivas)
        {
            GameObject dz = Instantiate(prefabDropZone, areaDropZones);

            var drop = dz.GetComponent<EmojiDropZone>();
            drop.requiredEmotion = emocao;
            drop.requiredEmotionTag = emocao.ToString();

            dz.tag = emocao.ToString();

            Transform textoObj = dz.transform.Find("texto");
            if (textoObj != null)
            {
                var textoUI = textoObj.GetComponent<TMPro.TextMeshProUGUI>();
                if (textoUI != null)
                    textoUI.text = emocao.ToString();
            }

            RectTransform rt = dz.GetComponent<RectTransform>();
            rt.anchoredPosition = posicoesFixas[index];

            index++;

            if (index >= posicoesFixas.Count)
                break;
            }
    }

    private List<Vector2> CalcularPosicoesFixas(RectTransform area)
    {
        List<Vector2> pos = new List<Vector2>();

        float left   = area.rect.xMin;
        float right  = area.rect.xMax;
        float top    = area.rect.yMax;
        float bottom = area.rect.yMin;
        float midY   = area.rect.center.y;

        // 1 - Top Left
        pos.Add(new Vector2(left + padding, top));

        // 2 - Top Right
        pos.Add(new Vector2(right - padding, top));

        // 3 - Bottom Left
        pos.Add(new Vector2(left + padding, bottom));

        // 4 - Bottom Right
        pos.Add(new Vector2(right - padding, bottom));

        // 5 - Middle Left
        pos.Add(new Vector2(left, midY));

        // 6 - Middle Right
        pos.Add(new Vector2(right, midY));

        return pos;
    }

    public void RespostaCorreta(TipoEmocao emocao)
    {
        starManager.AddStar();
        perguntasRespondidas++;

        telaVitoria.AddCorreta(emocao);
        feedbackManager.PlayFeedbackSimples(true);

        if (perguntasRespondidas == 6)
        {
            EncerrarFase();
        }
    }

    public void RespostaIncorreta(TipoEmocao emocao)
    {
        perguntasRespondidas++;
        telaVitoria.AddIncorreta(emocao);
        feedbackManager.PlayFeedbackSimples(false);
        starManager.IncorrectFeedback();

        if (perguntasRespondidas == 6)
        {
            EncerrarFase();
        }
    }

    private void EncerrarFase()
    {
        telaVitoria.DisplayResults();
        painelTelaVitoria.SetActive(true);
        painelJogo.SetActive(false);
    }
}
