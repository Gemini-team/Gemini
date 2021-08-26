using System.Collections;
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
        bool arrayNonZero = false;
        //int numNonZeroBytes = 0;
        public bool IsTestFinished
        {
            get 
            { 
                Assert.AreEqual("test_frame_id", lidar.FrameId);

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
