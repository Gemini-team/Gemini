using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{

    public class LidarTest
    {
        [UnityTest]
        public IEnumerator LidarMonoBehaviourTest()
        {
            yield return new MonoBehaviourTest<LidarMonoBehaviourTest>();
        }

    }

    public class LidarMonoBehaviourTest : MonoBehaviour, IMonoBehaviourTest
    {
        private int frameCount;
        public bool IsTestFinished
        {
            get { return frameCount  == 1 ; }
        }

        void Update()
        {
            frameCount++;
            Debug.Log("frameCount: " + frameCount);
        }
    }
}
