using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PassengerBoarder : MonoBehaviour {
    private const float SEAT_OFFSET = 0.25f;

    [HideInInspector]
    public UnityEvent OnBoardingBegin, OnPassengerRegistered, OnBoardingCompleted;
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

    public bool CanBoardFrom(DockController dock) {
		return passengers.Count < seats.Length && dock.Equals(ferry.AtDock);
    }

	public void BeginBoarding() {
		IEnumerator WaitUntilBoardingComplete() {
			yield return new WaitUntil(() => passengers.TrueForAll(x => x.ReachedDestination));

			ferry.boarding = false;
			OnBoardingCompleted?.Invoke();
		}
		
		if (ferry.boarding) return;  // Check if boarding is already in progress

		for (int i = 0; i < passengers.Count; i++) {
			Vector3 seat = seats[i] + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)) * SEAT_OFFSET;
			passengers[i].SetDestinationSynced(transform.position + transform.rotation * seat);
		}

		ferry.boarding = true;
		OnBoardingBegin?.Invoke();

		StartCoroutine(WaitUntilBoardingComplete());
	}

	public void RegisterPassenger(Passenger passenger) {
		if (passengers.Count >= seats.Length) {
			throw new System.ArgumentOutOfRangeException("Couldn't register passenger (no vacant seats)\nUse PassengerBoarder.CanBoardFrom to check if passenger can be registered for boarding");
		}

        passenger.transform.SetParent(transform);
        passengers.Add(passenger);

        OnPassengerRegistered?.Invoke();
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
