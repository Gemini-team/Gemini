using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : ExtendedMonoBehaviour {
	private const float NOTIFICATION_DURATION = 4, WARNING_DURATION = 15, BLINK_INTERVAL = 1;

	private Color BGColorNormal => new Color(0, 0, 0, 0.5f);
	private Color BGColorWarning => new Color(0.5f, 0, 0, 1);

	private Scenario scenario;
	private FerryController ferry;
	private Image alertBox;
	private Text alertMessage, distanceGauge;

	private List<Coroutine> notificationCoroutines = new List<Coroutine>();

	private void Notification(string message, float? duration = NOTIFICATION_DURATION) {
		alertBox.color = BGColorNormal;
		alertBox.gameObject.SetActive(true);
		alertMessage.text = message;

		foreach (Coroutine co in notificationCoroutines) {
			StopCoroutine(co);
		}
		notificationCoroutines.Clear();

		if (duration.HasValue) {
			notificationCoroutines.Add(
				Schedule(() => alertBox.gameObject.SetActive(false), duration.Value));
		}
	}

	private void Warning(string message, float? duration = WARNING_DURATION) {
		IEnumerator Blink() {
			bool on = false;
			while (alertBox.gameObject.activeSelf) {
				on = !on;
				alertBox.color = on ? BGColorWarning : BGColorNormal;
				yield return new WaitForSeconds(BLINK_INTERVAL);
			}
		}

		Notification(message, WARNING_DURATION);
		notificationCoroutines.Add(StartCoroutine(Blink()));
	}

	private void Toggle(string name, bool state) {
		transform.Find(name).gameObject.SetActive(state);
	}
	public void Show(string name) => Toggle(name, true);
	public void Hide(string name) => Toggle(name, false);

	private void Start() {
		alertBox = transform.Find("AlertBox").GetComponent<Image>();
		alertMessage = transform.Find("AlertBox/Text").GetComponent<Text>();
		distanceGauge = transform.Find("Dashboard/Distance/Value").GetComponent<Text>();

		Hide("AlertBox");
		Hide("EndScreen");
		Hide("HelpScreen");
		Hide("ExitScreen");

		scenario = FindObjectOfType<Scenario>();
		scenario.OnPlay.AddListener(() => {
			Notification("Autopilot engaged\nStandby");
		});
		scenario.OnCompletion.AddListener(() => {
			alertBox.gameObject.SetActive(false);

			Transform endScreen = transform.Find("EndScreen");
			endScreen.gameObject.SetActive(true);

			TimeSpan ts = TimeSpan.FromSeconds(scenario.Duration);
			endScreen.Find("Duration/Value").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
		});

		ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
		ferry.OnControlChange.AddListener(() => {
			if (ferry.ManualControl) {
				Warning("Manual takeover required\nDock at " + ferry.DestinationDock.name);
			}
			transform.Find("ModeIndicator/Text").GetComponent<Text>().text = "MODE: " + (ferry.ManualControl ? "MANUAL" : "AUTOMATIC");
		});
		ferry.DockMessage.AddListener(msg => {
			Notification(msg);
		});

		PassengerBoarder boarder = ferry.GetComponent<PassengerBoarder>();
		boarder.OnBoarding.AddListener(() => Notification("Boarding passengers", null));
		boarder.OnBoardingCompleted.AddListener(() => Notification("Boarding completed"));
	}

	private void Update() {
		distanceGauge.text = ferry.AtDock == null ? Mathf.RoundToInt(ferry.RemainingDistance) + "m" : "0m (Docked)";

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Show("ExitScreen");
		}
	}
}
