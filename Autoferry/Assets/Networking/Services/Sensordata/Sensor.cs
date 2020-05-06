using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Networking.Services
{

    public enum SensorType
    {
        Unknown,
        Optical,
        Infrared, 
        Radar,
        Lidar
    }


    public class Sensor : MonoBehaviour
    {


        public SensorType type;

        public string host = "localhost";

        private int _port = ServicePortGenerator.GenPort();

        public int Port
        {
            get => _port;
        }

        private static int _id = SensorIDGenerator.GenID();

        public int ID
        {
            get => _id;
        }

        private RenderTexture _renderTexture;

        public RenderTexture RenderTexture
        {
            get => _renderTexture;
            set => _renderTexture = value;
        }
        
        private readonly ByteString _data = ByteString.CopyFromUtf8("");

        public ByteString Data
        {
            get => _data;
        }

        private readonly int _dataLength = 0;
        
        public int DataLength
        {
            get => _dataLength;
        }

        public bool RenderFlag { get; set; }


        private Camera camera;
        private Server server;
        private SensordataServiceImpl serviceImpl;

        public Sensor()
        {
            RenderFlag = true;
        }


        void Start()
        {
            
            if (gameObject.GetComponent<Camera>() != null)
            {
                camera = gameObject.GetComponent<Camera>();
                _renderTexture = camera.targetTexture;
            }

            if ( _renderTexture == null)
            {
                _renderTexture = new RenderTexture(800, 640, 24, RenderTextureFormat.Default, 0);
                camera.targetTexture = _renderTexture;
            } 

            serviceImpl = new SensordataServiceImpl(_data, _dataLength);

            server = new Server
            {
                Services = { Sensordata.Sensordata.BindService(serviceImpl) },
                Ports = { new ServerPort(host, _port, ServerCredentials.Insecure) }
            };

            Debug.Log("Sensordata server listening on port: " + _port);
            server.Start();
           
        }

        public void OnEnable()
        {
            RenderPipelineManager.endFrameRendering += EndFrameRendering;
        }

        public void OnDisable()
        {
            RenderPipelineManager.endFrameRendering -= EndFrameRendering;
        }



        void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
        {

            if ( RenderFlag )
            {
                if (type == SensorType.Optical || type == SensorType.Infrared)
                {
                    if (!camera.name.Equals("Fly_optical_camera"))
                        AsyncGPUReadback.Request(camera.activeTexture, 0, TextureFormat.RGB24, ReadbackCompleted);
                }
            }

        }

        void ReadbackCompleted(AsyncGPUReadbackRequest request)
        {
            
            ByteString data = ByteString.CopyFrom(request.GetData<byte>().ToArray());
            serviceImpl.Data = data;
            serviceImpl.DataLength = data.Length;

        }

        public SensorType GetType()
        {
            return type;
        }
    }
}
