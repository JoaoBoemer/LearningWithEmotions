using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class EmojiDropZone : MonoBehaviour, IDropHandler
{
    // A emoção que este retângulo ACEITA (deve corresponder ao emotionTag do Emoji)
    public string requiredEmotionTag;
    public TipoEmocao requiredEmotion;
    private Fase1Controller fase1Controller;
    
    // NOVAS VARIÁVEIS PARA FEEDBACK
    private Image dropZoneImage;
    private Color originalColor;
    [Header("Configurações de Feedback")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failureColor = Color.red;
    [SerializeField] private float fadeDuration = 0.5f; // Tempo para voltar ao normal

    void Start()
    {
        fase1Controller = FindFirstObjectByType<Fase1Controller>();

        dropZoneImage = GetComponent<Image>();
        if (dropZoneImage != null)
        {
            originalColor = dropZoneImage.color;
        }
        else
        {
            Debug.LogError("O objeto Drop Zone deve ter um componente Image para feedback de cor.");
        }
    }

    // O que acontece quando um objeto é solto nesta área
    public void OnDrop(PointerEventData eventData)
    {
        // Tenta obter o script DraggableEmoji do objeto que foi solto
        DraggableEmoji droppedEmoji = eventData.pointerDrag.GetComponent<DraggableEmoji>();

        // Verifica se é realmente um Emoji arrastável
        if (droppedEmoji != null)
        {
            // Verifica se a tag do Emoji corresponde à tag que este retângulo aceita
            if (droppedEmoji.emotion == requiredEmotion)
            {
                // 1. Define que o Emoji foi "encaixado"
                StartColorFeedback(successColor); // Chamar feedback VERDE

                droppedEmoji.isSnapped = true;
                droppedEmoji.gameObject.SetActive(false);
                droppedEmoji.enabled = false;
                fase1Controller.RespostaCorreta(droppedEmoji.emotion);
            }
            else
            {
                StartColorFeedback(failureColor); // Chamar feedback VERMELHO

                droppedEmoji.gameObject.SetActive(false);
                droppedEmoji.enabled = false;
                fase1Controller.RespostaIncorreta(droppedEmoji.emotion);
            }
        }
    }

    private void StartColorFeedback(Color targetColor)
    {
        // Interrompe qualquer animação de cor anterior
        StopAllCoroutines();

        // Inicia a nova animação
        StartCoroutine(AnimateColorFeedback(targetColor));
    }
    
    private IEnumerator AnimateColorFeedback(Color targetColor)
    {
        // 1. Mudar IMEDIATAMENTE para a cor de feedback (verde ou vermelho)
        dropZoneImage.color = targetColor;

        // 2. Esperar um frame para garantir a mudança visual
        yield return null; 

        // 3. Fazer a cor retornar gradualmente para a cor original
        float time = 0;
        Color startColor = dropZoneImage.color;
        
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            
            // Suaviza a transição (opcional: Use 't * t' para aceleração)
            dropZoneImage.color = Color.Lerp(startColor, originalColor, t);
            
            yield return null;
        }

        // 4. Garante que a cor final seja exatamente a cor original
        dropZoneImage.color = originalColor;
    }
}