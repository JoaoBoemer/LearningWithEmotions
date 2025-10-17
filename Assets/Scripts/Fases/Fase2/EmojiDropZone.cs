using UnityEngine;
using UnityEngine.EventSystems;

public class EmojiDropZone : MonoBehaviour, IDropHandler
{
    // A emoção que este retângulo ACEITA (deve corresponder ao emotionTag do Emoji)
    public string requiredEmotionTag;
    public TipoEmocao requiredEmotion;
    private Fase2Controller fase2Controller;

    void Start()
    {
        fase2Controller = FindFirstObjectByType<Fase2Controller>();
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
                // **SUCESSO:** O Emoji está correto!

                // 1. Define que o Emoji foi "encaixado"
                droppedEmoji.isSnapped = true;

                droppedEmoji.gameObject.SetActive(false);

                // 2. Faz o Emoji encaixar perfeitamente no centro do retângulo
                // droppedEmoji.transform.SetParent(transform, true); // Torna o retângulo o pai do Emoji
                // droppedEmoji.rectTransform.anchoredPosition = Vector2.zero; // Centraliza o Emoji

                // 3. Desativa o script de arrastar para que não possa ser movido novamente
                droppedEmoji.enabled = false;

                fase2Controller.RespostaCorreta(droppedEmoji.emotion);
            }
            else
            {
                droppedEmoji.gameObject.SetActive(false);
                droppedEmoji.enabled = false;
                fase2Controller.RespostaIncorreta(droppedEmoji.emotion);
            }
        }
    }
}