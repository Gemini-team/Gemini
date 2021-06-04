using System;
using UnityEngine;
using UnityEngine.UI;

public class FerryUI : ExtendedMonoBehaviour {
    private int ALERT_DURATION = 4, IMPORTANT_ALERT_DURATION = 15;

    private Scenario scenario;
    private FerryController ferry;
    private Text alertMessage, distanceGauge;

    private void Alert(string message, float? duration=null, Color? color=null) {
        Show("AlertBox");
        alertMessage.text = message;
        alertMessage.color = color.GetValueOrDefault(Color.white);
        if (duration.HasValue) Schedule(() => Hide("AlertBox"), duration.Value);
    }

	private void Toggle(string name, bool state) {
		transform.Find(name).gameObject.SetActive(state);
	}
	public void Show(string name) => Toggle(name, true);
	public void Hide(string name) => Toggle(name, false);

	private void Start() {
        alertMessage = transform.Find("AlertBox/Text").GetComponent<Text>();
        distanceGauge = transform.Find("Dashboard/Distance/Value").GetComponent<Text>();

        Hide("AlertBox");
		Hide("EndScreen");
		Hide("HelpScreen");

        scenario = FindObjectOfType<Scenario>();
        scenario.OnPlay.AddListener(() => {
			Alert("Autopilot engaged\nStandby", ALERT_DURATION);
		});
		scenario.OnCompletion.AddListener(() => {
			Transform endScreen = transform.Find("EndScreen");
			endScreen.gameObject.SetActive(true);

			TimeSpan ts = TimeSpan.FromSeconds(scenario.Duration);
			endScreen.Find("Duration/Value").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
		});

        ferry = FindObjectOfType<FerryController>();
        ferry.OnControlChange.AddListener(() => {
            if (ferry.ManualControl) {
                Alert("Manual takeover required\nDock at " + ferry.DestinationDock.name, duration: IMPORTANT_ALERT_DURATION, color: Color.red);
            }
			transform.Find("ModeIndicator/Text").GetComponent<Text>().text = "MODE: " + (ferry.ManualControl ? "MANUAL" : "AUTOMATIC");
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
