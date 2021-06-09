using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassengerBoarder : MonoBehaviour {
    private const float SEAT_OFFSET = 0.25f;

    [HideInInspector]
    public UnityEvent OnBoardingBegin, OnBoarding, OnBoardingCompleted;
    public Vector3[] seats;

    private List<Passenger> passengers = new List<Passenger>();
    private FerryController ferry;

    public float PassengerCount => passengers.Count;

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
            }
            ferry.AtDock.PassengerArrival(passengers);
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

            if (boardingCompleted) {
                ferry.boarding = false;
                OnBoardingCompleted?.Invoke();
            }
        }
    }

    public bool CanBoardFrom(DockController dock) {
		return passengers.Count < seats.Length && dock.Equals(ferry.AtDock);
    }

    public void Board(Passenger passenger) {
        int seatIndex = passengers.Count;
        if (ferry.DockDirection < 0) seatIndex = seats.Length - seatIndex - 1;

        Vector3 seat = seats[seatIndex] + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * SEAT_OFFSET;

        passenger.transform.SetParent(transform);
        passenger.SetDestination(transform.position + transform.rotation * seat);
        passengers.Add(passenger);

		if (!ferry.boarding) {
			ferry.boarding = true;
			OnBoardingBegin?.Invoke();
		}

        OnBoarding?.Invoke();
    }

    private void OnDrawGizmosSelected() {
		/*
		Gizmos.color = Color.white;
        foreach (Vector3 seat in seats) {
            Gizmos.DrawSphere(transform.position + transform.rotation * seat, 0.5f);
        }
		*/
    }
}
