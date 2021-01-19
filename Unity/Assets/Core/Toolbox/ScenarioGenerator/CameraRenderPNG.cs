using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class CameraRenderPNG : MonoBehaviour
{
    private Camera cam;
    private Texture2D[] Images;
    private float OSPtime;
    public string cameraID = "VL_";
    public int width = 1024;
    public int frameRate = 1;
    public int scenarioSeconds = 5;
    public int height = 1024;
    string Filepath;

    // Start is called before the first frame update
    void Start()
    {
        OSPtime = 0;
        Images = new Texture2D[frameRate* scenarioSeconds];
        cam = gameObject.GetComponent<Camera>();
        cam.fieldOfView = 90;
        RenderTexture mRt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
        mRt.antiAliasing = 8;
        cam.targetTexture = mRt;
        Filepath = Application.dataPath + "/SyntheticDataset";
    }

    // Update is called once per frame

    private float nextActionTime = 0.0f;
    public bool RunRecording = false;
    int i = 0;

    void Update()
    {
        OSPtime = Time.time;
        if (OSPtime >= nextActionTime & RunRecording)
        {
            nextActionTime = OSPtime + 1 /(float)frameRate;
            Debug.Log(OSPtime);
            Images[i] = RTImage(cam);
            i++;
        }
        if (OSPtime >= (float)scenarioSeconds || i == Images.Length)
        {
            for (int j = 0; j < i; j++) {
                name = System.DateTime.UtcNow.ToString("HH:MM-"+j.ToString());
                print(Filepath+name);
                SaveTextureAsPNG(Images[j], Filepath, cameraID+name);
            }
            RunRecording = false;
            i = 0;
        }
    }

    // Take a "screenshot" of a camera's Render Texture.
    Texture2D RTImage(Camera camera)
    {
        // The Render Texture in RenderTexture.active is the one
        // that will be read by ReadPixels.
        var currentRT = RenderTexture.active;
        RenderTexture.active = camera.targetTexture;

        // Render the camera's view.
        camera.Render();

        // Make a new texture and read the active Render Texture into it.
        Texture2D image = new Texture2D(camera.targetTexture.width, camera.targetTexture.height,TextureFormat.RGB24,false,true);
        image.ReadPixels(new Rect(0, 0, camera.targetTexture.width, camera.targetTexture.height), 0, 0);
        image.Apply();

        // Replace the original active Render Texture.
        RenderTexture.active = currentRT;
        return image;
    }

    public static void SaveTextureAsPNG(Texture2D _texture, string _fullPath,string fileName)
    {
        byte[] _bytes = _texture.EncodeToPNG();
        System.IO.File.WriteAllBytes(_fullPath+"/"+ fileName, _bytes);
        Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
    }
}
