using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FerryModeWidget : MonoBehaviour {
    private void Start() {
        Text label = transform.Find("Text").GetComponent<Text>();
        Image image = GetComponent<Image>();

        Scenario scenario = FindObjectOfType<Scenario>();
        FerryController ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        ferry.OnReceivedInput.AddListener(() => {
            if (ferry.ManualControl) {
                image.color = UIColors.Alert;
                label.text = "MANUAL CONTROL ENGAGED";
            }
        });
        ferry.OnControlChange.AddListener(() => {
            if (ferry.ManualControl) {
                image.color = scenario.ManualTakeoverRequired ? UIColors.Alert : UIColors.Warning;
                label.text = scenario.ManualTakeoverRequired ? "MANUAL CONTROL ENGAGED" : "FERRY MANUALLY STOPPED";
            } else {
                image.color = UIColors.Positive;
                label.text = "AUTOPILOT ENGAGED";
            }
        });
    }
}
