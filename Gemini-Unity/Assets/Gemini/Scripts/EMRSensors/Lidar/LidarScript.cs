using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

using Google.Protobuf;
using Grpc.Core;

using Gemini.EMRS.Core;
using Gemini.EMRS.Core.ZBuffer;
using Gemini.EMRS.PointCloud;
using Sensorstreaming;



namespace Gemini.EMRS.Lidar {
    [RequireComponent(typeof(SphericalProjectionFilter))]
    [RequireComponent(typeof(PointCloudManager))]
    public class LidarScript : Sensor
    {
        public ComputeShader lidarShader;
        [Range(3, 5)] public int NrOfCameras = 4;

        [Space]
        [Header("Lidar Parameters")]
        public int WidthRes = 2048;
        private int HeightRes = 32;

        public DepthCameras.BufferPrecision DepthBufferPrecision = DepthCameras.BufferPrecision.bit24;

        public int LidarHorisontalRes = 1024;
        public int NrOfLasers = 16;
        [Range(0.0f, 1f)] public float rayDropProbability = 0.1f;
        [Range(0.01f, 2f)] public float MinDistance = 0.1F;
        [Range(5f, 1000f)] public float MaxDistance = 100F;
        [Range(5.0f, 90f)] public float VerticalAngle = 30f;


        [Space]
        [Header("Sensor Output")]
        [SerializeField] private uint NumberOfDepthPixels = 0;
        [SerializeField] private uint NumberOfLidarPoints = 0;

        [HideInInspector] public Camera[] lidarCameras;
        private DepthCameras depthCameras;
        private ComputeBuffer pixelCoordinatesBuffer;

        private PointCloudManager pointCloud;
        private SphericalProjectionFilter projectionFilter;

        int kernelHandle;
        UnifiedArray<Vector3> particleUnifiedArray;
        UnifiedArray<byte> lidarDataByte;

        void Start()
        {

            // Setting User information

            WidthRes /= NrOfCameras;

            float lidarVerticalAngle = VerticalAngle;
            HeightRes = (int)(WidthRes * Mathf.Tan(lidarVerticalAngle * Mathf.Deg2Rad / 2) / Mathf.Sin(Mathf.PI / NrOfCameras));
            VerticalAngle = Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan(lidarVerticalAngle * Mathf.Deg2Rad / 2) / Mathf.Cos(Mathf.PI / NrOfCameras));

            NumberOfDepthPixels = (uint)WidthRes * (uint)HeightRes * (uint)NrOfCameras;
            NumberOfLidarPoints = (uint)NrOfLasers * (uint)LidarHorisontalRes * (uint)NrOfCameras;

            // Settup Game objects

            pointCloud = GetComponent<PointCloudManager>();
            pointCloud.SetupPointCloud((int)NumberOfLidarPoints);

            var frustum = new CameraFrustum(WidthRes, MaxDistance, MinDistance, 2*Mathf.PI/NrOfCameras, lidarVerticalAngle*Mathf.Deg2Rad);
            depthCameras = new DepthCameras(NrOfCameras, frustum, this.transform,lidarShader,"CSMain");
            lidarCameras = depthCameras.cameras;

            projectionFilter = GetComponent<SphericalProjectionFilter>();
            projectionFilter.SetupSphericalProjectionFilter(LidarHorisontalRes, NrOfLasers, frustum);
            pixelCoordinatesBuffer = projectionFilter.filterCoordinates.buffer;

            // Setting up Compute Buffers

            kernelHandle = lidarShader.FindKernel("CSMain");

            lidarShader.SetBuffer(kernelHandle, "sphericalPixelCoordinates", pixelCoordinatesBuffer);
            lidarShader.SetInt("N_theta", LidarHorisontalRes);
            lidarShader.SetInt("N_phi", NrOfLasers);
            lidarShader.SetFloat("rayDropProbability", rayDropProbability);

            UnifiedArray<uint> RandomStateVector = new UnifiedArray<uint>(NrOfCameras * NrOfLasers * LidarHorisontalRes, sizeof(float), "_state_xorshift");
            RandomStateVector.SetBuffer(lidarShader, "CSMain");
            RandomStateVector.SetBuffer(lidarShader, "RNG_Initialize");
            RandomStateVector.SynchUpdate(lidarShader, "RNG_Initialize");

            particleUnifiedArray = new UnifiedArray<Vector3>(NrOfCameras * NrOfLasers * LidarHorisontalRes, sizeof(float) * 3, "lines");
            particleUnifiedArray.SetBuffer(lidarShader, "CSMain");

