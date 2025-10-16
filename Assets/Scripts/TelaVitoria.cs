using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class TelaVitoria : MonoBehaviour
{
    public int Fase = 0;
    private List<TipoEmocao> emocoesCorretas = new List<TipoEmocao>();
    private List<TipoEmocao> emocoesIncorretas = new List<TipoEmocao>();
    public List<Image> estrelas;

    public void VoltarMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ProximaFase()
    {
        if (Fase > 0)
            SceneManager.LoadScene("Fase" + Fase);
    }

    public void AddCorreta(TipoEmocao emocao)
    {
        emocoesCorretas.Add(emocao);
    }

    public void AddIncorreta(TipoEmocao emocao)
    {
        emocoesIncorretas.Add(emocao);
    }

    public void MostrarResultados()
    {
        for (int i = 0; i < estrelas.Count; i++)
        {
            estrelas[i].gameObject.SetActive(i < emocoesCorretas.Count);
        }
    }
}
