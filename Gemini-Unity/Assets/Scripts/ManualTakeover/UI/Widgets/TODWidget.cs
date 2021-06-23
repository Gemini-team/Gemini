using System;
using UnityEngine;
using UnityEngine.UI;

public class TODWidget : MonoBehaviour {

    private void Start() {
        FerryController ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        Text text = GetComponent<Text>();

        ferry.OnDisconnectFromDock.AddListener(() => {
            text.text = "TOD: " + string.Format("{0:00}:{1:00}", DateTime.Now.Hour, DateTime.Now.Minute);
        });
        ferry.OnConnectToDock.AddListener(() => text.text = "TOD: --:--");

        // Assumes ferry starts docked
        text.text = "TOD: --:--";
    }
}
