using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EmbarkPassenger : MonoBehaviour {
    public Vector3[] seats;

    private List<Passenger> passengers = new List<Passenger>();
    private FerryController ferry;

    private void Start() {
        ferry = GetComponent<FerryController>();

        ferry.OnDisconnectFromDock.AddListener(() => {
            foreach (Passenger passenger in passengers) {
                passenger.agent.enabled = false;
            }
        });

        ferry.OnConnectToDock.AddListener(() => {
            foreach (Passenger passenger in passengers) {
                passenger.agent.enabled = true;
                ferry.dock.IncomingPassenger(passenger);
            }
            passengers.Clear();
        });
    }

    private void Update() {
        if (ferry.boarding) {
            bool boardingCompleted = true;
            foreach (Passenger passenger in passengers) {
                if (!passenger.ReachedDestination) {
                    boardingCompleted = false;
                    break;
                }
            }
            ferry.boarding = !boardingCompleted;
        }
    }

    public bool CanEmbarkFrom(DockController dock) {
        return passengers.Count < seats.Length && ferry.dock.Equals(dock);
    }

    public void Embark(Passenger passenger) {
        int seatIndex = passengers.Count;
        if (ferry.DockDirection < 0) seatIndex = seats.Length - seatIndex - 1;

        passenger.transform.SetParent(transform);
        passenger.SetDestination(transform.position + transform.rotation * seats[seatIndex]);
        passengers.Add(passenger);

        ferry.boarding = true;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        foreach (Vector3 seat in seats) {
            Gizmos.DrawSphere(transform.position + transform.rotation * seat, 0.5f);
        }
    }
}
