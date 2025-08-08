using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfiguracaoEmocoes : MonoBehaviour
{
    public Button[] botoesEmocoes; // Arraste os botões na ordem das emoções no Inspector
    public Color corAtiva = Color.white;
    public Color corDesativada = Color.gray;

    private HashSet<TipoEmocao> emocoesDesativadas = new HashSet<TipoEmocao>();

    private void Start()
    {
        // Garante que todos começam ativos
        for (int i = 0; i < botoesEmocoes.Length; i++)
        {
            int index = i;
            botoesEmocoes[i].onClick.AddListener(() => AlternarEmocao((TipoEmocao)index));
            AtualizarCorBotao((TipoEmocao)index);
        }
    }

    public void AlternarEmocao(TipoEmocao emocao)
    {
        if (emocoesDesativadas.Contains(emocao))
            emocoesDesativadas.Remove(emocao); // Reativar
        else
            emocoesDesativadas.Add(emocao); // Desativar

        AtualizarCorBotao(emocao);
        SalvarConfiguracao();
    }

    private void AtualizarCorBotao(TipoEmocao emocao)
    {
        int index = (int)emocao;
        var colors = botoesEmocoes[index].colors;
        colors.normalColor = emocoesDesativadas.Contains(emocao) ? corDesativada : corAtiva;
        botoesEmocoes[index].colors = colors;
    }

    private void SalvarConfiguracao()
    {
        // Salva como string no PlayerPrefs (ex: "Alegria,Tristeza")
        string[] desativadasArray = new string[emocoesDesativadas.Count];
        int i = 0;
        foreach (var emocao in emocoesDesativadas)
        {
            desativadasArray[i] = emocao.ToString();
            i++;
        }
        PlayerPrefs.SetString("EmocoesDesativadas", string.Join(",", desativadasArray));
        PlayerPrefs.Save();
    }

    public static HashSet<TipoEmocao> CarregarConfiguracao()
    {
        var desativadas = new HashSet<TipoEmocao>();
        string data = PlayerPrefs.GetString("EmocoesDesativadas", "");
        if (!string.IsNullOrEmpty(data))
        {
            string[] nomes = data.Split(',');
            foreach (string nome in nomes)
            {
                if (System.Enum.TryParse(nome, out TipoEmocao emocao))
                    desativadas.Add(emocao);
            }
        }
        return desativadas;
    }
}
