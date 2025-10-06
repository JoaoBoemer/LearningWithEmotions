using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableEmoji : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Vector3 initialPosition;
    
    // Identifica qual emoção é este emoji (ex: "Raiva", "Felicidade")
    public string emotionTag; 
    
    // Usado para garantir que ele volte se não for solto corretamente
    public bool isSnapped = false; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // 1. O que acontece ao começar a arrastar
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Armazena a posição inicial
        initialPosition = transform.position;
        // Torna o emoji "transparente" para que o raycast encontre a área de soltura (Drop Zone)
        GetComponent<CanvasGroup>().blocksRaycasts = false; 
    }

    // 2. O que acontece enquanto arrasta
    public void OnDrag(PointerEventData eventData)
    {
        // Move o objeto junto com o cursor, usando a câmera do Canvas
        rectTransform.anchoredPosition += eventData.delta / rectTransform.lossyScale.x;
    }

    // 3. O que acontece ao soltar
    public void OnEndDrag(PointerEventData eventData)
    {
        // Torna o emoji visível para raycasts novamente
        GetComponent<CanvasGroup>().blocksRaycasts = true; 
        
        // Se a área de soltura (Drop Zone) não o aceitou, ele volta para a posição inicial
        if (!isSnapped)
        {
            transform.position = initialPosition;
        }
    }
}