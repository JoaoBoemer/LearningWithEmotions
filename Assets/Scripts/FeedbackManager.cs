using UnityEngine;

public class FeedbackManager : MonoBehaviour
{
    [Header("Feedback de Acerto")]
    [SerializeField] private AudioClip acertoSFX;


    [Header("Feedback de Erro")]
    [SerializeField] private AudioClip erroSFX;
    
    public void PlayFeedbackSimples(bool acertou)
    {
        if (AudioManager.Instance == null)
            return;
            
        if (acertou)
        {
            AudioManager.Instance.PlaySFX(acertoSFX); 
        }
        else
        {
            AudioManager.Instance.PlaySFX(erroSFX);
        }
    }
}