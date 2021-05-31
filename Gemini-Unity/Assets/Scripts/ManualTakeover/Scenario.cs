using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Scenario : ExtendedMonoBehaviour {
    private const float SPAWN_INTERVAL = 1, TAKEOVER_FORCE = 20000, SHUTDOWN_TIME = 20;

    [HideInInspector]
    public UnityEvent OnPlay, OnManualTakeover, OnCompletion;

    public int spawnAmount = 5, tripCount = 3, stepDelay = 20;
    public float manualTakeoverDelay = 10;

    public FerryController Ferry { get; private set; }
    private FerryTrip trip;
    private float manualTakeoverAtTime;

    public bool Playing { get; private set; }
    public bool Done { get; private set; }

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Ferry = player.GetComponent<FerryController>();
        trip = player.GetComponent<FerryTrip>();

        trip.OnEndReached.AddListener(() => {
            Schedule(Step, stepDelay);
        });

        Ferry.OnConnectToDock.AddListener(() => { 
            if (!Done && Ferry.ManualControl) {
                Done = true;
                Playing = false;
                Debug.Log("Scenario completed");
                OnCompletion?.Invoke();
                Schedule(Application.Quit, SHUTDOWN_TIME);
            }
        });

        player.GetComponent<EmbarkPassenger>().OnBoardingCompleted.AddListener(() => { 
            if (Playing) {
                trip.Play();
            }
        });

        Schedule(Play, 2);
    }

    public void Play() {
        if (Playing || Ferry.AtDock == null) return;

        Ferry.ManualControl = false;
        Done = false;
        Playing = true;
        Step();
        
        Debug.Log("Playing scenario");
        OnPlay?.Invoke();
    }

    private void Step() {
        if (tripCount > 0) {
            tripCount--;
            Repeat(() => { Ferry.AtDock.SpawnPassenger(); }, 
            onCompletion: Ferry.AtDock.EmbarkAll, 
            times: spawnAmount, interval: SPAWN_INTERVAL);

            if (tripCount == 0) manualTakeoverAtTime = Time.time + stepDelay + manualTakeoverDelay;
        }
    }

    private void Update() {
        if (!Playing) return;

        if (Input.GetButtonDown("ManualTakeover") || (!Ferry.ManualControl && tripCount == 0 && Time.time > manualTakeoverAtTime)) {
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
