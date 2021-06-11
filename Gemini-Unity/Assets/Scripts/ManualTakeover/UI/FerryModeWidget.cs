using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FerryModeWidget : MonoBehaviour {
    private Color AutomaticModeColor => new Color(0, 0.8f, 0, 1);
    private Color ManualModeColor => new Color(1, 0.25f, 0, 1);

    private void Start() {
        Text label = transform.Find("Text").GetComponent<Text>();
        Image image = GetComponent<Image>();

        FerryController ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        ferry.OnControlChange.AddListener(() => {
            image.color = ferry.ManualControl ? ManualModeColor : AutomaticModeColor;
            label.text = ferry.ManualControl ? "MANUAL CONTROL ENGAGED" : "AUTOPILOT ENGAGED";
        });
    }
}
