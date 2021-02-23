using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Gemini.Core;
using Gemini.EMRS.Core;
using UnityEngine.Rendering;

public class NavClient : Sensor
{

    private TcpClient _tcpClient;
    private string _serverIP = "192.168.80.128";
    private int _portNr = 12346;

    private StreamReader _streamReader;
    private StreamWriter _streamWriter;

    private BaseMessage _baseMessage;


    class BaseMessage
    {
        public string msg_type;
        public float TOD;
        public string data;

        public BaseMessage(string msg_type, float TOD, string data)
        {
            this.msg_type = msg_type;
            this.TOD = TOD;
            this.data = data;
        }

    }

    class Data
    {
        public string header;
        public string child_frame_id;
        public string pose;
        public string velocity;


        public Data(string header, string child_frame_id, string pose, string velocity)
        {
            this.header = header;
            this.child_frame_id = child_frame_id;
            this.pose = pose;
            this.velocity = velocity;
        }

    }

    class Header
    {
        public string frame_id;
        public string stamp;

        public Header(string frame_id, string stamp)
        {
            this.frame_id = frame_id;
            this.stamp = stamp;
        }
    }

    class Pose
    {
        public Vector3 position;
        public Quaternion orientation;

        public Pose(Vector3 position, Quaternion orientation)
        {
            this.position = position;
            this.orientation = orientation;
        }
    }

    class Velocity
    {
        public Vector3 linear;
        public Vector3 angular;

        public Velocity(Vector3 linear, Vector3 angular)
        {
            this.linear = linear;
            this.angular = angular;
        }
    }

    private void Awake()
    {
        SetupSensorCallbacks(new SensorCallback(NavUpdate, SensorCallbackOrder.Last)); 
    }

    void NavUpdate(ScriptableRenderContext context, Camera[] cameras)
    {
        _baseMessage = new BaseMessage(
            "nav",
            (float)OSPtime,
            JsonUtility.ToJson(new Data(
                JsonUtility.ToJson(new Header("piren", "" + OSPtime)),
                "velodyne",
                JsonUtility.ToJson(
                    new Pose(ConventionTransforms.TranslationUnityToNED(gameObject.transform.position),
                    ConventionTransforms.QuaternionUnityToNED(gameObject.transform.rotation))),
                    JsonUtility.ToJson(new Velocity(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f)))
            )));

        // TODO: Do not create a new message everytime we send.
        /*
         * 
        _baseMessage = new BaseMessage(
            JsonUtility.ToJson(new Header("piren", "" + OSPtime)),
            "velodyne",
            JsonUtility.ToJson(
                new Pose(ConventionTransforms.TranslationUnityToNED(gameObject.transform.position),
                ConventionTransforms.QuaternionUnityToNED(gameObject.transform.rotation))), 
                JsonUtility.ToJson(new Velocity(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, 0.0f, 0.0f))));
        */

        gate = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tcpClient = new TcpClient();
        _tcpClient.Connect(_serverIP, _portNr);

        _streamReader = new StreamReader(_tcpClient.GetStream(), Encoding.ASCII);
        _streamWriter = new StreamWriter(_tcpClient.GetStream(), Encoding.ASCII);
    }

    public override bool SendMessage()
    {
        Debug.Log("Sending Nav message!");
        _streamWriter.WriteLine(JsonUtility.ToJson(_baseMessage));
        _streamWriter.Flush();
        return true;
    }

    private void OnDestroy()
    {
        // Cleanup 
        _streamWriter.Close();
        _tcpClient.Close();
    }

}
