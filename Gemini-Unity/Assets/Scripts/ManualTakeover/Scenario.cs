using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scenario : ExtendedMonoBehaviour {
    private const float SPAWN_INTERVAL = 1, TAKEOVER_FORCE = 20000, SHUTDOWN_TIME = 20;

    public int spawnAmount = 5, tripCount = 3, stepDelay = 20;
    public float manualTakeoverDelay = 10;

    private FerryController ferry;
    private FerryTrip trip;
    private float manualTakeoverAtTime;

    public bool Playing { get; private set; }

    private void Start() {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        ferry = player.GetComponent<FerryController>();
        trip = player.GetComponent<FerryTrip>();

        trip.OnEndReached.AddListener(() => {
            Schedule(Step, stepDelay);
        });

        ferry.OnConnectToDock.AddListener(() => { 
            if (Playing && ferry.ManualControl) {
                Playing = false;
                Debug.Log("Scenario completed");
                Schedule(Application.Quit, SHUTDOWN_TIME);
                
            }
        });

        player.GetComponent<EmbarkPassenger>().OnBoardingCompleted.AddListener(trip.Play);

        Schedule(Play, 2);
    }

    public void Play() {
        if (Playing || ferry.dock == null) return;
        ferry.ManualControl = false;
        Playing = true;
        Step();

        Debug.Log("Playing scenario");
    }

    private void Step() {
        if (tripCount > 0) {
            tripCount--;
            Repeat(() => { ferry.dock.SpawnPassenger(); }, 
            onCompletion: ferry.dock.EmbarkAll, 
            times: spawnAmount, interval: SPAWN_INTERVAL);

            if (tripCount == 0) manualTakeoverAtTime = Time.time + stepDelay + manualTakeoverDelay;
        }
    }

    private void Update() {
        if (!Playing) return;

        if (Input.GetKeyDown(KeyCode.G) || (!ferry.ManualControl && tripCount == 0 && Time.time > manualTakeoverAtTime)) {
            ManualTakeover();
        }
    }

    private void ManualTakeover() {
        trip.Playing = false;
        ferry.ManualControl = true;
        ferry.GetComponent<Rigidbody>().AddForce(ferry.transform.forward * TAKEOVER_FORCE);
        Debug.Log("Manual takeover");
    }
}
