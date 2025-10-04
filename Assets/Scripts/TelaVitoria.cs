using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaVitoria : MonoBehaviour
{
    public int Fase = 0;
    public void VoltarMenu()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void ProximaFase()
    {
        if(Fase > 0)
            SceneManager.LoadScene("Fase" + Fase);
    }
}
