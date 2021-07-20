using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ConventionTransformsTest
    {
        [Test]
        public void ForceUnityToNEDTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(3.0f, 1.0f, -2.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.ForceUnityToNED(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void ForceNEDToUnityTest()
        {

            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(2.0f, -3.0f, 1.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.ForceNEDToUnity(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void TorqueUntiyToNEDTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(-3.0f, -1.0f, 2.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.TorqueUnityToNED(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void TorqueNEDToUnityTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(-2.0f, 3.0f, -1.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.TorqueNEDToUnity(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void PositionUnityToNEDTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(3.0f, 1.0f, -2.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.PositionUnityToNED(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void PositionNEDToUnityTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(2.0f, -3.0f, 1.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.PositionNEDToUnity(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void VelocityUnityToNED()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(3.0f, 1.0f, -2.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.VelocityUnityToNED(testVec);

            Assert.AreEqual(expectedVec, actualVec);

        }

        [Test]
        public void VelocityNEDToUnity()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(2.0f, -3.0f, 1.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.VelocityNEDToUnity(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void AngularVelocityUnityToNEDTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(-3.0f, -1.0f, 2.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.AngularVelocityUnityToNED(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void AngularVelocityNEDToUnityTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(-2.0f, 3.0f, -1.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.AngularVelocityNEDToUnity(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void EulerOrientationUnityToNEDTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(-3.0f, -1.0f, 2.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.EulerOrientationUnityToNED(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }

        [Test]
        public void EulerOrientationNEDToUnityTest()
        {
            Vector3 testVec = new Vector3(1.0f, 2.0f, 3.0f);    
            Vector3 expectedVec = new Vector3(-2.0f, 3.0f, -1.0f);

            Vector3 actualVec = Gemini.Core.ConventionTransforms.EulerOrientationNEDToUnity(testVec);

            Assert.AreEqual(expectedVec, actualVec);
        }
    }
}
