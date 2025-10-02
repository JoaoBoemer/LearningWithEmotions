using UnityEngine;
using UnityEngine.SceneManagement;

public class TelaVitoria : MonoBehaviour
{
    public void SairFase()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
