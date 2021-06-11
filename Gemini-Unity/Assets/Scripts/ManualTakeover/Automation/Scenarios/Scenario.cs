using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ManualTakeoverEvent : UnityEvent<float, string> { }

public abstract class Scenario : ExtendedMonoBehaviour {
	private const float SPAWN_INTERVAL = 1;

	[HideInInspector]
	public UnityEvent OnPlay, OnManualTakeover, OnCompletion;
	[HideInInspector]
	public ManualTakeoverEvent OnManualTakeoverImminent = new ManualTakeoverEvent();

	public int minSpawnAmount = 2, maxSpawnAmount = 12, tripCount = 3;
	public bool autoPlay = true;

	public FerryController Ferry { get; private set; }
	private FerryTrip trip;

	private float startTime, endTime;

	public bool Playing { get; private set; }
	public bool Done { get; private set; } = true;
	public float Duration => endTime - startTime;

	protected virtual void TripStartAction() { }
	protected virtual void Step() { }

	public abstract string FailureWarning { get; }

	public virtual void SetupScenario() {
		Debug.Log("Setting up scenario");

		GameObject player = GameObject.FindGameObjectWithTag("Player");
		Ferry = player.GetComponent<FerryController>();
		trip = player.GetComponent<FerryTrip>();

		Ferry.OnConnectToDock.AddListener(() => {
			if (!Done && Ferry.ManualControl) {
				Done = true;
				Playing = false;
				Ferry.ManualControl = false;
				endTime = Time.timeSinceLevelLoad;

				Debug.Log("Scenario completed");
				OnCompletion?.Invoke();
			}
		});

		// Ensure there are passengers on the destination dock before it is reached by the ferry
		Ferry.OnDisconnectFromDock.AddListener(() => {
			Repeat(() => { Ferry.DestinationDock.SpawnPassenger(); },
			times: Random.Range(minSpawnAmount, maxSpawnAmount), interval: SPAWN_INTERVAL);
		});

		foreach (DockController dock in FindObjectsOfType<DockController>()) {
			dock.OnArrivalComplete.AddListener(() => {
				if (tripCount > 0) dock.PassengerDeparture();
			});
		}

		player.GetComponent<PassengerBoarder>().OnBoardingCompleted.AddListener(() => {
			if (tripCount > 0) {
				if (!trip.Play()) {
					Debug.LogError("FerryTrip failed");
					return;
				}
				tripCount--;

				TripStartAction();
			}
		});

		// Ensure the scene is setup and ready before autoplay
		if (autoPlay) Schedule(Play, 1f);
	}

	public void Play() {
		if (Playing) return;

		if (Ferry.AtDock == null) {
			Debug.LogError("Couldn't play scenario (ferry not docked)");
			return;
		}

		startTime = Time.timeSinceLevelLoad;
		Ferry.ManualControl = false;
		Done = false;
		Playing = true;

		Repeat(() => { Ferry.AtDock.SpawnPassenger(); },
			onCompletion: Ferry.AtDock.PassengerDeparture,
			times: Random.Range(minSpawnAmount, maxSpawnAmount), interval: SPAWN_INTERVAL);

		Debug.Log("Playing scenario");
		OnPlay?.Invoke();
	}

	private void Update() {
		if (Playing) {
			endTime = Time.timeSinceLevelLoad;
			if (Input.GetButtonDown("ManualTakeover")) {
				ManualTakeover();
			}
			Step();
		}
	}

	protected virtual void ManualTakeover() {
		if (!Playing) return;

		Playing = false;
		trip.Playing = false;
		Ferry.ManualControl = true;
		Debug.Log("Manual takeover");

		OnManualTakeover?.Invoke();
	}
}
