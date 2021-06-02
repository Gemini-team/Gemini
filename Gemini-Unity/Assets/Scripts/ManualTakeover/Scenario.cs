using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scenario : ExtendedMonoBehaviour {
    private const float SPAWN_INTERVAL = 1, TAKEOVER_FORCE = 20000, SHUTDOWN_TIME = 10;

    [HideInInspector]
    public UnityEvent OnPlay, OnManualTakeover, OnCompletion;

    public int spawnAmount = 5, tripCount = 3, stepDelay = 20;
    public float manualTakeoverDelay = 10;

    public FerryController Ferry { get; private set; }
    private FerryTrip trip;

    public bool Playing { get; private set; }
    public bool Done { get; private set; }

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Ferry = player.GetComponent<FerryController>();
        trip = player.GetComponent<FerryTrip>();

        Ferry.OnConnectToDock.AddListener(() => { 
            if (!Done && Ferry.ManualControl) {
                Done = true;
                Playing = false;
                Debug.Log("Scenario completed");
                OnCompletion?.Invoke();
				// TODO: Toggle end screen
                Schedule(Application.Quit, SHUTDOWN_TIME);
            }
        });

		// Ensure there are passengers on the destination dock before it is reached by the ferry
		Ferry.OnDisconnectFromDock.AddListener(() => {
			Repeat(() => { Ferry.DestinationDock.SpawnPassenger(); },
			times: spawnAmount, interval: SPAWN_INTERVAL);
		});

		foreach (DockController dock in FindObjectsOfType<DockController>()) {
			dock.OnArrivalComplete.AddListener(() => {
				dock.PassengerDeparture();
			});
		}

        player.GetComponent<PassengerBoarder>().OnBoardingCompleted.AddListener(() => { 
            if (tripCount > 0) {
				trip.Play();
				tripCount--;

				if (tripCount == 0) Schedule(ManualTakeover, manualTakeoverDelay);
			}
        });

        Schedule(Play, 1);
    }

    public void Play() {
        if (Playing || Ferry.AtDock == null) return;

        Ferry.ManualControl = false;
        Done = false;
        Playing = true;

		Repeat(() => { Ferry.AtDock.SpawnPassenger(); },
			onCompletion: Ferry.AtDock.PassengerDeparture,
			times: spawnAmount, interval: SPAWN_INTERVAL);
        
        Debug.Log("Playing scenario");
        OnPlay?.Invoke();
    }

    private void Update() {
        if (Playing && Input.GetButtonDown("ManualTakeover")) {
            ManualTakeover();
        }
    }

    private void ManualTakeover() {
        Playing = false;
        trip.Playing = false;
        Ferry.ManualControl = true;
        Ferry.GetComponent<Rigidbody>().AddForce(Ferry.transform.forward * TAKEOVER_FORCE);
        Debug.Log("Manual takeover");

        OnManualTakeover?.Invoke();
    }
}
