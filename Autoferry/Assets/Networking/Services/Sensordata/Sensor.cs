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
        Optical,
        Infrared, 
        Radar,
        Lidar
    }


    class Sensor : MonoBehaviour
    {
        public SensorType type;
        public string host = "localhost";

        private int port = ServicePortGenerator.GenPort();
        private Server server;
        private SensordataServiceImpl serviceImpl;

        private ByteString data = ByteString.CopyFromUtf8("");
        private int dataLength = 0;

        private Camera camera;
        private RenderTexture renderTexture;

        public Sensor()
        {
        }


        void Start()
        {

            camera = gameObject.GetComponent<Camera>();
            renderTexture = camera.targetTexture;

            if ( renderTexture == null)
            {
                renderTexture = new RenderTexture(800, 640, 24, RenderTextureFormat.Default, 0);
                camera.targetTexture = renderTexture;
            } 

            serviceImpl = new SensordataServiceImpl(data, dataLength);

            server = new Server
            {
                Services = { Sensordata.Sensordata.BindService(serviceImpl) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            Debug.Log("Sensordata server listening on port: " + port);
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
            if (type == SensorType.Optical || type == SensorType.Infrared)
            {
                /*
                foreach (Camera cam in cameras)
                {
                    // TODO: Need to add logic to only use the camera on this sensor
                    if (!cam.name.Equals("Fly_optical_camera"))
                        AsyncGPUReadback.Request(cam.activeTexture, 0, TextureFormat.RGB24, ReadbackCompleted);
                }
                */

                if (!camera.name.Equals("Fly_optical_camera"))
                    AsyncGPUReadback.Request(camera.activeTexture, 0, TextureFormat.RGB24, ReadbackCompleted);

            }
        }

        void ReadbackCompleted(AsyncGPUReadbackRequest request)
        {
            
            ByteString data = ByteString.CopyFrom(request.GetData<byte>().ToArray());
            serviceImpl.Data = data;
            serviceImpl.DataLength = data.Length;

        }
    }
}
