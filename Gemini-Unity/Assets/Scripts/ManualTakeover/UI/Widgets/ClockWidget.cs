using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClockWidget : MonoBehaviour {
    private Text text;

    private void Start() {
        text = GetComponent<Text>();
    }

    private void Update() {
        text.text = string.Format("{0:00}:{1:00}", System.DateTime.Now.Hour, System.DateTime.Now.Minute);
    }
}
