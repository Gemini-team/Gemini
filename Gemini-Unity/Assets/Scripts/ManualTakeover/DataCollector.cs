using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Writes relevant measurements to a csv file at regular intervals
/// </summary>
public class DataCollector : MonoBehaviour {
    private const string DIRECTORY = "./data/";
    private readonly string[] HEADER = new string[] { "time", "distanceToDock", "speed", "position", "direction" };
    private const string SEPARATOR = ";";

    private Scenario scenario;
    StreamWriter sw;
    private bool open = false;
    private float startTime;

    public float interval = 1;

    private void Start() {
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
    private IEnumerator TakeMeasurements() {
        float prevDist = -1;
        
        while (open) {
            scenario.Ferry.ClosestDock(out float dist);

            object[] data = new object[] {
                Time.time - startTime,
                dist,
                prevDist < 0 ? 0 : (prevDist - dist) / interval,
                scenario.Ferry.transform.position,
                transform.forward
            };
            WriteRow(data);

            yield return new WaitForSeconds(interval);
            prevDist = dist;
        }
    }

    private void BeginMeasuring() {
        TryStopMeasuring();
        
        sw = new StreamWriter(DIRECTORY + System.DateTime.Now + ".csv");
        WriteRow(HEADER);

        startTime = Time.time;
        open = true;

        StartCoroutine(TakeMeasurements());
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
