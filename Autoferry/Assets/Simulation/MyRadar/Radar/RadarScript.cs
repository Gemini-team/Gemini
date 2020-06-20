using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Rendering;


public class RadarScript : MonoBehaviour
{

    public ComputeShader computeShader;
    [Range(3, 16)] public int NrOfCameras = 8;

    [Space]
    [Header("Radar Parameters")]
    public Texture2D RadiationPattern;
    public float AntennaGainDbi = 1;
    [SerializeField] private int HeightRes = 32;
    [SerializeField] private int WidthRes = 2048;
    [SerializeField]private int radarSweepResolution = 512;
    [SerializeField]private int radarSpokeResolution = 1024;
    public float PowerW = 20;
    public float RadarFrequencyGhz = 5.4f;
    [Range(5f, 5000f)] public float MaxDistance = 100F;
    [Range(0.01f, 2f)] public float MinDistance = 0.1F;
    [Range(5.0f, 90f)] public float VerticalAngle = 30f;


    [Space(10)]
    [Header("Debugging Options")]
    public RenderTexture RadarPlot;
    public RenderTexture RadarSpoke;
    [Range(1f, 30000)] public float Sensitivity = 500f;
    [Range(0, 360)] public float SpokeAngle = 0;

    [HideInInspector]public Camera[] radarCameras;
    private RenderTexture RadarPlotImage;
    private RenderTexture RadarSpokeImage;
    private NativeArray<int> RadarSpokesInt;
    private NativeArray<float> RadarSpokesFloat;

    void Start()
    {
        if (RadiationPattern == null) {
            RadiationPattern = new Texture2D(WidthRes,HeightRes);
        } else {
            HeightRes = RadiationPattern.height;
            WidthRes = RadiationPattern.width;
        }

        WidthRes /= NrOfCameras;

        radarCameras = SpawnCameras("DepthCam", NrOfCameras, WidthRes, HeightRes, VerticalAngle, RenderTextureFormat.Depth);

        StartCoroutine(UpdateRadar());
    }

