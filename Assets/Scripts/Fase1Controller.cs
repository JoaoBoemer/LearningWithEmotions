using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Fase1Controller : MonoBehaviour
{
    public Image imagemPergunta; // arraste no Inspector
    private ImagensFaseManager imagensManager;

    List<TipoEmocao> emocoesAtivas = new List<TipoEmocao>();
    private Dictionary<TipoEmocao, int> contadorEmocoes;

    [Header("Configurações da fase")]
    public int numeroDePerguntas = 6;
    private int perguntasRespondidas = 0;
    private TipoEmocao emocaoAtualPergunta; // emoção correta da pergunta atual

    void Start()
    {
        imagensManager = FindFirstObjectByType<ImagensFaseManager>();

        emocoesAtivas = ObterEmocoesAtivas();

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

        // Descobre o menor número de vezes usado
        int minimo = contadorEmocoes.Values.Min();

        // Filtra todas as emoções que têm esse valor mínimo
        var candidatas = contadorEmocoes
            .Where(kvp => kvp.Value == minimo)
            .Select(kvp => kvp.Key)
            .ToList();

        // Escolhe aleatoriamente entre as candidatas
        emocaoAtualPergunta = candidatas[Random.Range(0, candidatas.Count)];

        // Marca que foi usada
        contadorEmocoes[emocaoAtualPergunta]++;

        // Pega imagem aleatória dessa emoção
        Sprite imagem = imagensManager.ObterImagemAleatoria(emocaoAtualPergunta);

        // Troca a imagem no UI
        if (imagem != null)
            imagemPergunta.sprite = imagem;
        else
            Debug.LogWarning($"Sem imagens disponíveis para {emocaoAtualPergunta}");
    }

    public void Responder(int emocaoIndex)
    {
        // Converte o índice para o enum TipoEmocao
        TipoEmocao emocaoEscolhida = (TipoEmocao)emocaoIndex;

        if (emocaoEscolhida == emocaoAtualPergunta)
        {
            Debug.Log("Resposta correta!");
            ProximaPergunta();
        }
        else
        {
            Debug.Log("Resposta incorreta, tente novamente.");
            // Não avança, fica na mesma pergunta
        }
    }
    
    private void EncerrarFase()
    {
        Debug.Log("Fim da fase! Todas as perguntas foram feitas.");
    }
}