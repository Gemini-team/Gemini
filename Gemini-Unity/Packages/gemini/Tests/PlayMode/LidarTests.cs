using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

using Gemini.EMRS.Lidar;

namespace Tests
{


    public class LidarTest
    {

       // public void Setup()
       // {
       //     //EditorSceneManager.OpenScene("Assets/Scenes/Test.unity");
       //     //EditorSceneManager.OpenScene("C:\\Users\\ThomasSkarshaug\\Dev\\Simulation\\Gemini-team\\Gemini\\Gemini-Unity\\Packages\\gemini\\Runtime\\Gemini\\Examples");
       // }


        [OneTimeSetUp]
        public void LoadScene()
        {
            SceneManager.LoadScene("Test");
        }
//
//        [OneTimeTearDown]
//        public void TearDown()
//        {
//            SceneManager.UnloadSceneAsync("Test");
//        }



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
        bool arrayNonZero = false;
        //int numNonZeroBytes = 0;
        public bool IsTestFinished
        {
            get 
            { 
                //Debug.Log("lidarArray length: " + lidar.lidarDataByte.array.Length);

                //for (int i = 0; i < lidar.lidarDataByte.array.Length; i++)
                //{
                //    if (lidar.lidarDataByte.array[i] > 0.2)
                //    {
                //        numNonZeroBytes++;
                //        arrayNonZero = true;
                //    }
                //}

                //Debug.Log("nonZero bytes number: " + numNonZeroBytes);

                Assert.AreEqual("test_frame_id", lidar.FrameId);

                //Assert.AreEqual(true, arrayNonZero);

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
            //Debug.Log("frameCount: " + frameCount);
        }
    }
}
