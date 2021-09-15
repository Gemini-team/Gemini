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

        public CameraFrustum(int WidthRes, int HeightRes, float farPlane, float nearPlane, float horisontalAngle, float verticalAngle, float lidarVerticalAngle)
        {
            _horisontalAngle = horisontalAngle;
            _verticalAngle = verticalAngle;
            _verticalSideAngles = lidarVerticalAngle;

            _farPlane = farPlane;
            _nearPlane = nearPlane;

            _pixelHeight = HeightRes;
            _pixelWidth = WidthRes;

            _aspectRatio = Mathf.Tan(_horisontalAngle / 2) / Mathf.Tan(_verticalAngle / 2);

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