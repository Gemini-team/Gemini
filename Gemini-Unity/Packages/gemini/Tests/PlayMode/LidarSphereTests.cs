using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.Assertions;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using Gemini.EMRS.Lidar;

namespace Tests
{
    public class LidarSphereTests
    {
        [OneTimeSetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("SphereTest");
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            SceneManager.UnloadSceneAsync("SphereTest");
        }

        [UnityTest]
        public IEnumerator LidarMonoBehaviourTest()
        {
            yield return new MonoBehaviourTest<LidarSphereMonoBehaviourTest>();
        }
    }

    public class LidarSphereMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
    {

        GameObject lidarObject;
        int sphereRadius = 1;
        float delta = 0.1f;
        private LidarScript lidar;       
        private int frameCount;

        bool allLidarPointsInsideSameRadius = true;

        float distance = 0.0f;

        public bool IsTestFinished
        {

            get 
            { 

                UnityEngine.Assertions.Assert.AreEqual("test_frame_id", lidar.FrameId);

                LidarMessage message = new LidarMessage((int)lidar.NumberOfLidarPoints, 0.0, lidar.LidarDataByte.array);
                
                Vector4[] lidarPoints = message.ParseLidarPoints();

                // Testing the lidar point byte array which is accessed by external sources
                for (int i = 0; i < lidarPoints.Length; i++)
                {
                    // Use x coordinate here because NED Coordinate system in LidarCS
                    //if (Math.Abs(lidarPoints[i].x) > 8 && Math.Abs(lidarPoints[i].x) < 9)
                    distance = (float)Math.Sqrt(Math.Pow(lidarPoints[i].x, 2) + Math.Pow(lidarPoints[i].y, 2) + Math.Pow(lidarPoints[i].z, 2));
                    //Debug.Log("Distance: " + distance);

                    // Check if outside allowed range
                    if(distance > 1 + delta || distance < 1 - delta) 
                    {
                        Debug.Log("i: " + i + ", Distance: " + distance);
                        allLidarPointsInsideSameRadius = false;
                        //break;
                    }
                }

                UnityEngine.Assertions.Assert.AreEqual(true, allLidarPointsInsideSameRadius);

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
