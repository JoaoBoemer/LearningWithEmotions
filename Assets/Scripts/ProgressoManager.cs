using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Importante para trabalhar com UI Images

public class StarManager : MonoBehaviour
{
    public Image[] starImages; // Array para guardar as referências a todas as suas Image de estrela
    public Sprite filledStarSprite; // Sprite da estrela preenchida
    public Sprite emptyStarSprite;  // Sprite da estrela vazia

    private int currentStars = 0; // Quantas estrelas o jogador já tem (começa em 0)

    public Image backgroundImage;
    private Color originalColor;

    [Header("Configurações de Feedback")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failureColor = Color.red;
    [SerializeField] private float fadeDuration = 0.5f; // Tempo para voltar ao normal

    private void Start()
    {
        if (backgroundImage != null)
        {
            originalColor = backgroundImage.color;
        }
    }

    // Método chamado quando a fase começa, para resetar as estrelas
    public void ResetStars()
    {
        currentStars = 0;
        UpdateStarDisplay();
    }

    // Método para adicionar uma estrela (chamado a cada acerto)
    public void AddStar()
    {
        StartColorFeedback(successColor);
        if (currentStars < starImages.Length) // Garante que não adicionamos mais estrelas do que temos Image disponíveis
        {
            currentStars++; // Incrementa o contador de estrelas
            UpdateStarDisplay(); // Atualiza a exibição visual
        }
    }

    public void IncorrectFeedback()
    {
        StartColorFeedback(failureColor);
    }

    // Método para atualizar a exibição das estrelas com base em 'currentStars'
    private void UpdateStarDisplay()
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < currentStars)
            {
                // Se a estrela atual deve estar preenchida
                starImages[i].sprite = filledStarSprite;
            }
            else
            {
                // Se a estrela atual deve estar vazia
                starImages[i].sprite = emptyStarSprite;
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
        backgroundImage.color = targetColor;

        // 2. Esperar um frame para garantir a mudança visual
        yield return null; 

        // 3. Fazer a cor retornar gradualmente para a cor original
        float time = 0;
        Color startColor = backgroundImage.color;
        
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;
            
            // Suaviza a transição (opcional: Use 't * t' para aceleração)
            backgroundImage.color = Color.Lerp(startColor, originalColor, t);
            
            yield return null;
        }

        // 4. Garante que a cor final seja exatamente a cor original
        backgroundImage.color = originalColor;
    }
}