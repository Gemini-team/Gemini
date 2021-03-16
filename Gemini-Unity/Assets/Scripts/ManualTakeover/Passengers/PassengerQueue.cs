using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerQueue : MonoBehaviour {
    private const float SPACING = 1.5f;

    private List<Passenger> passengers = new List<Passenger>();

    public int Count => passengers.Count;

    private Vector3 QueuePosition(int index) => transform.position + transform.forward * SPACING * index;

    public void Enqueue(Passenger passenger) {
        passengers.Add(passenger);
        passenger.SetDestination(QueuePosition(passengers.Count));
    }

    public Passenger Dequeue() {
        Passenger passenger = passengers[0];
        passengers.RemoveAt(0);

        for (int i = 0; i < passengers.Count; i++) {
            passengers[i].SetDestination(QueuePosition(i));
        }

        return passenger;
    }

    public void AssembleQueue() {
        for (int i = 0; i < passengers.Count; i++) {
            Vector3 pos = QueuePosition(i);
            passengers[i].SetDestination(pos);
            passengers[i].transform.position = pos;
        }
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 4);
    }
}