            lidarDataByte = new UnifiedArray<byte>(NrOfCameras * NrOfLasers * LidarHorisontalRes, sizeof(float) * 6, "LidarData");
            lidarDataByte.SetBuffer(lidarShader, "CSMain");
        }

        private void Awake()
        {
            SetupSensorCallbacks(
            new SensorCallback(LidarUpdate, SensorCallbackOrder.First),
            new SensorCallback(PointCloudRendering, SensorCallbackOrder.Last),
            new SensorCallback(RecieveLidarData, SensorCallbackOrder.Last)
            );
        }

        public bool SynchronousUpdate = false;

        void LidarUpdate(ScriptableRenderContext context, Camera[] cameras)
        {
            lidarShader.SetFloat("rayDropProbability", rayDropProbability);
            lidarShader.Dispatch(kernelHandle, (int)Mathf.Ceil((float)NrOfCameras * (float)NrOfLasers * (float)LidarHorisontalRes / 1024.0f), 1, 1);
        }

        void PointCloudRendering(ScriptableRenderContext context, Camera[] cameras)
        {
            if (SynchronousUpdate)
            {
                particleUnifiedArray.SynchUpdate(lidarShader,"CSMain");
                if (pointCloud != null) { pointCloud.UpdatePointCloud(particleUnifiedArray.array); }
                gate = true;
            }
            else
            {
                AsyncGPUReadback.Request(particleUnifiedArray.buffer, PointCloudCompleted);
            }
        }

        void PointCloudCompleted(AsyncGPUReadbackRequest request)
        {
            particleUnifiedArray.AsynchUpdate(request);
            if (pointCloud != null) { pointCloud.UpdatePointCloud(particleUnifiedArray.nativeArray); }
            gate = true;
        }

        void RecieveLidarData(ScriptableRenderContext context, Camera[] cameras)
        {
            if (SynchronousUpdate)
            {
                lidarDataByte.SynchUpdate(lidarShader, "CSMain");
                message = new LidarMessage(LidarHorisontalRes * NrOfLasers * NrOfCameras, OSPtime, lidarDataByte.array);
                gate = true;
            }
            else
            {
                AsyncGPUReadback.Request(lidarDataByte.buffer, PointCloudDataCompleted);
            }
        }

        private LidarMessage message;

        void PointCloudDataCompleted(AsyncGPUReadbackRequest request)
        {
            lidarDataByte.AsynchUpdate(request);
            message = new LidarMessage(LidarHorisontalRes * NrOfLasers * NrOfCameras, OSPtime, lidarDataByte.array);
            gate = true;


            /*
            var stringarray = System.BitConverter.ToSingle(lidarDataByte.array, lidarDataByte.array.Length - 12);
            Debug.Log("x in bytes: " + stringarray.ToString());
            Debug.Log("CPU is little endian: " + System.BitConverter.IsLittleEndian.ToString());
            */
        }

        // Memo: hadde lønnt seg å lage en konstruktor for dette i den autogenererte koden
        public override bool SendMessage()
        {
            //Debug.Log("Lidar message time: " + message.timeInSeconds.ToString());
            
            LidarStreamingRequest lidarStreamingRequest = new LidarStreamingRequest();
            
            lidarStreamingRequest.TimeInSeconds = message.timeInSeconds;
            lidarStreamingRequest.Height = message.height;
            lidarStreamingRequest.Width = message.width;

            PointField[] fields = message.fields;
            Sensorstreaming.PointField[] pointFields = new Sensorstreaming.PointField[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                Sensorstreaming.PointField tempPointField = new Sensorstreaming.PointField();
                tempPointField.Name = fields[i]._name;
                tempPointField.Offset = fields[i]._offset;
                tempPointField.Datatype = fields[i]._datatype;
                tempPointField.Count = fields[i]._count;

                lidarStreamingRequest.Fields.Add(tempPointField);
            }

            lidarStreamingRequest.IsBigEndian = message.is_bigendian;
            lidarStreamingRequest.PointStep = message.point_step;
            lidarStreamingRequest.RowStep = message.row_step;
            lidarStreamingRequest.Data = ByteString.CopyFrom(message.data);
            lidarStreamingRequest.IsDense = message.is_dense;

            lidarStreamingRequest.IsDense = message.is_dense;

            bool success = false;
            connectionTime = Time.time;
            if (connectionTime < ConnectionTimeout || connected)
            {
                try
                {
                    success = _streamingClient.StreamLidarSensor(lidarStreamingRequest).Success;
                    connected = success;
                } catch(RpcException e)
                {
                    Debug.LogException(e);
                }
            }

            return success;
        }

    }

}
