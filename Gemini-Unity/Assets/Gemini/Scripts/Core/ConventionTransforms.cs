
using UnityEngine;

namespace Gemini.Core
{
    public static class ConventionTransforms
    {
        private static Vector3 TranslationUnityToNED(Vector3 vec)
        {
            return new Vector3(vec.z, vec.x, -vec.y);
        }

        private static Vector3 TranslationNEDToUnity(Vector3 vec)
        {
            return new Vector3(vec.y, -vec.z, vec.x);
        }

        private static Vector3 RotationUnityToNED(Vector3 angleVec)
        {
            return new Vector3(-angleVec.z, -angleVec.x, angleVec.y);
        }

        private static Vector3 RotationNEDToUnity(Vector3 angleVec)
        {
            return new Vector3(-angleVec.y, angleVec.z, -angleVec.x);
        }

        private static Quaternion QuaternionNEDToUnity(Quaternion quaternionVec)
        {
            return new Quaternion(
                quaternionVec.y,
                quaternionVec.w,
                quaternionVec.z,
                quaternionVec.x);
        }

        private static Quaternion QuaternionUnityToNED(Quaternion quaternionVec)
        {
            return Quaternion.Inverse(QuaternionNEDToUnity(quaternionVec));

        }

        public static Vector3 ForceUnityToNED(Vector3 force)
        {
            return TranslationUnityToNED(force);
        }

        public static Vector3 ForceNEDToUnity(Vector3 force)
        {
            return TranslationNEDToUnity(force);
        }

        public static Vector3 TorqueUnityToNED(Vector3 torque)
        {
            return RotationUnityToNED(torque);
        }

        public static Vector3 TorqueNEDToUnity(Vector3 torque)
        {
            return RotationNEDToUnity(torque);
        }

        public static Vector3 PositionUnityToNED(Vector3 position)
        {
            return TranslationUnityToNED(position);
        }

        public static Vector3 PositionNEDToUnity(Vector3 position)
        {
            return TranslationNEDToUnity(position);
        }

        public static Vector3 VelocityUnityToNED(Vector3 velocity)
        {
            return TranslationUnityToNED(velocity);
        }

        public static Vector3 VelocityNEDToUnity(Vector3 velocity)
        {
            return TranslationNEDToUnity(velocity);
        }

        public static Vector3 AngularVelocityUnityToNED(Vector3 angularVelocity)
        {
            return RotationUnityToNED(angularVelocity);
        }

        public static Vector3 AngularVelocityNEDToUnity(Vector3 angularVelocity)
        {
            return RotationNEDToUnity(angularVelocity);
        }

        public static Vector3 EulerOrientationUnityToNED(Vector3 orientation)
        {
            return RotationUnityToNED(orientation);
        }

        public static Vector3 EulerOrientationNEDToUnity(Vector3 orientation)
        {
            return RotationNEDToUnity(orientation);
        }
        
        public static Quaternion QuaternionOrientationUnityToNED(Quaternion orientation)
        {
            return QuaternionUnityToNED(orientation);
        }

        public static Quaternion QuaternionOrientationNEDToUnity(Quaternion orientation)
        {
            return QuaternionNEDToUnity(orientation);
        }
    }
}
