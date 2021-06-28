using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FerryAutopilot))]
public class ObstacleAvoidance : MonoBehaviour {
    private const int OBSTACLE_MASK = 1 << 15;

    private FerryAutopilot ferryTrip;
    private bool braking;

    public float acceleration, lookAhead;
    [Range(-1f, 0)]
    public float alignmentThreshold = -0.5f;

    private void Start() {
        ferryTrip = GetComponent<FerryAutopilot>();
    }

    private void Update() {
        if (ferryTrip.Playing) {
            braking = false;
            foreach (Collider col in Physics.OverlapSphere(transform.position, lookAhead, OBSTACLE_MASK)) {
                // Stop if obstacle is in front of ferry
                if (Vector3.Dot(transform.forward, Vector3.Normalize(col.transform.position - transform.position)) > alignmentThreshold) {
                    braking = true;
                    break;
                }
            }

            ferryTrip.throttleScale = Mathf.Clamp01(ferryTrip.throttleScale + acceleration * Time.deltaTime * (braking ? -1 : 1));
        }
    }

    private void OnDrawGizmos() {
        if (ferryTrip != null && ferryTrip.Playing) {
            Gizmos.color = braking ? Color.red : Color.yellow;
            Gizmos.DrawWireSphere(transform.position, lookAhead);
        }
    }
}
