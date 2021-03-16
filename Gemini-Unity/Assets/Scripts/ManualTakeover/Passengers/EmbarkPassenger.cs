﻿using System.Collections;
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

    public bool CanEmbarkFrom(DockController dock) {
        return passengers.Count < seats.Length && !ferryTrip.Playing && ferryTrip.dock.Equals(dock);
    }

    public void Embark(Passenger passenger) {
        passenger.transform.SetParent(transform);
        passenger.SetDestination(transform.position + transform.rotation * seats[passengers.Count]);
        passengers.Add(passenger);
    }

    private void DisembarkAll() {
        throw new System.NotImplementedException();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.white;
        foreach (Vector3 seat in seats) {
            Gizmos.DrawSphere(transform.position + transform.rotation * seat, 0.5f);
        }
    }
}
