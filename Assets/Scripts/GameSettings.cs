using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    public static List<TipoEmocao> emocoesDesativadas = new List<TipoEmocao>();

    public static void Salvar()
    {
        string dados = string.Join(",", emocoesDesativadas.ConvertAll(e => ((int)e).ToString()));
        PlayerPrefs.SetString("EmocoesDesativadas", dados);
        PlayerPrefs.Save();
    }

    public static void Carregar()
    {
        emocoesDesativadas.Clear();
        string dados = PlayerPrefs.GetString("EmocoesDesativadas", "");
        if (!string.IsNullOrEmpty(dados))
        {
            string[] partes = dados.Split(',');
            foreach (string parte in partes)
            {
                if (int.TryParse(parte, out int valor))
                {
                    emocoesDesativadas.Add((TipoEmocao)valor);
                }
            }
        }
    }

    public static bool EmocaoDesativada(TipoEmocao emocao)
    {
        return emocoesDesativadas.Contains(emocao);
    }

    public static void AdicionarEmocaoDesativada(TipoEmocao emocao)
    {
        if (!emocoesDesativadas.Contains(emocao))
        {
            emocoesDesativadas.Add(emocao);
        }
    }

    public static void RemoverEmocaoDesativada(TipoEmocao emocao)
    {
        if (emocoesDesativadas.Contains(emocao))
        {
            emocoesDesativadas.Remove(emocao);
        }
    }
}
