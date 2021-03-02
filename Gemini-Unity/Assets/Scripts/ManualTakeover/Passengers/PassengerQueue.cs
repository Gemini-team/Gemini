using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerQueue : MonoBehaviour {
    private const float SPACING = 1.5f;

    private List<Passenger> passengers = new List<Passenger>();

    public void Enqueue(Passenger passenger) {
        passengers.Add(passenger);
        passenger.SetDestination(transform.position + transform.forward * SPACING * passengers.Count);
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 4);
    }
}
