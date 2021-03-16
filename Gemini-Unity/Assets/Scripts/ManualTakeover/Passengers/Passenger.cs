using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Passenger : MonoBehaviour {
    public NavMeshAgent agent { get; private set; }
    public ThirdPersonCharacter character { get; private set; }

    private float waitUntil;

    public bool IsBusy => agent == null || !agent.enabled || waitUntil > Time.time;
    public bool ReachedDestination => agent == null || (agent.enabled && agent.remainingDistance <= agent.stoppingDistance);

    private void Start() {
        agent = GetComponentInChildren<NavMeshAgent>();
        character = GetComponent<ThirdPersonCharacter>();
    }

    public void SetDestination(Vector3 destination, float waitTime = 0) {
        waitUntil = Time.time + waitTime;
        agent.SetDestination(destination);
    }

    private void Update() {
        character.Move(ReachedDestination ? Vector3.zero : agent.desiredVelocity, false, false);
    }

    void OnDrawGizmosSelected() {
        if (agent == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, agent.steeringTarget);
        Gizmos.DrawSphere(agent.destination, 0.25f);
    }
}
