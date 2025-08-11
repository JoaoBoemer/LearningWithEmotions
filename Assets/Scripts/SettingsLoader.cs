using UnityEngine;

public class SettingsLoader : MonoBehaviour
{
    void Awake()
    {
        GameSettings.Carregar();
    }
}
