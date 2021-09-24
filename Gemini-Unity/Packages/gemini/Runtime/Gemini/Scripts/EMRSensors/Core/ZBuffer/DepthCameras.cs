using UnityEngine;
using System.Collections;

using UnityEngine.Rendering;
using System.Linq;

namespace Gemini.EMRS.Core.ZBuffer
{
    public class DepthCameras
    {
        private string CameraTag = "DepthCam";
        private CameraFrustum frustums;
        public Camera[] cameras;
        private DepthBits DepthBufferPrecision = DepthBits.Depth24;
        private uint TotalNumberOfDepthPixels;

        public DepthCameras(int cameraNumbers, CameraFrustum cameraFrustums, Transform transform)
        {
            frustums = cameraFrustums;
            cameras = SpawnDepthCameras(cameraNumbers, transform);
        }
        public DepthCameras(int cameraNumbers, CameraFrustum cameraFrustums, Transform transform, ComputeShader shader, string kernelName, DepthBits depthBits)
        {
            frustums = cameraFrustums;
            DepthBufferPrecision = depthBits;
            cameras = SpawnDepthCameras(cameraNumbers, transform);
            SetCameraBuffers(shader, kernelName);
            TotalNumberOfDepthPixels = (uint)(frustums._pixelWidth * frustums._pixelHeight * cameraNumbers);
        }

        private Camera[] SpawnDepthCameras(int numbers, Transform transform)
        {
            RenderTextureFormat format = RenderTextureFormat.Depth;
            Camera[] Cameras = new Camera[numbers];

            var depthBuffer = new RenderTexture(frustums._pixelWidth, frustums._pixelHeight, 0, format);
            depthBuffer.useMipMap = false;
            depthBuffer.filterMode = FilterMode.Point;
            depthBuffer.dimension = TextureDimension.Tex2DArray;
            depthBuffer.volumeDepth = numbers;
            depthBuffer.depth = (int)DepthBufferPrecision;
            if (!(DepthBufferPrecision == DepthBits.Depth16
                || DepthBufferPrecision == DepthBits.Depth24
                || DepthBufferPrecision == DepthBits.Depth32))
            {
                throw new System.ArgumentException(System.String.Format("{0} is not a valid depth buffer size.",
                    DepthBufferPrecision), "DepthBufferPrecision");
            }

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
            }
            shader.SetTexture(kernelHandle, "depthImages", cameras[0].targetTexture);
            shader.SetMatrixArray("CameraRotationMatrices", RotationMatrices);
        }
    }
}