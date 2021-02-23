using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerManualTakeover : MonoBehaviour {
    [Multiline(10)]
    public string message;

    private BoatController ferry;
    private UIManager ui;

    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<BoatController>();
        ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (active) return;
        active = true;
        ferry.manualControl = true;
        ui.Alert(message, 10);
        Debug.Log("Manual takeover activated");
    }
}
