using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Fase3Controller : MonoBehaviour
{
    public Image imagemPergunta; // arraste no Inspector
    private ImagensFaseManager imagensManager;
    [SerializeField] private GameObject painelTelaVitoria;
    [SerializeField] private GameObject painelJogo;

    List<TipoEmocao> emocoesAtivas = new List<TipoEmocao>();
    private Dictionary<TipoEmocao, int> contadorEmocoes;
    public Image personagem;
    public Sprite pensandoSprite;
    public Sprite erroSprite;
    public FeedbackManager feedbackManager;

    [Header("Configurações da fase")]
    public int numeroDePerguntas = 6;
    private int perguntasRespondidas = 0;
    private TipoEmocao emocaoAlvo;
    private StarManager starManager;
    public TelaVitoria telaVitoria;

    void Start()
    {
        imagensManager = FindFirstObjectByType<ImagensFaseManager>();

        emocoesAtivas = ObterEmocoesAtivas();

        starManager = FindFirstObjectByType<StarManager>(); 
        starManager.ResetStars(); // Reseta as estrelas no início da fase

        // Inicializa contador
        contadorEmocoes = new Dictionary<TipoEmocao, int>();
        foreach (var emocao in emocoesAtivas)
            contadorEmocoes[emocao] = 0;

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

    private void ProximaPergunta()
    {
        if (perguntasRespondidas >= numeroDePerguntas)
        {
            EncerrarFase();
            return;
        }

        // Não deve permitir por padrão nas configurações
        if (emocoesAtivas.Count == 0)
        {
            Debug.LogWarning("Nenhuma emoção ativa!");
            return;
        }

        BuscarProximaEmocao();
    }

    public void Responder(int emocaoIndex)
    {
        // Converte o índice para o enum TipoEmocao
        TipoEmocao emocaoEscolhida = (TipoEmocao)emocaoIndex;

        if (emocaoEscolhida == emocaoAlvo)
        {
            // personagem.sprite = pensandoSprite;
            starManager.AddStar();
            telaVitoria.AddCorreta(emocaoAlvo);
            feedbackManager.PlayFeedbackSimples(true);
        }
        else
        {
            telaVitoria.AddIncorreta(emocaoAlvo);
            starManager.IncorrectFeedback();
            feedbackManager.PlayFeedbackSimples(false);
            // personagem.sprite = erroSprite;
        }

        perguntasRespondidas++;
        ProximaPergunta();
    }

    private void EncerrarFase()
    {
        telaVitoria.DisplayResults();
        painelTelaVitoria.SetActive(true);
        painelJogo.SetActive(false);
    }

    private void BuscarProximaEmocao()
    {
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
}