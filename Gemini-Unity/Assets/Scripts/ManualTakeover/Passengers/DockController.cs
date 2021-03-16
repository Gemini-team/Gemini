using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockController : MonoBehaviour {
    public int spawnAmount;
    public GameObject passengerTemplate;
    public PassengerQueue queue;
    public Transform[] spawnAreas;

    private EmbarkPassenger boarder;
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
        boarder = GameObject.FindGameObjectWithTag("Player").GetComponent<EmbarkPassenger>();
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

    public void MoveToQueue() {
        if (passengers.Count == 0) return;

        Passenger passenger = passengers[0];
        queue.Enqueue(passenger);
        passengers.RemoveAt(0);
    }

    public void EmbarkAll() {
        while (queue.Count > 0 && boarder.CanEmbarkFrom(this)) {
            Passenger passenger = queue.Dequeue();
            boarder.Embark(passenger);
        }
    }
}