    private IEnumerator UpdateRadar()
    {
        // Set Global Buffers
        computeShader.SetMatrix("inv_CameraMatrix", radarCameras[0].projectionMatrix.inverse);
        computeShader.SetInt("ImageWidthRes", WidthRes);
        computeShader.SetInt("ImageHeightRes", HeightRes);
        computeShader.SetInt("NrOfImages", NrOfCameras);
        computeShader.SetInt("radarResolution", radarSpokeResolution);
        computeShader.SetInt("radarSweepResolution", radarSweepResolution);
        computeShader.SetFloat("MaxDist", MaxDistance);
        computeShader.SetFloat("MinDist", MinDistance);
        computeShader.SetFloat("AntennaGainDbi", AntennaGainDbi);
        computeShader.SetFloat("PowerW", PowerW);
        computeShader.SetFloat("RadarFrequencyGhz", RadarFrequencyGhz);
        computeShader.SetFloat("SpokeAngle", SpokeAngle);

        // Radar Spokes buffers 
        int RadarSpokesHandle = computeShader.FindKernel("RadarSpokes");
        computeShader.SetTexture(RadarSpokesHandle, "RadarCharacteristics", RadiationPattern);
        int dataSize = sizeof(int);
        ComputeBuffer RadarSpokesIntBuffer = new ComputeBuffer(radarSpokeResolution* radarSweepResolution, dataSize);
        RadarSpokesInt = new NativeArray<int>(radarSpokeResolution*radarSweepResolution, Allocator.Temp, NativeArrayOptions.ClearMemory);
        RadarSpokesIntBuffer.SetData(RadarSpokesInt);
        computeShader.SetBuffer(RadarSpokesHandle, "RadarSpokesInt", RadarSpokesIntBuffer);
        for (int i = 0; i < radarCameras.Length; i++)
        {
            Quaternion angle = Quaternion.Euler(0, i*360.0f/NrOfCameras, 0);
            Matrix4x4 m = Matrix4x4.Rotate(angle);

            computeShader.SetTexture(RadarSpokesHandle, "depthImage" + i.ToString(), radarCameras[i].targetTexture);
            computeShader.SetMatrix("CameraRotationMatrix"+i.ToString(), m);
        }

        // Radar range Convolution buffers 
        int RadarRangeCovolutionHandle = computeShader.FindKernel("RadarRangeCovolution");
        computeShader.SetBuffer(RadarRangeCovolutionHandle, "RadarSpokesInt", RadarSpokesIntBuffer);
        dataSize = sizeof(float);
        ComputeBuffer RadarSpokesFloatBuffer = new ComputeBuffer(radarSpokeResolution * radarSweepResolution, dataSize);
        RadarSpokesFloat = new NativeArray<float>(radarSpokeResolution * radarSweepResolution, Allocator.Temp, NativeArrayOptions.ClearMemory);
        RadarSpokesFloatBuffer.SetData(RadarSpokesFloat);
        computeShader.SetBuffer(RadarRangeCovolutionHandle, "RadarSpokesFloat", RadarSpokesFloatBuffer);

        // Radar spoke max buffers 
        int RadarSpokeMax = computeShader.FindKernel("FindRadarSpokeMax");
        ComputeBuffer MaxSpokesBuffer = new ComputeBuffer(radarSweepResolution, sizeof(float));
        NativeArray<float> MaxSpokes = new NativeArray<float>(radarSweepResolution, Allocator.Temp, NativeArrayOptions.ClearMemory);
        MaxSpokesBuffer.SetData(MaxSpokes);
        computeShader.SetBuffer(RadarSpokeMax, "MaxSpokes", MaxSpokesBuffer);
        computeShader.SetBuffer(RadarSpokeMax, "RadarSpokesFloat", RadarSpokesFloatBuffer);
        
        // Radar plot image buffers 
        int RadarPlotImageHandle = computeShader.FindKernel("CreateRadarPlotImage");
        if (RadarPlot != null){
            dataSize = 24;
            RadarPlotImage = new RenderTexture(RadarPlot.width, RadarPlot.height, dataSize);
            RadarPlotImage.enableRandomWrite = true;
            RadarPlotImage.Create();
            computeShader.SetTexture(RadarPlotImageHandle, "plotImage", RadarPlotImage);
            computeShader.SetBuffer(RadarPlotImageHandle, "RadarSpokesFloat", RadarSpokesFloatBuffer);
            computeShader.SetBuffer(RadarPlotImageHandle, "MaxSpokes", MaxSpokesBuffer);
        }

        // Radar spoke image buffers 
        int RadarSpokeImageHandle = computeShader.FindKernel("CreateRadarSpokeImage");
        if(RadarSpoke != null){
            dataSize = 24;
            RadarSpokeImage = new RenderTexture(RadarSpoke.width, RadarSpoke.height, dataSize);
            RadarSpokeImage.enableRandomWrite = true;
            RadarSpokeImage.Create();
            computeShader.SetTexture(RadarSpokeImageHandle, "spokeImage", RadarSpokeImage);
            computeShader.SetBuffer(RadarSpokeImageHandle, "RadarSpokesFloat", RadarSpokesFloatBuffer);
            computeShader.SetBuffer(RadarSpokeImageHandle, "MaxSpokes", MaxSpokesBuffer);
        }

        // Radar clear buffers 
        int ClearKernelHandle = computeShader.FindKernel("ClearRadar");
        computeShader.SetBuffer(ClearKernelHandle, "RadarSpokesInt", RadarSpokesIntBuffer);

        // Fetch data from GPU
        float tempSens = -1;
        float tempSpokeAngle = -1;

        while (true)
        {
            // Check change in Inspector Variables
            if (Sensitivity != tempSens) {
                computeShader.SetFloat("Sensitivity", Sensitivity);
                tempSens = Sensitivity;
            }
            if (SpokeAngle != tempSpokeAngle) {
                computeShader.SetFloat("SpokeAngle", SpokeAngle);
                tempSpokeAngle = SpokeAngle;
            }

            // Create and recieve Radar Data
            computeShader.Dispatch(RadarSpokesHandle, (int)Mathf.Ceil(NrOfCameras * WidthRes / 32), (int)Mathf.Ceil(HeightRes / 32), 1);
            computeShader.Dispatch(RadarRangeCovolutionHandle, (int)Mathf.Ceil(radarSweepResolution / 512), 1, 1);
            computeShader.Dispatch(RadarSpokeMax, (int)Mathf.Ceil(radarSweepResolution / 512), 1, 1);

            var request = AsyncGPUReadback.Request(RadarSpokesFloatBuffer);
            yield return new WaitUntil(() => request.done);
            RadarSpokesFloat = request.GetData<float>();
       
            // Debugging Images
            if (RadarPlot != null){
                computeShader.Dispatch(RadarPlotImageHandle, (int)Mathf.Ceil(RadarPlotImage.width / 32), (int)Mathf.Ceil(RadarPlotImage.height / 32), 1);
            }

            if (RadarSpoke != null){
                computeShader.Dispatch(RadarSpokeImageHandle, (int)Mathf.Ceil(RadarSpokeImage.width / 32), (int)Mathf.Ceil(RadarSpokeImage.height / 32), 1);
            }

            // Clear GPU memory
            computeShader.Dispatch(ClearKernelHandle, (int)Mathf.Ceil((radarSpokeResolution*radarSweepResolution) / 1024), 1, 1);
        }
    }

    private Camera[] SpawnCameras(string name, int numbers, int Width, int Height, float verticalAngle, RenderTextureFormat format)
    {

        Camera[] Cameras = new Camera[numbers];
        for (int i = 0; i < numbers; i++)
        {
            GameObject CameraObject = new GameObject();
            CameraObject.name = name + i;
            CameraObject.transform.SetParent(transform);
            CameraObject.transform.localRotation = Quaternion.Euler(0, 180 + (1/2 + i)*360.0f/numbers, 0);
            CameraObject.transform.localPosition = new Vector3(0, 0, 0);

            CameraObject.AddComponent<Camera>();
            Camera cam = CameraObject.GetComponent<Camera>();

            if (cam.targetTexture == null)
            {
                cam.targetTexture = new RenderTexture(Width, Height, 32, format);
            }

            cam.usePhysicalProperties = false;
            cam.aspect = (360.0f / numbers)/ verticalAngle;
            cam.fieldOfView = Camera.HorizontalToVerticalFieldOfView(360.0f / numbers, cam.aspect);
            cam.farClipPlane = MaxDistance;
            cam.nearClipPlane = MinDistance;
            cam.enabled = false;
            cam.cullingMask = LayerMask.GetMask("Radar");
            Cameras[i] = cam;
        }
        return Cameras;
    }

    void OnEnable()
    {
        RenderPipelineManager.endFrameRendering += EndCameraRendering;
    }

    void OnDisable()
    {
        RenderPipelineManager.endFrameRendering -= EndCameraRendering;
    }

    void EndCameraRendering(ScriptableRenderContext context, Camera[] cam)
    {
        if (RadarPlot != null){
            Graphics.Blit(RadarPlotImage, RadarPlot);
        }
        if(RadarSpoke != null){
            Graphics.Blit(RadarSpokeImage, RadarSpoke);
        }
    }
}
