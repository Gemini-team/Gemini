using UnityEngine;
using UnityEngine.Rendering;
using Gemini.EMRS.Core;
using Gemini.EMRS.Core.ZBuffer;
using Gemini.Networking.Clients;
using Google.Protobuf;
using UnityEngine.Experimental.Rendering;
using System.IO;

namespace Gemini.EMRS.RGB
{
    public struct CameraImage {

        public CameraImage(float time, string frameID, uint height, uint width)
        {
            data = ByteString.CopyFromUtf8("");
            this.time = time;
            this.frameID = frameID;
            this.width = width;
            this.height = height;
        }

        public ByteString data;
        public float time;
        public string frameID;
        public uint height;
        public uint width;
    }

    [RequireComponent(typeof(Camera))]
    public class RGBCamera : SensorNew<CameraImage>
    {
        public RenderTexture _cameraBuffer { get; set; }
        public RenderTexture SampleCameraImage;
        public ComputeShader cameraShader;
        public string FrameID = "F";
        public int ImageCrop = 4;
        public bool SynchronousUpdate = false;

        private bool _hasRenderedWhenUpdated = false;

        public bool HasRenderedWhenUpdated
        {
            get => _hasRenderedWhenUpdated;
            set => _hasRenderedWhenUpdated = value;
        }

        private Camera _camera;
        private UnifiedArray<byte> _cameraData;
        private RenderTextureFormat _renderTextureFormat = RenderTextureFormat.Default;
        private TextureFormat _textureFormat = TextureFormat.RGB24;

        private CameraClient client;

        private float time = 0f;

        [Space]
        [Header("Camera Parameters")]
        public int PixelWidth = 2448;
        public int PixelHeight = 2048;
        public float FarPlane = 10000f;
        public float NearPlane = 0.08f;
        public float focalLengthMilliMeters = 5.5f;
        public float pixelSizeInMicroMeters = 3.45f;
        public DepthBits DepthBufferPrecision = DepthBits.Depth24;
        public ByteString Data { get; private set; } = ByteString.CopyFromUtf8("");


        private void Awake()
        {
            SetupSensorCallbacks(new SensorCallback(RGBUpdate, SensorCallbackOrder.Last)); 
        }

        private void Start()
        {
            _client = new CameraClient();
            _sensorData = new CameraImage(0f, FrameID, (uint)(PixelHeight / ImageCrop), (uint)(PixelWidth / ImageCrop));

            CameraSetup();

            int kernelIndex = cameraShader.FindKernel("CSMain");
            _cameraData = new UnifiedArray<byte>(PixelHeight * PixelWidth, sizeof(float) * 3, "CameraData");
            _cameraData.SetBuffer(cameraShader, "CSMain");
            cameraShader.SetTexture(kernelIndex, "RenderTexture", _camera.targetTexture);
            cameraShader.SetInt("Width", PixelWidth / ImageCrop);
            cameraShader.SetInt("Height", PixelHeight / ImageCrop);
        }

        int saveCount = 0;

        private void RGBUpdate(ScriptableRenderContext context, Camera[] cameras)
        {
            if (SynchronousUpdate)
            {
                _cameraData.SynchUpdate(cameraShader, "CSMain");
                _sensorData.data = ByteString.CopyFrom(_cameraData.array);

                var imgArr = ImageConversion.EncodeArrayToPNG(_cameraData.array, GraphicsFormat.R8G8B8A8_SRGB, (uint)PixelWidth, (uint)PixelHeight);
                
                if (saveCount == 0)
                {
                    File.WriteAllBytes("cam_img.png", imgArr);
                    saveCount++;
                }

                //_client.SendMessage(_sensorData);
            }
            else
            {
                AsyncGPUReadback.Request(_camera.activeTexture, 0, _textureFormat, ReadbackCompleted);
            }
        }

        private void ReadbackCompleted(AsyncGPUReadbackRequest request)
        {
            _sensorData.data = ByteString.CopyFrom(request.GetData<byte>().ToArray());

            if (saveCount == 0)
            {
                //var imgArr = ImageConversion.EncodeArrayToPNG(_sensorData.data.ToByteArray(), GraphicsFormat.R8G8B8A8_SRGB, (uint)(PixelWidth / ImageCrop), (uint)(PixelHeight / ImageCrop));
                //File.WriteAllBytes("cam_img_srgb_async.png", imgArr);
                //saveCount++;

                //var imgArr = ImageConversion.EncodeArrayToPNG(request.GetData<byte>().ToArray(), GraphicsFormat.R8G8B8A8_UNorm, (uint)(PixelWidth / ImageCrop), (uint)(PixelHeight / ImageCrop));
                //var imgArr = ImageConversion.EncodeArrayToPNG(request.GetData<byte>().ToArray(), GraphicsFormat.R8G8B8A8_UNorm, (uint)PixelWidth, (uint)PixelHeight);
                Debug.Log("width: " + request.width + ", height: " + request.height);

                var imgArr = ImageConversion.EncodeArrayToPNG(
                    request.GetData<byte>().ToArray(), 
                    //GraphicsFormat.R8G8B8A8_UNorm, 
                    GraphicsFormat.R8G8B8_UNorm, 
                    (uint)(request.width), 
                    (uint)(request.height));

                Debug.Log("Data length: " + request.GetData<byte>().ToArray().Length);

                File.WriteAllBytes("cam_img_unorm_async.png", imgArr);
                saveCount++;

            }

            _client.SendMessage(_sensorData);

        }

        private byte[] RenderTextureToBinary(Camera cam)
        {
            // The Render Texture in RenderTexture.active is the one
            // that will be read by ReadPixels.
            var currentRT = RenderTexture.active;
            RenderTexture.active = cam.targetTexture;

            // Make a new texture and read the active Render Texture into it.
            Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, _textureFormat, false, true);
            image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
            image.Apply();

            // Replace the original active Render Texture.
            RenderTexture.active = currentRT;
            return image.EncodeToPNG();
        }

        private void CameraSetup()
        {
            CameraFrustum frustums = new CameraFrustum(PixelWidth, PixelHeight, FarPlane, NearPlane, focalLengthMilliMeters, pixelSizeInMicroMeters);
            _cameraBuffer = new RenderTexture(PixelWidth / ImageCrop, PixelHeight / ImageCrop, (int)DepthBufferPrecision, _renderTextureFormat, 0);

            _camera = gameObject.GetComponent<Camera>();
            _camera.usePhysicalProperties = false;
            _camera.targetTexture = _cameraBuffer;

            _camera.aspect = frustums._aspectRatio;//Mathf.Tan(Mathf.PI / numbers) / Mathf.Tan(frustums._verticalAngle / 2.0f);
            Debug.Log("Aspect Ratio RGB: " + frustums._aspectRatio.ToString());
            _camera.fieldOfView = frustums._verticalAngle * Mathf.Rad2Deg;//Camera.HorizontalToVerticalFieldOfView(360.0f / numbers, cam.aspect);
            _camera.farClipPlane = frustums._farPlane;
            _camera.nearClipPlane = frustums._nearPlane;
            //camera.enabled = false;
        }

        public static RGBCamera[] GetActiveCameras()
        {
            return GameObject.FindObjectsOfType<RGBCamera>();
        }
    }
}
