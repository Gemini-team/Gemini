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

        [Space]
        [Header("Lidar Parameters")]
        public int LidarHorisontalRes = 2048; // full rotation res..
        public int NrOfLasers = 16;
        [Range(0.012f, 1f)] public float ErrorTolerance = 0.02f;
        [Range(0.01f, 2f)] public float MinDistance = 0.1F;
        [Range(5f, 1000f)] public float MaxDistance = 100F;
        [Range(5.0f, 90f)] public float VerticalAngle = 30f;
        [Range(0.0f, 1f)] public float rayDropProbability = 0.1f;


        // [Space]
        // [Header("Calculated parameters")]
        // [ReadOnly, SerializeField] private float FrustumVerticalAngle = -1;
        // [ReadOnly, SerializeField] private float FrustumHorizontalAngle = -1;
        // [ReadOnly, SerializeField] private int FrustumWidthRes = -1;
        // [ReadOnly, SerializeField] private int FrustumHeightRes = -1;

        [Space]
        [Header("Camera Parameters")]
        public DepthCameras.BufferPrecision DepthBufferPrecision = DepthCameras.BufferPrecision.bit24;
        [Range(3, 5)] public int NrOfCameras = 4;


        [Space]
        [Header("Sensor Output")]
        public string FrameId;
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

            // Setup depth cameras

            // Placeholder horizontal res of 1000, is replaced by 
            // resolution determined by configured ErrorTolerance. 
            var frustum = new CameraFrustum(1000, MaxDistance, MinDistance,
                2f * Mathf.PI / NrOfCameras, Mathf.Deg2Rad * VerticalAngle);

            float minTol = LidarTolerances.minAchieveableTol(frustum, 24);
            float minTolPadding = 1.5f;
            float paddedMinTol = minTol * minTolPadding;

            if (ErrorTolerance < paddedMinTol)
            {
                Debug.LogError("Configured lidar tolerance: " + ErrorTolerance.ToString() +
                    " is set below minimal allowed tolerance: " + paddedMinTol.ToString() + ".\n" +
                    "This tolerance is set as the theoretical tolerance for infinite resolution: "
                     + minTol.ToString() + " scaled by " + minTolPadding.ToString()
                );
                // FIXME throw some exception here...
                Application.Quit();
            }

            uint depthBits = 24;
            switch (DepthBufferPrecision)
            {
                case DepthCameras.BufferPrecision.bit16:
                    depthBits = 16;
                    break;
                case DepthCameras.BufferPrecision.bit24:
                    depthBits = 24;
                    break;
                case DepthCameras.BufferPrecision.bit32:
                    depthBits = 32;
                    break;
                default:
                    // FIXME throw some exception here
                    Debug.LogError("LidarScript: No suitable mapping for DepthBufferPrecision found.");
                    Application.Quit();
                    break;
            }

            float requiredWidthRes = LidarTolerances.minHorizRes(ErrorTolerance, frustum, depthBits);

            frustum = new CameraFrustum((int)requiredWidthRes, MaxDistance, MinDistance,
                2f * Mathf.PI / NrOfCameras, Mathf.Deg2Rad * VerticalAngle);

            Debug.Log("Frustum res: " + frustum._pixelWidth.ToString() + " x " + frustum._pixelHeight.ToString());

            NumberOfDepthPixels = (uint)frustum._pixelWidth * (uint)frustum._pixelHeight * (uint)NrOfCameras;
            numberOfLidarPoints = (uint)NrOfLasers * (uint)LidarHorisontalRes;
            depthCameras = new DepthCameras(NrOfCameras, frustum, this.transform, lidarShader, "CSMain");
            lidarCameras = depthCameras.cameras;

            // Setup Game objects

            int horizontalReflectionsPerCamera = (int)((float)LidarHorisontalRes / NrOfCameras);

            pointCloud = GetComponent<PointCloudManager>();
            pointCloud.SetupPointCloud((int)numberOfLidarPoints);

            projectionFilter = GetComponent<SphericalProjectionFilter>();
            projectionFilter.SetupSphericalProjectionFilter(horizontalReflectionsPerCamera, NrOfLasers, frustum);
            pixelCoordinatesBuffer = projectionFilter.filterCoordinates.buffer;

            // Setting up Compute Buffers

            kernelHandle = lidarShader.FindKernel("CSMain");

            lidarShader.SetBuffer(kernelHandle, "sphericalPixelCoordinates", pixelCoordinatesBuffer);
            lidarShader.SetInt("N_theta", horizontalReflectionsPerCamera); // horizontal reflections per camera.. 
            lidarShader.SetInt("N_phi", NrOfLasers);
            lidarShader.SetFloat("rayDropProbability", rayDropProbability);

            UnifiedArray<uint> RandomStateVector = new UnifiedArray<uint>(NrOfLasers * LidarHorisontalRes, sizeof(float), "_state_xorshift");
            RandomStateVector.SetBuffer(lidarShader, "CSMain");
            RandomStateVector.SetBuffer(lidarShader, "RNG_Initialize");
            RandomStateVector.SynchUpdate(lidarShader, "RNG_Initialize");

            particleUnifiedArray = new UnifiedArray<Vector3>(NrOfLasers * LidarHorisontalRes, sizeof(float) * 3, "lines");
            particleUnifiedArray.SetBuffer(lidarShader, "CSMain");

            lidarDataByte = new UnifiedArray<byte>(NrOfLasers * LidarHorisontalRes, sizeof(float) * 6, "LidarData");
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
