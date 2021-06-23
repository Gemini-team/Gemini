using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FerryModeWidget : MonoBehaviour {
    private void Start() {
        Text label = transform.Find("Text").GetComponent<Text>();
        Image image = GetComponent<Image>();

        FerryController ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        ferry.OnControlChange.AddListener(() => {
            image.color = ferry.ManualControl ? UIColors.Warning : UIColors.PositiveInfo;
            label.text = ferry.ManualControl ? "MANUAL CONTROL ENGAGED" : "AUTOPILOT ENGAGED";
        });
    }
}
