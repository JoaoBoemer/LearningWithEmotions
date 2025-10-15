using UnityEngine;

public class Fase2Controller : MonoBehaviour
{
    [SerializeField] private GameObject painelTelaVitoria;
    [SerializeField] private GameObject painelJogo;
    private StarManager starManager; // Referência ao seu StarManager
    private int perguntasRespondidas = 0;
    public void Start()
    {
        starManager = FindFirstObjectByType<StarManager>();
        starManager.ResetStars(); // Reseta as estrelas no início da fase
    }
    public void RespostaCorreta()
    {
        starManager.AddStar();
        perguntasRespondidas++;

        if (perguntasRespondidas == 6)
        {
            painelTelaVitoria.SetActive(true);
            painelJogo.SetActive(false);
        }
    }
}
