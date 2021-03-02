using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Passenger : MonoBehaviour {
    private NavMeshAgent agent;
    private float waitUntil;

    public bool IsBusy => agent == null || waitUntil > Time.time;
    public bool inTransit;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Vector3 destination, float waitTime=0) {

        waitUntil = Time.time + waitTime;
        agent.SetDestination(destination);
    }

    void OnDrawGizmos()
    {
        if (inTransit) OnDrawGizmosSelected();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, agent.steeringTarget);
        Gizmos.DrawSphere(agent.destination, 0.25f);
    }
}
