using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MathTools;

public class ETAWidget : MonoBehaviour {
    private const float UPDATE_INTERVAL = 5;

    private FerryController ferry;
    private Text text;
    private readonly RollingAverage rollingSpeedAvg = new RollingAverage(count: 15, initialAverage: 0.6f);

    private void Start() {
        ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        text = GetComponent<Text>();

        ferry.OnConnectToDock.AddListener(() => text.text = "ETA: --:--");
        StartCoroutine(UpdateETA());

        // Assumes ferry starts docked
        text.text = "ETA: --:--";
    }

    private IEnumerator UpdateETA() {
        while (true) {
            if (ferry.AtDock == null && ferry.DestinationDock != null) {
                rollingSpeedAvg.PushValue(ferry.Speed);

                DateTime eta = DateTime.Now.AddSeconds(ferry.RemainingDistance / rollingSpeedAvg.Average);
                text.text = "ETA: " + string.Format("{0:00}:{1:00}", eta.Hour, eta.Minute);
            }
            yield return new WaitForSeconds(UPDATE_INTERVAL);
        }
    }
}
