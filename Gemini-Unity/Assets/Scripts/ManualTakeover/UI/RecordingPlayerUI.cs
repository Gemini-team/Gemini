using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordingPlayerUI : MonoBehaviour {
    private RecordingPlayer player;
    private Slider slider;
    private Text label;

    private string endTimeStamp;

    private string CreateTimestamp(float time) {
        System.TimeSpan ts = System.TimeSpan.FromSeconds(time);
        return string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
    }

    private void Start() {
        player = FindObjectOfType<RecordingPlayer>();
        slider = GetComponentInChildren<Slider>();
        label = GetComponentInChildren<Text>();

        player.Load(PlayerPrefs.GetString("RecordingPath"));

        slider.minValue = 0;
        slider.maxValue = player.Duration;
        slider.onValueChanged.AddListener(value => {
            player.Elapsed = value;
        });

        endTimeStamp = CreateTimestamp(player.Duration);
    }

    private void Update() {
        slider.value = player.Elapsed;
        label.text = CreateTimestamp(player.Elapsed) + " / " + endTimeStamp;
    }
}
