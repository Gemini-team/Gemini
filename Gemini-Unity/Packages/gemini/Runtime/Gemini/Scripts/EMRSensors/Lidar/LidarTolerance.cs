using System.Collections;
using UnityEngine;

using Gemini.EMRS.Core;
using Gemini.EMRS.Core.ZBuffer;
using Gemini.EMRS.PointCloud;

namespace Gemini.EMRS.Lidar
{
    static public class LidarTolerances
    {
        // pos is in a right handed [right, up, backwards] frame
        public static float euclidianError(Vector3 pos, CameraFrustum frustum, uint depthBufferSizeInBits)
        {
            // constants 
            float N_cd = Mathf.Pow(2f, depthBufferSizeInBits);
            float C_1 = N_cd * frustum._nearPlane * frustum._farPlane / (frustum._farPlane - frustum._nearPlane);
            float C_2 = 2f * C_1 * Mathf.Tan(frustum._verticalSideAngles / 2f) / (float)frustum._pixelHeight;

            Vector3 errorVec = pos - new Vector3(C_2, C_2, 0);

            return Mathf.Abs(-1f * pos.z / (C_1 + pos.z)) * errorVec.magnitude;
        }

        // Kjetil claims in his thesis that the worst case error is at this point,
        // i.e. the the far, bottom left corner of a frustum.
        // Given the symmetric nature of the frustum and the clip space coordinates
        // I would expect any of the far corners to produce the same error 
        private static Vector3 worstCasePos(CameraFrustum frustum)
        {
            return new Vector3(frustum._farPlane * Mathf.Tan(frustum._horisontalAngle / 2f), frustum._farPlane * Mathf.Tan(frustum._verticalSideAngles / 2f), frustum._farPlane) * (-1.0f);
        }

        public static float minHorizRes(float tol, CameraFrustum frustum, uint depthBufferSizeInBits)
        {
            // formulate as an equation of A*(C_2)^2 + B*C_2 + C = 0
            // solve for C_2
            // then solve vertical res as a function of C_2
            // then solve for horiz res as a function of vertical res

            // Find A, B, C
            float N_cd = Mathf.Pow(2f, depthBufferSizeInBits);
            float C_1 = N_cd * frustum._nearPlane * frustum._farPlane / (frustum._farPlane - frustum._nearPlane);
            Vector3 pos = worstCasePos(frustum);

            float A = 2;
            float B = -2f * (pos.x + pos.y);
            float C = pos.sqrMagnitude - Mathf.Pow(tol * Mathf.Abs((C_1 + pos.z) / pos.z), 2f);

            // find positive solution for C_2
            float C_2_pos = (Mathf.Sqrt(B * B - 4f * A * C) - B) / 2 * A;

            // find positive solution for N_c,h 
            float minVerticalRes = 2 * C_1 * Mathf.Tan(frustum._verticalSideAngles / 2f) / C_2_pos;

            // return N_c,w
            return Mathf.Ceil(frustum._aspectRatio * minVerticalRes);
        }

        // Note that this is a theoretical best-case for INFINITE camera resolution,
        // and is included mostly to have some sane bound for tolerance.
        public static float minAchieveableTol(CameraFrustum frustum, uint depthBufferSizeInBits)
        {
            // formulate as an equation of A*(C_2)^2 + B*C_2 + C = 0
            // solve for C in B^2 = 4*A*C 
            // solve for tol as a function of C

            // Find A, B, C
            float N_cd = Mathf.Pow(2f, depthBufferSizeInBits);

            float C_1 = N_cd * frustum._nearPlane * frustum._farPlane / (frustum._farPlane - frustum._nearPlane);

            Vector3 pos = worstCasePos(frustum);

            float A = 2;
            float B = -2f * (pos.x + pos.y);
            float C = B * B / (4 * A);

            return Mathf.Sqrt(pos.sqrMagnitude - C) / Mathf.Abs((C_1 + pos.z) / pos.z);
        }
    }
}
