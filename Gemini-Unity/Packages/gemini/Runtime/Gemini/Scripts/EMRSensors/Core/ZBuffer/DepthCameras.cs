﻿using UnityEngine;
using System.Collections;

using UnityEngine.Rendering;

namespace Gemini.EMRS.Core.ZBuffer
{
    public class DepthCameras
    {
        public enum BufferPrecision // your custom enumeration
        {
            bit16,
            bit24,
            bit32
        };

        private string CameraTag = "DepthCam";
        private CameraFrustum frustums;
        public Camera[] cameras;

        public Texture2DArray depthTextures;

        public DepthCameras(int cameraNumbers, CameraFrustum cameraFrustums, Transform transform)
        {
            frustums = cameraFrustums;
            cameras = SpawnDepthCameras(cameraNumbers, transform);
        }
        public DepthCameras(int cameraNumbers, CameraFrustum cameraFrustums, Transform transform, ComputeShader shader, string kernelName)
        {
            frustums = cameraFrustums;
            cameras = SpawnDepthCameras(cameraNumbers, transform);
            SetCameraBuffers(shader, kernelName);
        }

        private BufferPrecision DepthBufferPrecision = BufferPrecision.bit24;

        private Camera[] SpawnDepthCameras(int numbers, Transform transform)
        {
            RenderTextureFormat format = RenderTextureFormat.Depth;
            Camera[] Cameras = new Camera[numbers];

            // just lets the camera know that it is supposed to render into a textureArray
            var depthBuffer = new RenderTexture(frustums._pixelWidth, frustums._pixelHeight, 0, format);
            depthBuffer.useMipMap = false;
            depthBuffer.filterMode = FilterMode.Point;
            // // depthBuffer.generateMips = false;
            depthBuffer.dimension = TextureDimension.Tex2DArray;
            depthBuffer.volumeDepth = 4;

            depthTextures = new Texture2DArray(frustums._pixelWidth, frustums._pixelHeight, 4, TextureFormat.RGBA32, false, true);
            depthTextures.filterMode = FilterMode.Point;
            // depthTextures.useMipMap = false;
            // depthTextures.anisoLevel = 0;

            for (int i = 0; i < numbers; i++)
            {
                GameObject CameraObject = new GameObject();
                CameraObject.name = CameraTag + i;
                CameraObject.transform.SetParent(transform);
                CameraObject.transform.localRotation = Quaternion.Euler(0, i * 360.0f / numbers, 0);
                CameraObject.transform.localPosition = new Vector3(0, 0, 0);
                //CameraObject.layer = LayerMask.NameToLayer(LidarLayer);
                CameraObject.AddComponent<Camera>();
                Camera cam = CameraObject.GetComponent<Camera>();

                //var depthBuffer = new RenderTexture(frustums._pixelWidth, frustums._pixelHeight, 16, format);//,// format);

                if (cam.targetTexture == null)
                {
                    if (DepthBufferPrecision == BufferPrecision.bit16)
                    {
                        depthBuffer.depth = 16;
                    }
                    else if (DepthBufferPrecision == BufferPrecision.bit24)
                    {
                        depthBuffer.depth = 24;

                    }
                    else if (DepthBufferPrecision == BufferPrecision.bit32)
                    {
                        depthBuffer.depth = 32;
                    }
                }


                cam.targetTexture = depthBuffer;

                cam.usePhysicalProperties = false;

                // Projection Matrix Setup
                cam.aspect = frustums._aspectRatio;//Mathf.Tan(Mathf.PI / numbers) / Mathf.Tan(frustums._verticalAngle / 2.0f);
                cam.fieldOfView = frustums._verticalAngle * Mathf.Rad2Deg;//Camera.HorizontalToVerticalFieldOfView(360.0f / numbers, cam.aspect);
                cam.farClipPlane = frustums._farPlane;
                cam.enabled = false;
                cam.nearClipPlane = frustums._nearPlane;
                Cameras[i] = cam;
                //Debug.Log(cam.projectionMatrix);
            }
            return Cameras;
        }

        public void SetCameraBuffers(ComputeShader shader, string kernelName)
        {
            int kernelHandle = shader.FindKernel(kernelName);
            shader.SetMatrix("inv_CameraMatrix", cameras[0].projectionMatrix.inverse);
            shader.SetMatrix("CameraMatrix", cameras[0].projectionMatrix);
            shader.SetInt("ImageWidthRes", frustums._pixelWidth);
            shader.SetInt("ImageHeightRes", frustums._pixelHeight);
            shader.SetFloat("VFOV_camera", frustums._verticalAngle);
            shader.SetFloat("HFOV_camera", frustums._horisontalAngle);
            shader.SetInt("NrOfImages", cameras.Length);
            Matrix4x4[] RotationMatrices = new Matrix4x4[cameras.Length];
            for (int i = 0; i < cameras.Length; i++)
            {
                Quaternion angle = Quaternion.Euler(0, i * 360.0f / cameras.Length, 0);
                RotationMatrices[i] = Matrix4x4.Rotate(angle);

                //Debug.Log("Camera depth texture set for: " + i.ToString());

                // shader.SetTexture(kernelHandle, "depthImage" + i.ToString(), cameras[i].targetTexture);


                
            }
            // shader.SetTexture(kernelHandle, "DepthImages", cameras[0].targetTexture);
            // this constitutes a problem, as every camera wants to render into this one
            shader.SetTexture(kernelHandle, "depthImages", depthTextures);
            // or try shader.SetTexture(kernelHandle, "depthImages", depthTextures, 0, RenderTextureSubElement.Depth);
            shader.SetMatrixArray("CameraRotationMatrices", RotationMatrices);
        }

    }
}