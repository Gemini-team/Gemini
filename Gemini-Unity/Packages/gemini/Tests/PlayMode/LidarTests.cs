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

                LidarMessage message = new LidarMessage((int)lidar.NumberOfLidarPoints, 0.0, lidar.LidarDataByte.array);
                
                Vector4[] lidarPoints = message.ParseLidarPoints();

                // Testing the lidar point byte array which is accessed by external sources
                for (int i = 0; i < lidarPoints.Length; i++)
                {
                    // Use x coordinate here because fucking NED Coordinate system in LidarCS
                    if (Math.Abs(lidarPoints[i].x) > 8 && Math.Abs(lidarPoints[i].x) < 9)
                    {
                        lidarPointIsInsideRange = true;
                        break;
                    }
                }

                Assert.AreEqual(true, lidarPointIsInsideRange);

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
