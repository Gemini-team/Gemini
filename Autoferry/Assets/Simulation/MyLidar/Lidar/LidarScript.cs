using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Material))]


public class LidarScript : MonoBehaviour
{
    public ComputeShader computeShader;
    [Range(3, 32)] public int NrOfCameras = 4;

    [Space]
    [Header("Lidar Parameters")]
    public int HeightRes = 32;
    public int WidthRes = 2048;
    [Range(0.01f, 2f)] public float MinDistance = 0.1F;
    [Range(5f, 1000f)] public float MaxDistance = 100F;
    [Range(5.0f, 90f)] public float VerticalAngle = 30f;

    [Space(10)]
    [Header("Debugging Options")]
    public Material particleMaterial;
    public bool ShowPointCloud = true;

    [HideInInspector]public Camera[] lidarCameras;
    private Mesh particleMesh;
    private NativeArray<Vector3> particleCloud;

    void Start()
    {
        WidthRes /= NrOfCameras;

        lidarCameras = SpawnCameras("DepthCam", NrOfCameras, WidthRes, HeightRes, VerticalAngle, RenderTextureFormat.Depth);

        SetupLidarPointCloud();

        StartCoroutine(UpdateLidarLines());
    }

    private IEnumerator UpdateLidarLines()
    {
        // Setting up Global Buffers

        computeShader.SetMatrix("inv_CameraMatrix", lidarCameras[0].projectionMatrix.inverse);
        computeShader.SetInt("ImageWidthRes", WidthRes);
        computeShader.SetInt("ImageHeightRes", HeightRes);
        computeShader.SetInt("NrOfImages", NrOfCameras);

        // Setting up Compute Buffers

        int kernelHandle = computeShader.FindKernel("CSMain");
        int dataSize = sizeof(float) * 3;
        ComputeBuffer particleBuffer = new ComputeBuffer(NrOfCameras * HeightRes * WidthRes, dataSize);
        particleCloud = new NativeArray<Vector3>(NrOfCameras * HeightRes * WidthRes, Allocator.Temp, NativeArrayOptions.ClearMemory);
        particleBuffer.SetData(particleCloud);
        computeShader.SetBuffer(kernelHandle, "lines", particleBuffer);
        for(int i = 0; i < lidarCameras.Length; i++)
        {
            Quaternion angle = Quaternion.Euler(0, i*360.0f/NrOfCameras, 0);
            Matrix4x4 m = Matrix4x4.Rotate(angle);

            computeShader.SetTexture(kernelHandle, "depthImage" + i.ToString(), lidarCameras[i].targetTexture);
            computeShader.SetMatrix("CameraRotationMatrix"+i.ToString(), m);
        }

        // Fetch data from GPU

        while (true)
        {
            computeShader.Dispatch(kernelHandle, (int)Mathf.Ceil(NrOfCameras * WidthRes / 64), (int)Mathf.Ceil(HeightRes / 16), 1);

            var request = AsyncGPUReadback.Request(particleBuffer);
            yield return new WaitUntil(() => request.done);
            particleCloud = request.GetData<Vector3>();

            if (ShowPointCloud){ particleMesh.SetVertices(particleCloud);}
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
            CameraObject.transform.localRotation = Quaternion.Euler(0, i * 360.0f / numbers, 0);
            CameraObject.transform.localPosition = new Vector3(0, 0, 0);

            CameraObject.AddComponent<Camera>();
            Camera cam = CameraObject.GetComponent<Camera>();

            if (cam.targetTexture == null)
            {
                cam.targetTexture = new RenderTexture(Width, Height, 1, format);
            }

            cam.usePhysicalProperties = false;
            cam.aspect = (360.0f / numbers)/ verticalAngle;
            cam.fieldOfView = Camera.HorizontalToVerticalFieldOfView(360.0f / numbers, cam.aspect);
            cam.farClipPlane = MaxDistance;
            cam.enabled = false;
            cam.nearClipPlane = MinDistance;
            Cameras[i] = cam;
        }
        return Cameras;
    }

    private void SetupLidarPointCloud()
    {
        int[] indices = new int[HeightRes * NrOfCameras * WidthRes];
        indices = ArrayAllocator(HeightRes * NrOfCameras * WidthRes);
        particleMesh = new Mesh{vertices = new Vector3[HeightRes * NrOfCameras * WidthRes]};
        particleMesh.SetIndices(indices, MeshTopology.Points, 0);
        
        GetComponent<MeshFilter>().mesh = particleMesh;
        GetComponent<Renderer>().material = particleMaterial;
    }

    private int[] ArrayAllocator(int length)
    {
        int kernel = computeShader.FindKernel("QuickArrayAllocation");

        // Set up buffers to GPU
        ComputeBuffer arrayBuffer = new ComputeBuffer(length, sizeof(int));
        int[] array = new int[length];
        arrayBuffer.SetData(array);
        computeShader.SetBuffer(kernel, "array", arrayBuffer);

        // Wait for data from GPU
        computeShader.Dispatch(kernel, (int)Mathf.Ceil(length / 1024), 1, 1);
        arrayBuffer.GetData(array);

        // Cleanup and return
        arrayBuffer.Release();
        arrayBuffer.Dispose();
        return array;
    }
}
