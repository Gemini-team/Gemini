using UnityEngine;
using System.Collections;
using Gemini.EMRS.Core;
using Gemini.EMRS.Core.ZBuffer;

using UnityEngine.Rendering;

using Google.Protobuf;
using Grpc.Core;
using Sensorstreaming;

namespace Gemini.EMRS.RGB
{
    [RequireComponent(typeof(Camera))]
    public class RGBScript : Sensor
    {
        public RenderTexture _cameraBuffer { get; set; }
        public RenderTexture SampleCameraImage;
        public ComputeShader cameraShader;
        public string FrameID = "F";
        public int ImageCrop = 4;
        public bool SynchronousUpdate = false;

        private Camera camera;
        private UnifiedArray<byte> cameraData;
        private RenderTextureFormat renderTextureFormat = RenderTextureFormat.Default;
        private TextureFormat textureFormat = TextureFormat.RGB24;


        [Space]
        [Header("Camera Parameters")]
        public int PixelWidth = 2448;
        public int PixelHeight = 2048;
        public float FarPlane = 10000f;
        public float NearPlane = 0.08f;
        public float focalLengthMilliMeters = 5.5f;
        public float pixelSizeInMicroMeters = 3.45f;
        public DepthBits DepthBufferPrecision = DepthBits.Depth24;

        void Start()
        {
            CameraSetup();

            int kernelIndex = cameraShader.FindKernel("CSMain");
            cameraData = new UnifiedArray<byte>(PixelHeight * PixelWidth, sizeof(float) * 3, "CameraData");
            cameraData.SetBuffer(cameraShader, "CSMain");
            cameraShader.SetTexture(kernelIndex, "RenderTexture", camera.targetTexture);
            cameraShader.SetInt("Width", PixelWidth / ImageCrop);
            cameraShader.SetInt("Height", PixelHeight / ImageCrop);
        }

        private void Awake()
        {
            SetupSensorCallbacks(new SensorCallback(RGBUpdate, SensorCallbackOrder.Last)); 
        }

        public ByteString Data { get; private set; } = ByteString.CopyFromUtf8("");
        public override bool SendMessage()
        {
            //Debug.Log("RGB " + FrameID + " message time: " + OSPtime);
            if(SampleCameraImage != null)
            {
                Graphics.Blit(camera.targetTexture, SampleCameraImage);
            }

            // TODO: Remove

            bool success = false;
            connectionTime = Time.time;
             
            if(connectionTime < ConnectionTimeout || connected)
            {
                try
                {
                    success = _streamingClient.StreamCameraSensor(new CameraStreamingRequest { Data = Data, TimeStamp = OSPtime, FrameId = FrameID, Height = (uint)(PixelHeight/ImageCrop), Width = (uint)(PixelWidth/ImageCrop) }).Success;
                    connected = success;
                } catch (RpcException e)
                {
                    Debug.LogException(e);
                }
            }

            return success;
            
        }

        void RGBUpdate(ScriptableRenderContext context, Camera[] cameras)
        {
            if (SynchronousUpdate)
            {
                cameraData.SynchUpdate(cameraShader, "CSMain");
                Data = ByteString.CopyFrom(cameraData.array);
                gate = true;
            }
            else
            {
                AsyncGPUReadback.Request(camera.activeTexture, 0, textureFormat, ReadbackCompleted);
            }
        }

        void ReadbackCompleted(AsyncGPUReadbackRequest request)
        {
            Data = ByteString.CopyFrom(request.GetData<byte>().ToArray());
            gate = true;
        }

        byte[] RenderTextureToBinary(Camera cam)
        {
            // The Render Texture in RenderTexture.active is the one
            // that will be read by ReadPixels.
            var currentRT = RenderTexture.active;
            RenderTexture.active = cam.targetTexture;

            // Make a new texture and read the active Render Texture into it.
            Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, textureFormat, false, true);
            image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            image.Apply();

            // Replace the original active Render Texture.
            RenderTexture.active = currentRT;
            return image.EncodeToPNG();
        }

        private void CameraSetup()
        {
            CameraFrustum frustums = new CameraFrustum(PixelWidth, PixelHeight, FarPlane, NearPlane, focalLengthMilliMeters, pixelSizeInMicroMeters);
            _cameraBuffer = new RenderTexture(PixelWidth / ImageCrop, PixelHeight / ImageCrop, (int)DepthBufferPrecision, renderTextureFormat, 0);

            camera = gameObject.GetComponent<Camera>();
            camera.usePhysicalProperties = false;
            camera.targetTexture = _cameraBuffer;

            camera.aspect = frustums._aspectRatio;//Mathf.Tan(Mathf.PI / numbers) / Mathf.Tan(frustums._verticalAngle / 2.0f);
            Debug.Log("Aspect Ratio RGB: " + frustums._aspectRatio.ToString());
            camera.fieldOfView = frustums._verticalAngle * Mathf.Rad2Deg;//Camera.HorizontalToVerticalFieldOfView(360.0f / numbers, cam.aspect);
            camera.farClipPlane = frustums._farPlane;
            camera.nearClipPlane = frustums._nearPlane;
            //camera.enabled = false;
        }

    }
}