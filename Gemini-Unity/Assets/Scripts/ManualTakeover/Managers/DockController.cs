using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DockController : ExtendedMonoBehaviour {
	[HideInInspector]
	public UnityEvent OnArrivalComplete;

    public GameObject[] passengerTemplates;
    public PassengerQueue queue;
    public Transform spawnPoint, despawnPoint;

    private PassengerBoarder boarder;
	private int passengersInTransit;

    private void Start() {
        boarder = GameObject.FindGameObjectWithTag("Player").GetComponent<PassengerBoarder>();
        queue = GetComponentInChildren<PassengerQueue>();
    }

    public Passenger SpawnPassenger() {
        GameObject instance = Instantiate(
            original: passengerTemplates[Random.Range(0, passengerTemplates.Length)],
            position: spawnPoint.position,
            rotation: Quaternion.identity);

        Passenger passenger = instance.GetComponent<Passenger>();
        passenger.transform.parent = transform;
        queue.Enqueue(passenger);

        return passenger;
    }

    public void PassengerDeparture() {
		// Register passengers for boarding until either queue is empty OR all seats are taken
        while (queue.Count > 0 && boarder.CanBoardFrom(this)) {
            Passenger passenger = queue.Dequeue();
            boarder.RegisterPassenger(passenger);
        }
		boarder.BeginBoarding();
    }

    public void PassengerArrival(IList<Passenger> passengers) {
		passengersInTransit += passengers.Count;

		foreach (Passenger passenger in passengers) {
			passenger.transform.SetParent(transform);
			passenger.SetDestinationSynced(despawnPoint.position);

			// Delay to ensure the agent's destination has been updated in time
			Schedule(() => {
				passenger.OnDestinationReached.AddListener(() => {
					Destroy(passenger.gameObject);

					passengersInTransit--;
					if (passengersInTransit <= 0) {
						OnArrivalComplete?.Invoke();
					}
				});
			}, 0.1f);
		}
    }

    private void OnDrawGizmos() {
        if (spawnPoint != null) {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(spawnPoint.position, 0.75f);
        }
        if (despawnPoint != null) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(despawnPoint.position, 0.75f);
        }
    }
}
