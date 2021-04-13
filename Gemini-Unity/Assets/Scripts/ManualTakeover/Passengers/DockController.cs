using System.Collections.Generic;
using UnityEngine;

public class DockController : ExtendedMonoBehaviour {
    public GameObject[] passengerTemplates;
    public PassengerQueue queue;
    public Transform spawnPoint;

    private EmbarkPassenger boarder;

    private void Start() {
        boarder = GameObject.FindGameObjectWithTag("Player").GetComponent<EmbarkPassenger>();
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

    public void EmbarkAll() {
        while (queue.Count > 0 && boarder.CanEmbarkFrom(this)) {
            Passenger passenger = queue.Dequeue();
            boarder.Embark(passenger);
        }
    }

    public void IncomingPassenger(Passenger passenger) {
        passenger.transform.SetParent(transform);
        passenger.SetDestination(spawnPoint.position);

        // Delay to ensure the agent's destination has been updated in time
        Schedule(() => {
            passenger.OnDestinationReached.AddListener(() => {
                Destroy(passenger.gameObject);
            });
        }, 0.1f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(spawnPoint.position, 0.75f);
    }
}
