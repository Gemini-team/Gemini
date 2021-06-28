using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ManualTakeoverEvent : UnityEvent<float, string> { }

public abstract class Scenario : ExtendedMonoBehaviour {
    private const float SPAWN_INTERVAL = 1;

    [HideInInspector]
    public UnityEvent OnPlay, OnManualTakeoverRequired, OnCompletion;
    [HideInInspector]
    public ManualTakeoverEvent OnManualTakeoverImminent = new ManualTakeoverEvent();

    public int minSpawnAmount = 2, maxSpawnAmount = 12, tripCount = 3;
    public bool autoPlay = true, infinite = false;

    public FerryController Ferry { get; private set; }
    private FerryAutopilot trip;

    private float startTime, endTime;

    protected bool Playing { get; private set; }
    public bool Done { get; private set; } = true;
    public bool ManualTakeoverRequired { get; private set; }
    public float Duration => endTime - startTime;

    protected virtual void TripStartAction() { }

    public abstract string ScenarioName { get; }
    public abstract string FailureWarning { get; }

    public virtual void SetupScenario() {
        Debug.Log("Setting up scenario");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Ferry = player.GetComponent<FerryController>();
        trip = player.GetComponent<FerryAutopilot>();

        Ferry.OnConnectToDock.AddListener(() => {
            if (tripCount <= 0 && !Done && Ferry.ManualControl) {
                Pause();
                Ferry.ManualControl = false;
                Done = true;
                endTime = Time.timeSinceLevelLoad;

                Debug.Log("Scenario completed");
                OnCompletion?.Invoke();
            }
        });

        // Ensure there are passengers on the destination dock before it is reached by the ferry
        Ferry.OnDisconnectFromDock.AddListener(() => {
            Repeat(() => { Ferry.DestinationDock.SpawnPassenger(); },
            times: Random.Range(minSpawnAmount, maxSpawnAmount + 1), interval: SPAWN_INTERVAL);
        });

        foreach (DockController dock in FindObjectsOfType<DockController>()) {
            dock.OnArrivalComplete.AddListener(() => {
                if (tripCount > 0) dock.PassengerDeparture();
            });
        }

        player.GetComponent<PassengerBoarder>().OnBoardingCompleted.AddListener(() => {
            if (tripCount > 0) {
                if (!Ferry.ManualControl) {  // Only play trip if ferry's autopilot is engaged
                    bool success = trip.Play();
                    if (!success) {
                        Debug.LogError("FerryTrip failed");
                        return;
                    }
                }

                if (!infinite) tripCount--;

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
        ManualTakeoverRequired = false;
        Ferry.ManualControl = false;
        Done = false;
        Playing = true;

        Repeat(() => { Ferry.AtDock.SpawnPassenger(); },
            onCompletion: Ferry.AtDock.PassengerDeparture,
            times: Random.Range(minSpawnAmount, maxSpawnAmount), interval: SPAWN_INTERVAL);

        OnPlay?.Invoke();
        Debug.Log("Playing scenario");
    }

    private void Pause() {
        if (!Playing) return;

        Playing = false;
        trip.Playing = false;
        Ferry.ManualControl = true;
    }

    private void Resume() {
        if (Done || Playing) return;

        Playing = true;
        trip.Playing = true;
        Ferry.ManualControl = false;
    }

    private void Update() {
        if (Done) return;

        endTime = Time.timeSinceLevelLoad;

        if (!ManualTakeoverRequired && Input.GetButtonDown("ManualTakeover")) {
            if (Playing) Pause();
            else Resume();
        }
    }

    protected virtual void TriggerManualTakeoverEvent() {
        if (!Playing || ManualTakeoverRequired) return;

        ManualTakeoverRequired = true;
        Pause();

        Debug.Log("Manual takeover required");
        OnManualTakeoverRequired?.Invoke();
    }
}
