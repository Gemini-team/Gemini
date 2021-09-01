using System.Collections;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

using Gemini.EMRS.Lidar;

namespace Tests
{

    public class LidarTest
    {



        [OneTimeSetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("Test");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            SceneManager.UnloadSceneAsync("Test");
        }

        [UnityTest]
        public IEnumerator LidarMonoBehaviourTest()
        {
            yield return new MonoBehaviourTest<LidarMonoBehaviourTest>();
        }

    }

    public class LidarMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
    {

        struct LidarPoint
        {
            public uint x;
            public uint y;
            public uint z;
            public uint intensity;

            LidarPoint (uint x, uint y, uint z, uint intensity)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                this.intensity = intensity;
            }
        }

        GameObject lidarObject;
        private LidarScript lidar;       
        private int frameCount;
        bool nonZeroZValue = false;
        public bool IsTestFinished
        {
            get 
            { 
                Assert.AreEqual("test_frame_id", lidar.FrameId);

                LidarPoint[] points = ParseLidarPoints(lidar.lidarDataByte.array);

                for (int i = 0; i < points.Length; ++i)
                {
                    Debug.Log("Z: " + points[i].z);
                    if (points[i].z != 0)
                        nonZeroZValue = true;
                }

                Assert.AreEqual(true, nonZeroZValue);

                return frameCount == 1 ; 
            }
        }

        void Awake()
        {
            lidar = FindObjectOfType<LidarScript>();
        }

        void Update()
        {
            frameCount++;
        }


        LidarPoint[] ParseLidarPoints(byte[] data)
        {
            int offset = 24;
            LidarPoint[] points = new LidarPoint[data.Length / offset]; 
            byte[] slice = new byte[16];

            int pointIndex = 0;

            for (int i = 0; i < data.Length; ++i)
            {
                if (i % 24 == 0)
                {
                    points[pointIndex] = new LidarPoint();

                    // TODO: Need some check
                    uint x = 0;
                    uint y = 0;
                    uint z = 0;
                    uint intensity = 0;

                    if (i + 4 < data.Length)
                        x = BitConverter.ToUInt32(data, i);
                    if (i + 8 < data.Length)
                        y = BitConverter.ToUInt32(data, i + 4);
                    if (i + 12 < data.Length)
                        z = BitConverter.ToUInt32(data, i + 8);
                    if (i + 16 < data.Length)
                        intensity = BitConverter.ToUInt32(data, i + 12);

                    points[pointIndex].x = x;
                    points[pointIndex].y = y;
                    points[pointIndex].z = z;
                    points[pointIndex].intensity = intensity;

                    pointIndex++;
                }
            }
            return points;
        }
    }
}
