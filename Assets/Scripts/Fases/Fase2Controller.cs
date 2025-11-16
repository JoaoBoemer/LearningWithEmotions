using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Fase2Controller : MonoBehaviour
{
    public TextMeshProUGUI textoEmocaoDetectada;
    private ImagensFaseManager imagensManager;
    public Image imagemPergunta;
    public FeedbackManager feedbackManager;

    [Header("Configurações da fase")]
    public int numeroDePerguntas = 6;
    private int perguntasRespondidas = 0;
    private Dictionary<TipoEmocao, int> contadorEmocoes;
    private TipoEmocao emocaoAlvo;
    List<TipoEmocao> emocoesAtivas = new List<TipoEmocao>();
    public StarManager starManager;
    public TelaVitoria telaVitoria;
    [SerializeField] private GameObject painelJogo;
    [SerializeField] private GameObject painelTelaVitoria;

    void Start()
    {
        imagensManager = FindFirstObjectByType<ImagensFaseManager>();

        // starManager = FindFirstObjectByType<StarManager>(); 
        starManager.ResetStars(); // Reseta as estrelas no início da fase

        // telaVitoria = FindFirstObjectByType<TelaVitoria>();

        emocoesAtivas = ObterEmocoesAtivas();

        numeroDePerguntas = emocoesAtivas.Count;

        IniciarContador();

        ProximaPergunta();
    }

    // Essa função deve ser chamada pela sua rotina de detecção a cada 1 segundo
    public void VerificarEmocao(TipoEmocao emocaoDetectada)
    {
        return;
        if (textoEmocaoDetectada != null)
            textoEmocaoDetectada.text = emocaoDetectada.ToString();

        if (emocaoDetectada == emocaoAlvo)
        {
            starManager.AddStar();
            telaVitoria.AddCorreta(emocaoAlvo);
            feedbackManager.PlayFeedbackSimples(true);
        }
        else
        {
            telaVitoria.AddIncorreta(emocaoAlvo);
            starManager.IncorrectFeedback();
            feedbackManager.PlayFeedbackSimples(false);
        }

        perguntasRespondidas++;
        ProximaPergunta();
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

    void ProximaPergunta()
    {
        if (perguntasRespondidas >= numeroDePerguntas)
        {
            painelTelaVitoria.SetActive(true);
            telaVitoria.DisplayResults();
            painelJogo.SetActive(false);
            return;
        }
        
        // Descobre o menor número de vezes usado
        int minimo = contadorEmocoes.Values.Min();

        // Filtra todas as emoções que têm esse valor mínimo
        var candidatas = contadorEmocoes
            .Where(kvp => kvp.Value == minimo)
            .Select(kvp => kvp.Key)
            .ToList();

        // Escolhe aleatoriamente entre as candidatas
        emocaoAlvo = candidatas[Random.Range(0, candidatas.Count)];

        // Marca que foi usada
        contadorEmocoes[emocaoAlvo]++;

        // Pega imagem aleatória dessa emoção
        Sprite imagem = imagensManager.ObterImagemAleatoria(emocaoAlvo);

        // Troca a imagem no UI
        if (imagem != null)
            imagemPergunta.sprite = imagem;
        else
            Debug.LogWarning($"Sem imagens disponíveis para {emocaoAlvo}");
    }

    private void IniciarContador()
    {
        contadorEmocoes = new Dictionary<TipoEmocao, int>();
        foreach (var emocao in emocoesAtivas)
            contadorEmocoes[emocao] = 0;
    }
}
