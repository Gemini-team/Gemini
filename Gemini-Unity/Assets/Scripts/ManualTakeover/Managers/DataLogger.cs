using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// Writes relevant measurements to a csv file at regular intervals
/// </summary>
public class DataLogger : MonoBehaviour {
    private const string DIRECTORY = "./data/";
    private readonly string[] HEADER = new string[] { "time", "remainingDistance", "speed", "position", "direction", "linearInput", "angularInput", "manualControl", "camera" };
    private const string SEPARATOR = ";";

    private Scenario scenario;
	private CameraController cameraController;
    StreamWriter sw;
    private bool open = false;
    private float startTime;
    private Vector3 startPos;

    public float interval = 1;

    private void Start() {
		cameraController = FindObjectOfType<CameraController>();
        scenario = FindObjectOfType<Scenario>();

		// TODO: Consider beginning measurements on manual takeover instead
		scenario.OnPlay.AddListener(BeginMeasuring);
        scenario.OnCompletion.AddListener(TryStopMeasuring);

        if (!Directory.Exists(DIRECTORY)) {
            Directory.CreateDirectory(DIRECTORY);
        }

        if (interval <= 0) {
            Debug.LogWarning("Consider using a non-zero interval for data collection");
        }
    }

    private void WriteRow(object[] values) {
        sw.WriteLine(string.Join(SEPARATOR, values));
    }

    // Precondition: open == true
    private IEnumerator TakeRegularMeasurements() {
        while (open) {
			object[] data = new object[] {
				Time.time - startTime,
				scenario.Ferry.RemainingDistance,
				scenario.Ferry.Speed,
				scenario.Ferry.transform.position - startPos,
				scenario.Ferry.transform.forward,
				scenario.Ferry.input,
				scenario.Ferry.rudder,
				scenario.Ferry.ManualControl ? "1" : "0",
				cameraController.MountI.GetValueOrDefault(0) + 1
            };
            WriteRow(data);

            yield return new WaitForSeconds(interval);
        }
    }

    private void BeginMeasuring() {
        TryStopMeasuring();

        startPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        string scenarioInfo = scenario.ScenarioName;
        if (PlayerPrefs.GetInt("ObjectDetection") == 1) scenarioInfo += " OD";
        if (PlayerPrefs.GetInt("AIDecisionSupport") == 1) scenarioInfo += " AI";

        sw = new StreamWriter(DIRECTORY + System.DateTime.Now + " " + scenarioInfo + ".csv");
        WriteRow(HEADER);

        startTime = Time.time;
        open = true;

        StartCoroutine(TakeRegularMeasurements());
    }

    private void TryStopMeasuring() {
        if (open) sw.Close();
        open = false;
    }

    private void OnDisable() {
        TryStopMeasuring();
    }

    private void OnApplicationQuit() {
        TryStopMeasuring();
    }
}
