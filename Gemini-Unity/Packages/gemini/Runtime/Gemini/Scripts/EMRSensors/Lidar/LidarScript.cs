using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

using Google.Protobuf;
using Grpc.Core;

using Gemini.EMRS.Core;
using Gemini.EMRS.Core.ZBuffer;
using Gemini.EMRS.PointCloud;
using Sensorstreaming;



namespace Gemini.EMRS.Lidar
{
    [RequireComponent(typeof(SphericalProjectionFilter))]
    [RequireComponent(typeof(PointCloudManager))]
    public class LidarScript : Sensor
    {
        public ComputeShader lidarShader;
        [Range(3, 5)] public int NrOfCameras = 4;

        [Space]
        [Header("Lidar Parameters")]
        public string FrameId;
        public int FullRotationWidthRes = 32768;

        public DepthCameras.BufferPrecision DepthBufferPrecision = DepthCameras.BufferPrecision.bit24;

        public int LidarHorisontalRes = 1024;
        public int NrOfLasers = 16;
        [Range(0.0f, 1f)] public float rayDropProbability = 0.1f;
        [Range(0.01f, 2f)] public float MinDistance = 0.1F;
        [Range(5f, 1000f)] public float MaxDistance = 100F;
        [Range(5.0f, 90f)] public float VerticalAngle = 30f;


        [Space]
        [Header("Calculated parameters")]
        [ReadOnly, SerializeField] private float FrustumVerticalAngle = -1;
        [ReadOnly, SerializeField] private float FrustumHorizontalAngle = -1;
        [ReadOnly, SerializeField] private int FrustumWidthRes = -1;
        [ReadOnly, SerializeField] private int FrustumHeightRes = -1;


        [Space]
        [Header("Sensor Output")]
        [ReadOnly, SerializeField] private uint NumberOfDepthPixels = 0;
        [ReadOnly, SerializeField] private uint numberOfLidarPoints = 0;

        public uint NumberOfLidarPoints
        {
            get { return numberOfLidarPoints; }
        }

        [HideInInspector] public Camera[] lidarCameras;
        private DepthCameras depthCameras;
        private ComputeBuffer pixelCoordinatesBuffer;

        private PointCloudManager pointCloud;
        private SphericalProjectionFilter projectionFilter;

        int kernelHandle;
        private UnifiedArray<Vector3> particleUnifiedArray;

        public UnifiedArray<Vector3> ParticleUnifiedArray
        {
            get { return particleUnifiedArray; }
        }

        private UnifiedArray<byte> lidarDataByte;

        public UnifiedArray<byte> LidarDataByte
        {
            get { return lidarDataByte; }
        }

        void Start()
        {

            // Setting User information

            /* 
                Note that the frustum is a projected square, by extension its effective 
                V_FOV is dependent on which horizontal angle you're looking at.

                E.g. at the center of a frustum, the effective V_FOV is equal to the
                frustum V_FOV, while it gets smaller towards the edges.

                In order to correct for this, we need to set a larger frustum V_FOV such that
                the effective V_FOV at the intersections between frustums (i.e. angle from center equal to H_FOV/2)
                is equal to the configured lidar V_FOV. This correction is found as: 
                tan(CORRECTED_FRUSTUM_V_FOV/2) = tan(LIDAR_V_FOV/2)/cos(H_FOV/2)
            */
            FrustumWidthRes = FullRotationWidthRes / NrOfCameras;
            FrustumHeightRes = (int)(FrustumWidthRes * Mathf.Tan(VerticalAngle * Mathf.Deg2Rad / 2) / Mathf.Sin(Mathf.PI / NrOfCameras));

            FrustumVerticalAngle = Mathf.Rad2Deg * 2 * Mathf.Atan(Mathf.Tan(VerticalAngle * Mathf.Deg2Rad / 2) / Mathf.Cos(Mathf.PI / NrOfCameras));
            FrustumHorizontalAngle = Mathf.Rad2Deg * 2 * Mathf.PI / NrOfCameras;

            NumberOfDepthPixels = (uint)FrustumWidthRes * (uint)FrustumHeightRes * (uint)NrOfCameras;
            numberOfLidarPoints = (uint)NrOfLasers * (uint)LidarHorisontalRes * (uint)NrOfCameras;


            // Settup Game objects

            pointCloud = GetComponent<PointCloudManager>();
            pointCloud.SetupPointCloud((int)numberOfLidarPoints);

            var frustum = new CameraFrustum(FrustumWidthRes, FrustumHeightRes, MaxDistance, MinDistance, Mathf.Deg2Rad * FrustumHorizontalAngle,
                                                Mathf.Deg2Rad * FrustumVerticalAngle, Mathf.Deg2Rad * VerticalAngle);
            depthCameras = new DepthCameras(NrOfCameras, frustum, this.transform, lidarShader, "CSMain");
            lidarCameras = depthCameras.cameras;


            float minHorizRes = LidarTolerances.minHorizRes(0.01f, frustum, 24);
            Debug.Log("Minimal horiz res: " + minHorizRes);

            float minTol = LidarTolerances.minAchieveableTol(frustum, 24);
            Debug.Log("Minimal achieveable tol: " + minTol);

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
                particleUnifiedArray.SynchUpdate(lidarShader, "CSMain");
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
        }

        public override bool SendMessage()
        {
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
            lidarStreamingRequest.FrameId = FrameId;

            bool success = false;
            connectionTime = Time.time;
            if (connectionTime < ConnectionTimeout || connected)
            {
                try
                {
                    success = _streamingClient.StreamLidarSensor(lidarStreamingRequest).Success;
                    connected = success;
                }
                catch (RpcException e)
                {
                    Debug.LogException(e);
                }
            }

            return success;
        }
    }
}
