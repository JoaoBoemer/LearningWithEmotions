using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    [Header("Feedback de Acerto")]
    [SerializeField] private AudioClip acertoSFX;


    [Header("Feedback de Erro")]
    [SerializeField] private AudioClip erroSFX;
    
    public void PlayFeedbackSimples(bool acertou)
    {
        if (acertou)
        {
            AudioManager.Instance.PlaySFX(acertoSFX); 
        }
        else
        {
            AudioManager.Instance.PlaySFX(erroSFX);
        }
    }

    // private void Awake()
    // {
    //     // Configuração do Singleton
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject);
    //     }
    //     else
    //     {
    //         Instance = this;
    //     }
    // }

    // // Método para ser chamado quando o jogador acertar
    // // A 'position' é o lugar onde o efeito deve aparecer (ex: no objeto clicado)
    // public void PlayAcerto(Vector3 position)
    // {
    //     // Toca o efeito visual (partículas) na posição desejada
    //     if (acertoEffectPrefab != null)
    //     {
    //         GameObject effect = Instantiate(acertoEffectPrefab, position, Quaternion.identity);
    //         // Destroi o efeito depois de 2 segundos para não poluir a cena
    //         Destroy(effect, 2f);
    //     }

    //     // Toca o som de acerto
    //     if (acertoSound != null && audioSource != null)
    //     {
    //         // PlayOneShot é ideal para efeitos sonoros curtos e repetidos
    //         audioSource.PlayOneShot(acertoSound);
    //     }
    // }

    // // Método para ser chamado quando o jogador errar
    // public void PlayErro(Vector3 position)
    // {
    //     // Toca o efeito visual de erro
    //     if (erroEffectPrefab != null)
    //     {
    //         GameObject effect = Instantiate(erroEffectPrefab, position, Quaternion.identity);
    //         Destroy(effect, 2f);
    //     }

    //     // Toca o som de erro
    //     if (erroSound != null && audioSource != null)
    //     {
    //         audioSource.PlayOneShot(erroSound);
    //     }
    // }
}