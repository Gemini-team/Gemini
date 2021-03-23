using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FerryTrip))]
public class ObstacleAvoidance : MonoBehaviour {
    public float acceleration, lookAhead;
    private FerryTrip ferryTrip;
    private const int OBSTACLE_MASK = 1 << 15;

    bool braking;

    private void Start() {
        ferryTrip = GetComponent<FerryTrip>();
    }

    private void Update() {
        if (ferryTrip.Playing) {
            braking = false;
            foreach (Collider col in Physics.OverlapSphere(transform.position, lookAhead, OBSTACLE_MASK)) {
                // Stop if obstacle is in front of ferry
                if (Vector3.Dot(transform.forward, transform.position - col.transform.position) >= 0) {
                    braking = true;
                    break;
                }
            }

            ferryTrip.speedScale = Mathf.Clamp01(ferryTrip.speedScale + acceleration * Time.deltaTime * (braking ? -1 : 1));
        }
    }

    private void OnDrawGizmos() {
        if (ferryTrip != null && ferryTrip.Playing) {
            Gizmos.color = braking ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, lookAhead);
        }
    }
}
