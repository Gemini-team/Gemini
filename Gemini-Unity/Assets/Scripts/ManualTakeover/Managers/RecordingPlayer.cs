using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RecordingPlayer : MonoBehaviour {
    [HideInInspector]
    public UnityEvent OnLoadRecording;

    public Transform[] recordedObjects;

    [HideInInspector]
    public bool playing = false;

    public float Duration => frames.Last().time;


    private float time;
    public float Elapsed {
        get => time;
        set {
            time = value;

            while (index > 0 && time < frames[index].time) index--;
            while (index < frames.Count - 1 && time > frames[index + 1].time) index++;
        }
    }

    private int index;
    private List<(float time, bool manualControl, Vector3[] data)> frames = new List<(float, bool, Vector3[])>();


    public void Load(string path) {
        Debug.Log("Loading recording from path: " + PlayerPrefs.GetString("RecordingPath"));
        StreamReader sr = new StreamReader(PlayerPrefs.GetString("RecordingPath"));

        sr.ReadLine();  // Ignore header
        while (!sr.EndOfStream) {
            string[] vals = sr.ReadLine().Split(DataLogger.SEPARATOR);

            frames.Add((
                time: DataLogger.Formatter.ParseFloat(vals[0]),
                manualControl: int.Parse(vals[1]) == 1,
                data: (from string val in vals.Skip(2) select DataLogger.Formatter.ParseVector3(val)).ToArray()
            ));
        }

        playing = true;
    }

    public void SetTime(float time) {
        
    }

    private void Update() {
        // Check if playback can continue
        if (!playing || index >= frames.Count - 1) {
            return;
        }

        var f0 = frames[index];
        var f1 = frames[index + 1];
        float t = (time - f0.time) / (f1.time - f0.time);

        for (int i = 0; i < recordedObjects.Length; i++) {
            recordedObjects[i].position = Vector3.Lerp(f0.data[i * 2], f1.data[i * 2], t);
            recordedObjects[i].rotation = Quaternion.Lerp(Quaternion.Euler(f0.data[i * 2 + 1]), Quaternion.Euler(f1.data[i * 2 + 1]), t);
        }

        Elapsed = time + Time.deltaTime;  // Need to assign to Elapsed, so that the index is updated accordingly
    }
}
