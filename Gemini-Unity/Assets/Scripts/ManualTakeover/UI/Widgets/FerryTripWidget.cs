using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FerryTripWidget : MonoBehaviour {
    private void Start() {
        Text label = GetComponent<Text>();

        FerryController ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        ferry.OnConnectToDock.AddListener(() => {
            label.text = $"Ferry 1: {ferry.AtDock.name} to {ferry.DestinationDock.name}";
        });
    }
}
