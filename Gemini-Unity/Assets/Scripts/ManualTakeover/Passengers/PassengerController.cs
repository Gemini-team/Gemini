using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawns passengers. Controls passenger behaviours (idle and embark/disembark)
/// Add as component to waiting area.
/// </summary>
public class PassengerController : MonoBehaviour {
    private List<Passenger> passengers = new List<Passenger>();
    private BoatController ferry;

    public int spawnAmount;
    public GameObject passengerTemplate;

    /// <summary>
    /// Generates a random destination on the attached nav mesh
    /// </summary>
    /// <returns>Vector3</returns>
    private Vector3 IdleDestination() => transform.position + new Vector3(Random.Range(-0.5f, 0.5f) * transform.localScale.x, passengerTemplate.transform.localScale.y, Random.Range(-0.5f, 0.5f) * transform.localScale.z);

    private void Start() {
        ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<BoatController>();

        for (int i = 0; i < spawnAmount; i++) {
            GameObject instance = Instantiate(
                original: passengerTemplate,
                position: IdleDestination(),
                rotation: Quaternion.identity);
            passengers.Add(instance.GetComponent<Passenger>());
        }
    }

    void Update() {
        foreach (Passenger passenger in passengers) {
            if (!passenger.IsBusy) {
                passenger.SetDestination(IdleDestination(), Random.Range(15, 30));
            }
        }
    }
}
