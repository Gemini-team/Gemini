using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerController : MonoBehaviour {
    public int spawnAmount;
    public GameObject passengerTemplate;
    public PassengerQueue queue;
    public Transform[] spawnAreas;

    private List<Passenger> passengers = new List<Passenger>();

    /// <summary>
    /// Generates a random destination on the attached nav mesh
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 RandomDestination() {
        Transform area = spawnAreas[Random.Range(0, spawnAreas.Length)];
        return area.position + new Vector3(
            Random.Range(-0.5f, 0.5f) * area.localScale.x,
            passengerTemplate.transform.localScale.y,
            Random.Range(-0.5f, 0.5f) * area.localScale.z);
    }

    private void Start() {
        queue = GetComponentInChildren<PassengerQueue>();

        for (int i = 0; i < spawnAmount; i++) {
            GameObject instance = Instantiate(
                original: passengerTemplate,
                position: RandomDestination(),
                rotation: Quaternion.identity);
            passengers.Add(instance.GetComponent<Passenger>());
        }
    }

    void Update() {
        foreach (Passenger passenger in passengers) {
            if (!passenger.IsBusy) {
                passenger.SetDestination(RandomDestination(), Random.Range(15, 30));
            }
        }
    }

    public void Embark() {
        if (passengers.Count == 0) return;

        Passenger passenger = passengers[0];
        passenger.inTransit = true;
        queue.Enqueue(passenger);
        passengers.RemoveAt(0);
    }
}
