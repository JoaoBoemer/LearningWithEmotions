using UnityEngine;
using UnityEngine.EventSystems;

public class EmojiDropZone : MonoBehaviour, IDropHandler
{
    // A emoção que este retângulo ACEITA (deve corresponder ao emotionTag do Emoji)
    public string requiredEmotionTag;
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
            if (droppedEmoji.emotionTag == requiredEmotionTag)
            {
                // **SUCESSO:** O Emoji está correto!

                // 1. Define que o Emoji foi "encaixado"
                droppedEmoji.isSnapped = true;

                // 2. Faz o Emoji encaixar perfeitamente no centro do retângulo
                droppedEmoji.transform.SetParent(transform, true); // Torna o retângulo o pai do Emoji
                droppedEmoji.rectTransform.anchoredPosition = Vector2.zero; // Centraliza o Emoji

                // 3. Desativa o script de arrastar para que não possa ser movido novamente
                droppedEmoji.enabled = false;

                fase2Controller.RespostaCorreta();

                Debug.Log($"Sucesso! O Emoji {droppedEmoji.emotionTag} foi para o lugar correto.");

                // *** Aqui você adicionaria a lógica de JOGO (ex: marcar fase como correta) ***
            }
            else
            {
                // **FALHA:** O Emoji será devolvido pelo OnEndDrag do script DraggableEmoji
                Debug.Log("Erro! Emoji incorreto.");
            }
        }
    }
}