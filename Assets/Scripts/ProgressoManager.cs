using UnityEngine;
using UnityEngine.UI; // Importante para trabalhar com UI Images

public class StarManager : MonoBehaviour
{
    public Image[] starImages; // Array para guardar as referências a todas as suas Image de estrela
    public Sprite filledStarSprite; // Sprite da estrela preenchida
    public Sprite emptyStarSprite;  // Sprite da estrela vazia

    private int currentStars = 0; // Quantas estrelas o jogador já tem (começa em 0)

    // Método chamado quando a fase começa, para resetar as estrelas
    public void ResetStars()
    {
        currentStars = 0;
        UpdateStarDisplay();
    }

    // Método para adicionar uma estrela (chamado a cada acerto)
    public void AddStar()
    {
        if (currentStars < starImages.Length) // Garante que não adicionamos mais estrelas do que temos Image disponíveis
        {
            currentStars++; // Incrementa o contador de estrelas
            UpdateStarDisplay(); // Atualiza a exibição visual
        }
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

    // Exemplo de como você chamaria isso de outro script (seu Game Manager, por exemplo)
    /*
    // Em algum outro script (ex: GameController.cs)
    public StarManager starManager; // Referência ao seu StarManager

    void Start()
    {
        // Encontra o StarManager na cena (ou o atribui via Inspector)
        starManager = FindObjectOfType<StarManager>(); 
        starManager.ResetStars(); // Reseta as estrelas no início da fase
    }

    // Quando o jogador acerta uma pergunta:
    public void PlayerAnsweredCorrectly()
    {
        starManager.AddStar(); // Adiciona uma estrela
        // ... Lógica para ir para a próxima pergunta ...
    }
    */
}