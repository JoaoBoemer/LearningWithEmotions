using System.Collections.Generic;
using UnityEngine;

public class ImagensFaseManager : MonoBehaviour
{
    private Dictionary<TipoEmocao, List<Sprite>> imagensPorEmocao = new Dictionary<TipoEmocao, List<Sprite>>();
    public string pastaFase; // Ex.: "Fase1"

    void Start()
    {
        GameSettings.Carregar(); // garante que o estado das emoções esteja carregado
        CarregarImagens();
    }

    void CarregarImagens()
    {
        string caminhoPasta = "";
        foreach (TipoEmocao emocao in System.Enum.GetValues(typeof(TipoEmocao)))
        {
            // Se a emoção estiver desativada, pula
            if (GameSettings.EmocaoDesativada(emocao))
                continue;

            if (pastaFase == "Fase1")
                caminhoPasta = $"{pastaFase}/Perguntas/{emocao}";
            else
                caminhoPasta = $"{pastaFase}/{emocao}";

            Sprite[] sprites = Resources.LoadAll<Sprite>(caminhoPasta);
            imagensPorEmocao[emocao] = new List<Sprite>(sprites);
        }
    }

    public Sprite ObterImagemAleatoria(TipoEmocao emocao)
    {
        // Se a emoção não foi carregada, retorna null
        if (!imagensPorEmocao.ContainsKey(emocao))
            return null;

        List<Sprite> lista = imagensPorEmocao[emocao];

        if (lista.Count == 0)
            return null; // sem imagens

        int index = Random.Range(0, lista.Count);
        Sprite imagemEscolhida = lista[index];
        lista.RemoveAt(index); // remove para não repetir
        return imagemEscolhida;
    }
}
