
using UnityEngine;

namespace Gemini.Core
{
    public static class ConventionTransforms
    {

        //LINK: https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system
        public static Quaternion QuaternionNEDToUnity(Quaternion quaternionVec)
        {
            return new Quaternion(
                quaternionVec.y,
                -quaternionVec.z,
                -quaternionVec.x,
                quaternionVec.w);
        }
        
        public static Quaternion QuaternionUnityToNED(Quaternion quaternionVec)
        {
            return Quaternion.Inverse(QuaternionNEDToUnity(quaternionVec));
        }

        public static Vector3 ForceNEDToUnity(Vector3 force)
        {
            return new Vector3(force.y, -force.z, force.x);
        }

        public static Vector3 TorqueNEDToUnity(Vector3 torque)
        {
            return new Vector3(-torque.y, torque.z, -torque.x);
        }

        public static Vector3 TranslationUnityToNED(Vector3 vec)
        {
            return new Vector3(vec.z, vec.x, -vec.y);
        }

        public static Vector3 RotationUnityToNED(Vector3 angleVec)
        {
            return new Vector3(-angleVec.z, -angleVec.x, angleVec.y);
        }

    }
}
