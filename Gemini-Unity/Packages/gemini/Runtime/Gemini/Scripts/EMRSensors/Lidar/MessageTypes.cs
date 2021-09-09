using System;
using UnityEngine;
using System.Collections;


namespace Gemini.EMRS.Lidar
{

    public struct PointField
    {
        public byte INT8;
        public byte UINT8;
        public byte INT16;
        public byte UINT16;
        public byte INT32;
        public byte UINT32;
        public byte FLOAT32;
        public byte FLOAT64;

        public string _name;
        public uint _offset;
        public byte _datatype;
        public uint _count;

        public PointField(string name, uint offset, byte datatype, uint count)
        {
            _name = name;
            _offset = offset;
            _datatype = datatype;
            _count = count;

            INT8 = 1;
            UINT8 = 2;
            INT16 = 3;
            UINT16 = 4;
            INT32 = 5;
            UINT32 = 6;
            FLOAT32 = 7;
            FLOAT64 = 8;
        }
    }

    public struct LidarMessage
    {
        public double timeInSeconds;
        public uint height;
        public uint width;
        public PointField[] fields;
        public bool is_bigendian;
        public uint point_step;        // Length of a point in bytes
        public uint row_step;          // Length of a row in bytes
        public byte[] data;            // Actual point data, size is (row_step*height)
        public bool is_dense;          // True if there are no invalid points
        public LidarMessage(int nrOfElements, double timeStep, byte[] lidarFields)
        {
            timeInSeconds = timeStep;
            height = 1;
            width = (uint)nrOfElements;
            fields = new PointField[6];
            fields[0] = new PointField("x", 0, 7, 1);
            fields[1] = new PointField("y", 4, 7, 1);
            fields[2] = new PointField("z", 8, 7, 1);
            fields[3] = new PointField("intensity", 12, 7, 1);
            fields[4] = new PointField("ring", 16, 7, 1);       // NB! Lidar velodyne has datatype 4
            fields[5] = new PointField("time", 20, 7, 1);
            is_bigendian = false;
            point_step = 24;
            row_step = 0;
            data = lidarFields;
            is_dense = false;
        }

        public Vector4[] ParseLidarPoints()
        {
            int offset = 24;
            Vector4[] points = new Vector4[data.Length / offset]; 
            byte[] slice = new byte[16];

            int pointIndex = 0;

            for (int i = 0; i < data.Length; i++)
            {
                if (i % offset == 0)
                {
                    points[pointIndex] = new Vector4();

                    // TODO: Need some check
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    float intensity = 0;

                    if (i + 4 < data.Length)
                        x = BitConverter.ToSingle(data, i);
                    if (i + 8 < data.Length)
                        y = BitConverter.ToSingle(data, i + 4);
                    if (i + 12 < data.Length)
                        z = BitConverter.ToSingle(data, i + 8);
                    if (i + 16 < data.Length)
                        intensity = BitConverter.ToSingle(data, i + 12);

                    points[pointIndex].x = x;
                    points[pointIndex].y = y;
                    points[pointIndex].z = z;
                    points[pointIndex].w = intensity;

                    pointIndex++;
                }
            }
            return points;
        }
    }
}