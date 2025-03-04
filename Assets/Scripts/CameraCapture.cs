using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    public RawImage display;
    private WebCamTexture webcamTexture;
    public static CameraCapture instance; // Criar uma instância global para acesso
    // private bool camAvaiable;
    
    void Awake()
    {
        instance = this; // Permite que outros scripts acessem
    }

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if(devices.Length == 0)
        {
            Debug.Log("No camera detected.");
            // camAvaiable = false;
            return;
        }

        for(int i = 0; i < devices.Length; i++)
        {
            if(devices[i].isFrontFacing)
            {
                webcamTexture = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if(webcamTexture == null)
        {
            Debug.Log("Unable to found front camera");
            return;
        }

        webcamTexture.Play();
        display.texture = webcamTexture;
    }

    public WebCamTexture GetCameraTexture()
    {
        return webcamTexture;
    }
}
