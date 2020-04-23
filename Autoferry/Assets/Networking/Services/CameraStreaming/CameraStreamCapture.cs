﻿using UnityEngine;
using UnityEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Google.Protobuf;
public class CameraStreamCapture : MonoBehaviour
{
    int frameCounter = 0;
    public CameradataServer cameradataService = new CameradataServer();
    CameradataImpl tempCameraDataImpl = new CameradataImpl(ByteString.CopyFromUtf8(""), 0);
    int id = 0;
    Unity.Collections.NativeArray<byte> imageBytes = new Unity.Collections.NativeArray<byte>();
    
    void OnEnable()
    {
        RenderPipelineManager.endFrameRendering += EndFrameRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.endFrameRendering -= EndFrameRendering;
    }

    // Start is called before the first frame update
    void Start()
    {
        id = CameraServiceGen.GenID();
    }

    /*
    void OnPostRender()
    {
        frameCounter++;
        if (frameCounter % 2 == 0)
        {
            ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
            AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, ReadbackCompleted);
            renderTexture.Release();
        }

        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);
        AsyncGPUReadback.Request(renderTexture, 0, TextureFormat.RGB24, ReadbackCompleted);
        renderTexture.Release();
    }
    */

    void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {
        foreach (Camera cam in cameras)
        {
            if (!cam.name.Equals("Fly_optical_camera"))
                AsyncGPUReadback.Request(cam.activeTexture, 0, TextureFormat.RGB24, ReadbackCompleted);
        }
    }

    void ReadbackCompleted(AsyncGPUReadbackRequest request)
    {
         
        tempCameraDataImpl.SetImagedata(ByteString.CopyFrom(request.GetData<byte>().ToArray()));

        // This should be no problem since the length of the imagedata should always be > 0.
        //tempCameraDataImpl.SetImagedataLength((uint)tempCameraDataImpl.GetImagedata().Length);

        cameradataService.SetCameradata(id, tempCameraDataImpl);
    }
}
