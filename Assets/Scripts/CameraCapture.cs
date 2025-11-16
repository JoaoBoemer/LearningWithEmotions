using UnityEngine;
using UnityEngine.UI;

public class CameraCapture : MonoBehaviour
{
    public RawImage display;
    private WebCamTexture webcamTexture;
    public Texture2D imgTeste;

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

        // Debug.Log("Usando a camera: " + webcamTexture.deviceName);

        webcamTexture.Play();
        display.texture = webcamTexture;
    }

    public string GetBase64()
    {
        Texture source = display.texture;
        RenderTexture rt = RenderTexture.GetTemporary(source.width, source.height, 0, RenderTextureFormat.Default);

        Graphics.Blit(source, rt);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D novaTextura = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);

        novaTextura.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        novaTextura.Apply();

        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt);

        byte[] imgBytes = novaTextura.EncodeToJPG();
        string b64Image = System.Convert.ToBase64String(imgBytes);

        // Debug.Log(b64Image);

        return b64Image;
        
        // Texture2D tempTexture = new Texture2D(webcamTexture.width, webcamTexture.height);
        // tempTexture.SetPixels(webcamTexture.GetPixels());
        // tempTexture.Apply();

        // byte[] imageBytes = tempTexture.EncodeToJPG();
        // string base64Image = System.Convert.ToBase64String(imageBytes);

        // return base64Image;
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
        if (webcamTexture != null
        //  && webcamTexture.isPlaying
         )
        {
            webcamTexture.Stop();
            webcamTexture = null;
            display.texture = null;
        }
    }
}
