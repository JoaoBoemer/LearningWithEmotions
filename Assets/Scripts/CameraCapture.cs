using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    public RawImage display;
    private WebCamTexture webcamTexture;
    //public static CameraCapture instance; // Criar uma instância global para acesso
    // private bool camAvaiable;

    // void Awake()
    // {
    //     if (instance == null)
    //     {
    //         instance = this; // Permite que outros scripts acessem
    //         DontDestroyOnLoad(gameObject);
    //     }
    //     else
    //     {
    //         Destroy(gameObject);
    //     }
    // }

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (display == null)
            Debug.LogWarning("O campo display não foi atribuido no inspetor");

        if (devices.Length == 0)
        {
            Debug.Log("No camera detected.");
            // camAvaiable = false;
            return;
        }

        for (int i = 0; i < devices.Length; i++)
        {
            if (devices[i].isFrontFacing)
            {
                webcamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (webcamTexture == null)
        {
            for (int i = 0; i < devices.Length; i++)
            {
                webcamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (webcamTexture == null)
        {
            Debug.Log("Não foi possível encontrar uma camera");
            return;
        }

        Debug.Log("Usando a camera: " + webcamTexture.deviceName);

        webcamTexture.Play();
        display.texture = webcamTexture;
    }

    public WebCamTexture GetCameraTexture()
    {
        return webcamTexture;
    }

    void OnDisable()
    {
        StopCamera();
    }

    void OnDestroy()
    {
        StopCamera();
    }

    public void StopCamera()
    {
        Debug.Log("Try Harder");
        if (webcamTexture != null
        //  && webcamTexture.isPlaying
         )
        {
            webcamTexture.Stop();
            webcamTexture = null;
            display.texture = null;
            Debug.Log("Camera parada");
        }
    }
}
