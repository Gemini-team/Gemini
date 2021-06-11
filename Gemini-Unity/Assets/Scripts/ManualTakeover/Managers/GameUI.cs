using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : ExtendedMonoBehaviour {
	private Scenario scenario;
	private FerryController ferry;

	private void Toggle(string name, bool state) {
		transform.Find(name).gameObject.SetActive(state);
	}
	public void Show(string name) => Toggle(name, true);
	public void Hide(string name) => Toggle(name, false);

	private NotificationWidget notifications;

	private void Start() {
		notifications = FindObjectOfType<NotificationWidget>();
		
		Hide("EndScreen");
		Hide("ExitScreen");

		scenario = FindObjectOfType<Scenario>();
		scenario.OnPlay.AddListener(() => {
			notifications.PushNotification("Autopilot engaged\nStandby");
		});
		scenario.OnCompletion.AddListener(() => {
			Transform endScreen = transform.Find("EndScreen");
			endScreen.gameObject.SetActive(true);

			TimeSpan ts = TimeSpan.FromSeconds(scenario.Duration);
			endScreen.Find("Duration/Value").GetComponent<Text>().text = string.Format("{0:00}:{1:00}", ts.Minutes, ts.Seconds);
		});

		ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
		ferry.OnControlChange.AddListener(() => {
			if (ferry.ManualControl) {
				GetComponentInChildren<WarningWidget>().ShowWarning(scenario.FailureWarning + "\n Manual takeover required");
			}
		});
		ferry.DockMessage.AddListener(msg => {
			notifications.PushNotification(msg);
		});

		PassengerBoarder boarder = ferry.GetComponent<PassengerBoarder>();
		boarder.OnBoardingBegin.AddListener(() => notifications.PushNotification("Boarding passengers"));
		boarder.OnBoardingCompleted.AddListener(() => notifications.PushNotification("Boarding completed"));
	}

	private void Update() {
		if (Input.GetButtonDown("Pause")) {
			Show("ExitScreen");
		}
	}
}
