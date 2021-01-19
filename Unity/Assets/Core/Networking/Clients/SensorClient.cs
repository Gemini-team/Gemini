using Google.Protobuf;
using Grpc.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Gemini.EMRS.Lidar;
using Sensorstreaming;

public class SensorClient : MonoBehaviour
{

    //public static string serverIP = "172.18.106.219";
    public static string serverIP = "192.168.0.116";

    public static int serverPort = 30052;

    private RenderTexture _renderTexture;

    public RenderTexture RenderTexture
    {
        get => _renderTexture;
        set => _renderTexture = value;
    }
    
    private readonly ByteString _data = ByteString.CopyFromUtf8("");

    public ByteString Data
    {
        get => _data;
    }

    //private Camera camera;

    private static Channel channel = new Channel(serverIP + ":" + serverPort, ChannelCredentials.Insecure);
    private Sensorstreaming.SensorStreaming.SensorStreamingClient client = new SensorStreaming.SensorStreamingClient(channel);

    // Start is called before the first frame update
    void Start()
    {


        // TODO: Clean this up
        /*
            if (gameObject.GetComponent<Camera>() != null)
            {
                camera = gameObject.GetComponent<Camera>();
                _renderTexture = camera.targetTexture;
            }

            if ( _renderTexture == null)
            {
                _renderTexture = new RenderTexture(800, 640, 24, RenderTextureFormat.Default, 0);
                camera.targetTexture = _renderTexture;
            } 
        */
    }

    // Update is called once per frame
    void Update()
    {
        //System.Threading.Thread.Sleep(1000);
    }


    public bool StreamCameraData(ByteString data, int dataLength, UInt32 TimeStamp)
    {
        bool success = client.StreamCameraSensor(new CameraStreamingRequest { Data = data, TimeStamp = 0 }).Success;

        if (success)
            return true;

        return false;

    }

    public bool StreamLidarData(LidarMessage lidarMessage)
    {

        LidarStreamingRequest lidarStreamingRequest = new LidarStreamingRequest();

        lidarStreamingRequest.TimeInSeconds = lidarMessage.timeInSeconds;
        lidarStreamingRequest.Height = lidarMessage.height;
        lidarStreamingRequest.Width = lidarMessage.width;

        Gemini.EMRS.Lidar.PointField[] fields = lidarMessage.fields;
        Sensorstreaming.PointField[] pointFields = new Sensorstreaming.PointField[fields.Length];
        for (int i = 0; i < fields.Length; i++)
        {
            Sensorstreaming.PointField tempPointField = new Sensorstreaming.PointField();
            tempPointField.Name = fields[i]._name;
            tempPointField.Offset = fields[i]._offset;
            tempPointField.Datatype = fields[i]._datatype;
            tempPointField.Count = fields[i]._count;

            lidarStreamingRequest.Fields.Add(tempPointField);

            //pointFields.SetValue(tempPointField, i);

        }

        lidarStreamingRequest.IsBigEndian = lidarMessage.is_bigendian;
        lidarStreamingRequest.PointStep = lidarMessage.point_step;
        lidarStreamingRequest.RowStep = lidarMessage.row_step;
        lidarStreamingRequest.Data = ByteString.CopyFrom(lidarMessage.data);
        lidarStreamingRequest.IsDense = lidarMessage.is_dense;

        // Setting the LidarFields are bit more involved.
        //Vec3 tempPosition;
        //LidarField tempLidarField;

        /*
        foreach (LidarScript.LidarFieldData lidarField in lidarMessage.data)
        {
            tempPosition = new Vec3();
            tempPosition.X = lidarField.position.x;
            tempPosition.Y = lidarField.position.y;
            tempPosition.Z = lidarField.position.z;

            tempLidarField = new LidarField();
            tempLidarField.Position = tempPosition;
            tempLidarField.Intensity = lidarField.intensity;
            tempLidarField.Ring = lidarField.ring;
            tempLidarField.Time = lidarField.time;

            lidarStreamingRequest.LidarFields.Add(tempLidarField);
        };
        */

        lidarStreamingRequest.IsDense = lidarMessage.is_dense;

        bool success = client.StreamLidarSensor(lidarStreamingRequest).Success;

        if (success)
            return true;

        return false;

    }

    /*
    public void OnEnable()
    {
        RenderPipelineManager.endFrameRendering += EndFrameRendering;
    }

    public void OnDisable()
    {
        RenderPipelineManager.endFrameRendering -= EndFrameRendering;
    }



    void EndFrameRendering(ScriptableRenderContext context, Camera[] cameras)
    {

        AsyncGPUReadback.Request(camera.activeTexture, 0, TextureFormat.RGB24, ReadbackCompleted);

    }

    void ReadbackCompleted(AsyncGPUReadbackRequest request)
    {
        Debug.Log("picture");
        ByteString data = ByteString.CopyFrom(request.GetData<byte>().ToArray());
        var success = client.StreamCameraSensor(new CameraStreamingRequest { Data = data, DataLength = 0, TimeStamp = 0 });
    }
    */

}
