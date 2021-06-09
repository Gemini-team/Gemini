using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ETAWidget : MonoBehaviour {
    private const float UPDATE_INTERVAL = 5, AVG_SPEED = 5;

    private FerryController ferry;
    private Text text;


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
                // Pseudo-approximation of ETA
                DateTime eta = DateTime.Now.AddSeconds(ferry.RemainingDistance / AVG_SPEED);
                text.text = "ETA: " + string.Format("{0:00}:{1:00}", eta.Hour, eta.Minute);
            }
            yield return new WaitForSeconds(UPDATE_INTERVAL);
        }
    }
}
