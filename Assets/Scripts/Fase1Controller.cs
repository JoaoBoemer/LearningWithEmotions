using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Fase1Controller : MonoBehaviour
{
    public List<string> emocaoConfiguradas; // Ex: "Alegria", "Tristeza"
    public List<Pergunta> bancoPerguntas; // Todas as perguntas possíveis

    private Queue<Pergunta> filaPerguntas;
    private int acertos;

    void Start()
    {
        CriarFilaDePerguntas();
        MostrarProximaPergunta();
    }

    void CriarFilaDePerguntas()
    {
        List<Pergunta> listaFinal = new List<Pergunta>();
        int qtdEmocoes = emocaoConfiguradas.Count;

        // Define quantas perguntas por emoção
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
            // Feedback de erro
            Debug.Log("Tente novamente");
        }
    }

    void MostrarProximaPergunta()
    {
        if (filaPerguntas.Count > 0)
        {
            var pergunta = filaPerguntas.Peek();
            // Aqui você mostra a imagem e opções na UI
            Debug.Log("Mostrando pergunta: " + pergunta.descricao);
        }
    }

    void VencerFase()
    {
        Debug.Log("Parabéns! Você completou a fase.");
        // Chamar tela de vitória
    }
}

[System.Serializable]
public class Pergunta
{
    public string descricao;
    public Sprite imagem;
    public string emocao;
}
