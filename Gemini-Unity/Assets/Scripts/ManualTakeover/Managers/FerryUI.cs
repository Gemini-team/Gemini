using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FerryUI : ExtendedMonoBehaviour {
    private int ALERT_DURATION = 4, IMPORTANT_ALERT_DURATION = 15;

    private Scenario scenario;
    private FerryController ferry;
    private Text alertMessage, distanceGauge;

    private void Alert(string message, float? duration=null, Color? color=null) {
        Toggle("AlertBox", true);
        alertMessage.text = message;
        alertMessage.color = color.GetValueOrDefault(Color.white);
        if (duration.HasValue) Schedule(() => Toggle("AlertBox", false), duration.Value);
    }

    private void Toggle(string name, bool state) {
        transform.Find(name).gameObject.SetActive(state);
    }

    private void Start() {
        alertMessage = transform.Find("AlertBox/Text").GetComponent<Text>();
        distanceGauge = transform.Find("Dashboard/Distance/Value").GetComponent<Text>();

        Toggle("AlertBox", false);
        Toggle("ManualIndicator", false);

        scenario = FindObjectOfType<Scenario>();
        scenario.OnPlay.AddListener(() => Alert("Autopilot engaged", ALERT_DURATION));

        ferry = FindObjectOfType<FerryController>();
        ferry.OnControlChange.AddListener(() => {
            if (ferry.ManualControl) {
                Alert("Manual takeover required\nDock at " + ferry.DestinationDock.name, duration: IMPORTANT_ALERT_DURATION, color: Color.red);
            }
            Toggle("ManualIndicator", ferry.ManualControl);
        });
        ferry.DockMessage.AddListener(msg => {
            Alert(msg, ALERT_DURATION);
        });

        PassengerBoarder boarder = ferry.GetComponent<PassengerBoarder>();
        boarder.OnBoarding.AddListener(() => Alert("Boarding passengers"));
        boarder.OnBoardingCompleted.AddListener(() => Alert("Boarding completed", ALERT_DURATION));
    }

    private void Update() {
        distanceGauge.text = ferry.AtDock == null ? Mathf.RoundToInt(ferry.RemainingDistance) + "m" : "0m (Docked)";
    }
}
