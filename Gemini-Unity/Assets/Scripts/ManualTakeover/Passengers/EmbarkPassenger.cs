using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EmbarkPassenger : MonoBehaviour {
    public Vector3[] seats;

    private List<Passenger> passengers = new List<Passenger>();
    private FerryTrip ferryTrip;

    private void Start() {
        ferryTrip = GetComponent<FerryTrip>();
        ferryTrip.OnPlay.AddListener(() => { 
            foreach (Passenger passenger in passengers) {
                passenger.agent.enabled = false;
            }
        });
        ferryTrip.OnEndReached.AddListener(DisembarkAll);
    }

    private void Update() {
        if (ferryTrip.boarding) {
            bool boardingCompleted = true;
            foreach (Passenger passenger in passengers) {
                if (!passenger.ReachedDestination) {
                    boardingCompleted = false;
                    break;
                }
            }
            ferryTrip.boarding = !boardingCompleted;
        }
    }

    public bool CanEmbarkFrom(DockController dock) {
        return passengers.Count < seats.Length && !ferryTrip.Playing && ferryTrip.dock.Equals(dock);
    }

    public void Embark(Passenger passenger) {
        passenger.transform.SetParent(transform);
        passenger.SetDestination(transform.position + transform.rotation * seats[passengers.Count]);
        passengers.Add(passenger);

        ferryTrip.boarding = true;
    }

    private void DisembarkAll() {
        if (ferryTrip.dock == null) return;

        foreach (Passenger passenger in passengers) {
            passenger.agent.enabled = true;
            ferryTrip.dock.IncomingPassenger(passenger);
        }
        passengers.Clear();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        foreach (Vector3 seat in seats) {
            Gizmos.DrawSphere(transform.position + transform.rotation * seat, 0.5f);
        }
    }
}
