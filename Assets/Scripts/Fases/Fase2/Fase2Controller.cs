using UnityEngine;

public class Fase2Controller : MonoBehaviour
{
    [SerializeField] private GameObject painelTelaVitoria;
    [SerializeField] private GameObject painelJogo;
    private StarManager starManager; // Referência ao seu StarManager
    private int perguntasRespondidas = 0;
    public TelaVitoria telaVitoria;
    public void Start()
    {
        starManager = FindFirstObjectByType<StarManager>();
        starManager.ResetStars(); // Reseta as estrelas no início da fase
    }
    public void RespostaCorreta(TipoEmocao emocao)
    {
        starManager.AddStar();
        perguntasRespondidas++;

        telaVitoria.AddCorreta(emocao);

        if (perguntasRespondidas == 6)
        {
            EncerrarFase();
        }
    }

    public void RespostaIncorreta(TipoEmocao emocao)
    {
        perguntasRespondidas++;
        telaVitoria.AddIncorreta(emocao);

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
