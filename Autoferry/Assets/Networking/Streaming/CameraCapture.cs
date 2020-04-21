using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using System;

public class CameraCapture : MonoBehaviour
{

    int frameCounter = 0;
    //public CameradataService cameradataService = new CameradataService();
    //CameradataImpl tempCameraDataImpl = new CameradataImpl(ByteString.CopyFromUtf8(""), 0);

    NativeArray<byte> imageBytes = new Unity.Collections.NativeArray<byte>();

    void OnEnable()
    {
        /*
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref imageBytes, AtomicSafetyHandle.Create());
#endif
        RenderPipelineManager.endFrameRendering += EndFrameRendering;
        */
        RenderPipelineManager.endFrameRendering += EndFrameRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.endFrameRendering -= EndFrameRendering;
    }

     void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera cam in cameras)
        {
            if (!cam.name.Equals("Fly_optical_camera"))
                AsyncGPUReadback.Request(cam.activeTexture, 0, TextureFormat.RGB24, ReadbackCompleted);
        }


        /*
        unsafe
        {
            foreach (Camera cam in cameras)
            {
                if (!cam.name.Equals("Fly_camera"))
                    AsyncGPUReadback.RequestIntoNativeArray(ref imageBytes, cam.activeTexture, 0, TextureFormat.RGB24, ReadbackCompleted);
                    Debug.Log("imageBytes: " + imageBytes.Length);
            }

        }
        */
    }

    // Function for testing saving of images.
    void ReadbackCompleted(AsyncGPUReadbackRequest request)
    {

        var bytes = request.GetData<byte>();

        /*
        unsafe
        {
            var bytesptr = (byte*)NativeArrayUnsafeUtility.GetUnsafeReadOnlyPtr<byte>(imageBytes);
        }
        */
        

        /*
        var tex = new Texture2D(800, 400, TextureFormat.RGBA32, false);
        tex.SetPixels32(request.GetData<Color32>().ToArray());
        tex.Apply();
        File.WriteAllBytes(request.ToString() +".png", ImageConversion.EncodeToPNG(tex));
        */
#if UNITY_EDITOR
        //DestroyImmediate(tex);
#else
        //Destroy(tex);
#endif

    }
}

