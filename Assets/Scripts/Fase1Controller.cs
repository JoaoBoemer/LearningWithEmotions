using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Fase1Controller : MonoBehaviour
{
    public Image imagemPergunta; // arraste no Inspector
    private ImagensFaseManager imagensManager;

    public List<Pergunta> bancoPerguntas; // Todas as perguntas possíveis
    private Queue<Pergunta> filaPerguntas;
    private int acertos;

    // Lista fixa de todas as emoções possíveis
    private readonly string[] todasEmocoes = { "Alegria", "Tristeza", "Medo", "Raiva", "Surpresa", "Nojo" };

    void Start()
    {
        imagensManager = FindFirstObjectByType<ImagensFaseManager>();
        TipoEmocao emocaoDaPergunta = TipoEmocao.Alegria;

        // Pega uma imagem aleatória dessa emoção
        Sprite imagem = imagensManager.ObterImagemAleatoria(emocaoDaPergunta);

        // Troca a imagem no UI
        if (imagem != null)
            imagemPergunta.sprite = imagem;
        else
            Debug.LogWarning($"Sem imagens disponíveis para {emocaoDaPergunta}");
        // CarregarEmocoesConfiguradas();
        // CriarFilaDePerguntas();
        // MostrarProximaPergunta();
    }

    void CarregarEmocoesConfiguradas()
    {
        // Recupera do PlayerPrefs as emoções ativas
        List<string> emocoesAtivas = new List<string>();

        foreach (var emocao in todasEmocoes)
        {
            if (PlayerPrefs.GetInt("Emocao_" + emocao, 1) == 1) // 1 = ativa, 0 = desativada
                emocoesAtivas.Add(emocao);
        }

        // Se por algum motivo todas foram desativadas, usa todas
        if (emocoesAtivas.Count == 0)
            emocoesAtivas = todasEmocoes.ToList();

        // Guarda no campo para uso no resto do script
        emocaoConfiguradas = emocoesAtivas;
    }

    public List<string> emocaoConfiguradas { get; private set; }

    void CriarFilaDePerguntas()
    {
        List<Pergunta> listaFinal = new List<Pergunta>();
        int qtdEmocoes = emocaoConfiguradas.Count;

        // Distribuição para sempre ter 6 perguntas no total
        Dictionary<int, int[]> distribuicao = new Dictionary<int, int[]>
        {
            {1, new int[] {6}},
            {2, new int[] {3,3}},
            {3, new int[] {2,2,2}},
            {4, new int[] {2,2,1,1}},
            {5, new int[] {2,1,1,1,1}},
            {6, new int[] {1,1,1,1,1,1}}
        };

        int[] qtdPorEmocao = distribuicao[qtdEmocoes];

        // Monta a lista final
        for (int i = 0; i < qtdEmocoes; i++)
        {
            var perguntasDaEmocao = bancoPerguntas
                .Where(p => p.emocao == emocaoConfiguradas[i])
                .OrderBy(x => Random.value)
                .Take(qtdPorEmocao[i])
                .ToList();

            listaFinal.AddRange(perguntasDaEmocao);
        }

        // Embaralha todas
        listaFinal = listaFinal.OrderBy(x => Random.value).ToList();
        filaPerguntas = new Queue<Pergunta>(listaFinal);
    }

    public void Responder(string emocaoEscolhida)
    {
        var perguntaAtual = filaPerguntas.Peek();

        if (perguntaAtual.emocao == emocaoEscolhida)
        {
            acertos++;
            filaPerguntas.Dequeue();

            if (acertos >= 6)
            {
                VencerFase();
                return;
            }
            MostrarProximaPergunta();
        }
        else
        {
            Debug.Log("Tente novamente");
        }
    }

    void    MostrarProximaPergunta()
    {
        if (filaPerguntas.Count > 0)
        {
            var pergunta = filaPerguntas.Peek();
            Debug.Log("Mostrando pergunta: " + pergunta.descricao);
        }
    }

    void VencerFase()
    {
        Debug.Log("Parabéns! Você completou a fase.");
    }
}

[System.Serializable]
public class Pergunta
{
    public string descricao;
    public Sprite imagem;
    public string emocao;
}
