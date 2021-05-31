using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FerryUI : ExtendedMonoBehaviour {
    private FerryController ferry;
    private Text alertMessage, distanceGauge;

    private void Alert(string message, float? duration=null) {
        alertMessage.text = message;
        if (duration.HasValue) Schedule(() => alertMessage.text = "", duration.Value);
    }

    private void Toggle(string name, bool state) {
        transform.Find(name).gameObject.SetActive(state);
    }

    private void Start() {
        alertMessage = transform.Find("AlertMessage").GetComponent<Text>();
        distanceGauge = transform.Find("Dashboard/Distance/Value").GetComponent<Text>();

        ferry = FindObjectOfType<FerryController>();
        ferry.OnControlChange.AddListener(() => {
            if (ferry.ManualControl) {
                Alert("Manual takeover required\nDock at " + ferry.DestinationDock.name, 15);
            }
            Toggle("ManualIndicator", ferry.ManualControl);
        });
    }

    private void Update() {
        distanceGauge.text = ferry.AtDock == null ? Mathf.RoundToInt(ferry.RemainingDistance) + "m" : "0m (Docked)";
    }
}
