using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Writes relevant measurements to a csv file at regular intervals
/// </summary>
public class DataLogger : MonoBehaviour {
    public class Formatter {
        private StreamWriter sw;
        private List<string> buffer = new List<string>();

        public Formatter(string path) {
            sw = new StreamWriter(path);
        }

        public static float ParseFloat(string s) => float.Parse(s.Replace(".", ","));

        public static Vector3 ParseVector3(string s) {
            string[] vals = s.Replace("(", "").Replace(")", "").Split(',');

            return new Vector3(
                ParseFloat(vals[0]),
                ParseFloat(vals[1]),
                ParseFloat(vals[2]));
        }

        private string FormatFloat(float f) => f.ToString().Replace(",", ".");

        public void PushLine() {
            sw.WriteLine(string.Join(SEPARATOR.ToString(), buffer));
            buffer.Clear();
        }

        public void Append(float f) => buffer.Add(FormatFloat(f));
        public void Append(Vector3 v) => buffer.Add($"({FormatFloat(v.x)},{FormatFloat(v.y)},{FormatFloat(v.z)})");
        public void Append(object o) => buffer.Add(o.ToString());

        public void Close() => sw.Close();
    }

    public static string ROOT_DIRECTORY => Path.GetFullPath("./data");
    public const char SEPARATOR = ';';
    private readonly string[] LOG_HEADER = new string[] { "time", "remainingDistance", "speed", "position", "direction", "linearInput", "angularInput", "manualControl", "camera" };
    private const float INTERVAL = 1;

    private Scenario scenario;
	private CameraController cameraController;
    Formatter swLog, swRecording = null;
    private bool open = false;
    private float startTime;
    private Vector3 startPos;

    public Transform[] recordObjects;

    private void Start() {
		cameraController = FindObjectOfType<CameraController>();
        scenario = FindObjectOfType<Scenario>();

		scenario.OnPlay.AddListener(BeginMeasuring);
        scenario.OnCompletion.AddListener(TryStopMeasuring);

        CreateDirectoryIfNeeded(ROOT_DIRECTORY);
    }

    private void CreateDirectoryIfNeeded(string path) {
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }
    }

    private Formatter CreateCSV(string rootPath, string name) => new Formatter(Path.Combine(rootPath, name + ".csv"));

    // Precondition: open == true
    private IEnumerator TakeRegularMeasurements() {
        while (open) {
            float time = Time.time - startTime;

            swLog.Append(scenario.Ferry.RemainingDistance);
            swLog.Append(scenario.Ferry.Speed);
            swLog.Append(scenario.Ferry.transform.position - startPos);
            swLog.Append(scenario.Ferry.transform.forward);
            swLog.Append(scenario.Ferry.input);
            swLog.Append(scenario.Ferry.rudder);
            swLog.Append(scenario.Ferry.ManualControl ? "1" : "0");
            swLog.Append(cameraController.MountI.GetValueOrDefault(0) + 1);

            swLog.PushLine();

            if (swRecording != null) {
                swRecording.Append(time);
                swRecording.Append(scenario.Ferry.ManualControl ? "1" : "0");
                foreach (Transform obj in recordObjects) {
                    swRecording.Append(obj.position);
                    swRecording.Append(obj.eulerAngles);
                }
                swRecording.PushLine();
            }

            yield return new WaitForSeconds(INTERVAL);
        }
    }

    private void BeginMeasuring() {
        TryStopMeasuring();

        startPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        string scenarioInfo = scenario.ScenarioName;
        if (PlayerPrefs.GetInt("ObjectDetection") == 1) scenarioInfo += "_OD";
        if (PlayerPrefs.GetInt("AIDecisionSupport") == 1) scenarioInfo += "_AI";

        // Manually specify datetime format, to ensure the string can be used as a path
        string rootPath = Path.Combine(ROOT_DIRECTORY, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_" + scenarioInfo);
        CreateDirectoryIfNeeded(rootPath);

        swLog = CreateCSV(rootPath, "log");
        foreach (string s in LOG_HEADER) swLog.Append(s);
        swLog.PushLine();

        if (recordObjects.Length > 0) {
            swRecording = CreateCSV(rootPath, "recording");
            
            swRecording.Append("time");
            swRecording.Append("manualControl");
            foreach (Transform obj in recordObjects) {
                swRecording.Append(obj.name + "Position");
                swRecording.Append(obj.name + "Rotation");
            }
            swRecording.PushLine();
        }

        startTime = Time.time;
        open = true;

        StartCoroutine(TakeRegularMeasurements());
    }

    private void TryStopMeasuring() {
        if (open) {
            swLog.Close();
            if (swRecording != null) swRecording.Close();
        }
        open = false;
    }

    private void OnDisable() {
        TryStopMeasuring();
    }

    private void OnApplicationQuit() {
        TryStopMeasuring();
    }
}
