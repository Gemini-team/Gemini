using System.Collections;
using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

using Gemini.EMRS.Lidar;

namespace Tests
{
    public class LidarVisualizationTest
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
            yield return new MonoBehaviourTest<LidarVisualizationMonoBehaviourTest>();
        }
    }

    public class LidarVisualizationMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
    {

        GameObject lidarObject;
        private LidarScript lidar;       
        private int frameCount;
        bool lidarPointIsInsideRange = false;
        bool particleSystemLidarPointIsInsideRange = false;

        public bool IsTestFinished
        {
            get 
            { 
                Assert.AreEqual("test_frame_id", lidar.FrameId);

                Vector3[] points = lidar.ParticleUnifiedArray.array;

                // Testing the lidar point array for visualization in Unity (particle system)
                for (int i = 0; i < points.Length; ++i)
                {
                    if (Math.Abs(points[i].z) > 8 && Math.Abs(points[i].z) < 9)
                    {
                        particleSystemLidarPointIsInsideRange = true;
                        break;
                    }
                }

                Assert.AreEqual(true, particleSystemLidarPointIsInsideRange);

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
    }
}
