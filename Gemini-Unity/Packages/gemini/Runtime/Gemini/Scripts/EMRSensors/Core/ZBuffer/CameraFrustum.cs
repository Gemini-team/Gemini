using UnityEngine;
using System.Collections;

using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

using Unity.Collections;
using UnityEngine.Rendering;


namespace Gemini.EMRS.Core.ZBuffer
{
    public class CameraFrustum
    {
        public int _pixelWidth { get; }
        public int _pixelHeight { get; }
        public float _farPlane { get; }
        public float _nearPlane { get; }
        public float _verticalAngle { get; }
        public float _horisontalAngle { get; }
        public float _aspectRatio { get; }
        public float _verticalSideAngles { get; }
        public Matrix4x4 _cameraMatrix { get; }

        /* 
            Note that the frustum is a projected square, by extension its effective 
            V_FOV is dependent on which horizontal angle you're looking at.

            E.g. at the center of a frustum, the effective V_FOV is equal to the
            frustum V_FOV, while it gets smaller towards the edges.

            In order to correct for this, we need to set a larger frustum V_FOV such that
            the effective V_FOV at the intersections between frustums (i.e. angle from center equal to H_FOV/2)
            is equal to the configured lidar V_FOV. This correction is found as: 
            tan(CORRECTED_FRUSTUM_V_FOV/2) = tan(LIDAR_V_FOV/2)/cos(H_FOV/2).
            See Kjetils thesis: https://folk.ntnu.no/edmundfo/msc2020-2021/vassteinMSc.pdf equation 4.17 of page 42. 
        */

        public CameraFrustum(int pixelWidth, int pixelHeight, float farPlane, float nearPlane, float focalLengthMilliMeters, float pixelSizeInMicroMeters)
        {
            _pixelWidth = pixelWidth;
            _pixelHeight = pixelHeight;
            _farPlane = farPlane;
            _nearPlane = nearPlane;
            _verticalAngle = 2 * Mathf.Atan((float)pixelHeight * pixelSizeInMicroMeters * Mathf.Pow(10, -3) / (2 * focalLengthMilliMeters));
            Debug.Log("_VerticalAngle" + _verticalAngle.ToString() + " power " + ((float)pixelHeight).ToString());

            _aspectRatio = (float)_pixelWidth / (float)_pixelHeight;
            _horisontalAngle = 2 * Mathf.Atan(_aspectRatio * Mathf.Tan(_verticalAngle / 2));
            _verticalSideAngles = 2 * Mathf.Atan(Mathf.Cos(_horisontalAngle / 2) * Mathf.Tan(_verticalAngle / 2));
            _cameraMatrix = new Matrix4x4();
            _cameraMatrix = MakeCameraMatrix(_aspectRatio, _verticalAngle, farPlane, nearPlane);
        }

        public CameraFrustum(int WidthRes, float farPlane, float nearPlane, float horisontalAngle, float lidarVerticalAngle)
        {
            _horisontalAngle = horisontalAngle;
            _verticalAngle = 2f * Mathf.Atan(Mathf.Tan(lidarVerticalAngle / 2f) / Mathf.Cos(horisontalAngle / 2f));
            _verticalSideAngles = lidarVerticalAngle;

            _farPlane = farPlane;
            _nearPlane = nearPlane;

            _aspectRatio = Mathf.Tan(_horisontalAngle / 2) / Mathf.Tan(_verticalAngle / 2);

            _pixelHeight = (int)Mathf.Ceil((float)WidthRes / _aspectRatio);
            _pixelWidth = WidthRes;

            _cameraMatrix = new Matrix4x4();
            _cameraMatrix = MakeCameraMatrix(_aspectRatio, _verticalAngle, farPlane, nearPlane);
        }


        private Matrix4x4 MakeCameraMatrix(float a, float VFOV, float f, float n)
        {
            float P_2 = 1 / Mathf.Tan(VFOV / 2);
            float P_1 = P_2 / a;
            float P_3 = -(f + n) / (f - n);
            float P_4 = -2 * f * n / (f - n);

            Vector4 colum_1 = new Vector4(P_1, 0, 0, 0);
            Vector4 colum_2 = new Vector4(0, P_2, 0, 0);
            Vector4 colum_3 = new Vector4(0, 0, P_3, -1);
            Vector4 colum_4 = new Vector4(0, 0, P_4, 0);

            return new Matrix4x4(colum_1, colum_2, colum_3, colum_4);
        }
    }
}