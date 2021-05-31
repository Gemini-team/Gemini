﻿using System.Collections;
using System.IO;
using UnityEngine;

/// <summary>
/// Writes relevant measurements to a csv file at regular intervals
/// </summary>
public class DataCollector : MonoBehaviour {
    private const string DIRECTORY = "./data/";
    private readonly string[] HEADER = new string[] { "time", "remainingDistance", "movement", "position", "direction", "linearInput", "angularInput" };
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

        scenario.OnManualTakeover.AddListener(() => { 
            if (open) {
                // TODO: Include this time in the stored measurements
                Debug.Log("Manual takeover occurred at " + (Time.time - startTime));
            }
        });
    }

    private void WriteRow(object[] values) {
        sw.WriteLine(string.Join(SEPARATOR, values));
    }

    // Precondition: open == true
    private IEnumerator TakeRegularMeasurements() {
        float prevDist = -1;
        
        while (open) {
            float remainingDist = scenario.Ferry.RemainingDistance;
            object[] data = new object[] {
                Time.time - startTime,
                remainingDist,
                prevDist < 0 ? 0 : (prevDist - remainingDist) / interval,
                scenario.Ferry.transform.position,
                scenario.Ferry.transform.forward,
                scenario.Ferry.input,
                scenario.Ferry.rudder
            };
            WriteRow(data);

            yield return new WaitForSeconds(interval);
            prevDist = remainingDist;
        }
    }

    private void BeginMeasuring() {
        TryStopMeasuring();
        
        sw = new StreamWriter(DIRECTORY + System.DateTime.Now + ".csv");
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
