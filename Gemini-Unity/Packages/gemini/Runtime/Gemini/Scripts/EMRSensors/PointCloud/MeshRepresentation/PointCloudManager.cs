using UnityEngine;
using System.Collections;
using Unity.Collections;
using Gemini.EMRS.Core;

namespace Gemini.EMRS.PointCloud
{
    public class PointCloudManager:MonoBehaviour
    {
        public Material _particleMaterial;
        public ComputeShader _computeParticle;

        private GameObject _cloudObject;
        private Mesh _particleMesh;

        public string _displaymentLayer = "Lidar";
        [SerializeField]private int _nrOfParticles;

        public void SetupPointCloud(int nrOfParticles)
        {
            _particleMesh = CreateMesh(nrOfParticles);

            _cloudObject = CreatePointCloudObject(_displaymentLayer);
            _cloudObject.GetComponent<MeshFilter>().mesh = _particleMesh;
            _cloudObject.GetComponent<Renderer>().material = _particleMaterial;
        }

        private Mesh CreateMesh(int nrOfParticles)
        {
            _nrOfParticles = nrOfParticles;

            int[] indices = new int[_nrOfParticles];
            indices = ArrayAllocator(_nrOfParticles, _computeParticle);

            Mesh mesh = new Mesh { vertices = new Vector3[_nrOfParticles] };
            mesh.SetIndices(indices, MeshTopology.Points, 0);

            return mesh;
        }

        public static int[] ArrayAllocator(int length, ComputeShader shader)
        {
            string kernelName = "QuickArrayAllocation";

            UnifiedArray<int> array = new UnifiedArray<int>(length, sizeof(int), "array");
            array.SetBuffer(shader, kernelName);
            shader.SetInt("arrayLength", length);

            return array.SynchUpdate(shader, kernelName).array;
        }

        private GameObject CreatePointCloudObject(string displaymentLayer)
        {
            GameObject obj = new GameObject();

            _displaymentLayer = displaymentLayer;
            obj.layer = LayerMask.NameToLayer(_displaymentLayer);

            obj.name = "PointCloud";

            obj.transform.SetParent(transform);
            obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            obj.transform.localPosition = new Vector3(0, 0, 0);

            obj.AddComponent<MeshFilter>();
            obj.AddComponent<MeshRenderer>();

            return obj;
        }


        public void UpdatePointCloud(NativeArray<Vector3> points){
            _particleMesh.SetVertices(points);
        }

        public void UpdatePointCloud(Vector3[] points)
        {
            _particleMesh.SetVertices(points);
        }

    }
}