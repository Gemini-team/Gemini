using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class Passenger : MonoBehaviour {
    private AICharacterControl ai;
    private float waitUntil;

    public bool IsBusy => ai == null || waitUntil > Time.time;
    public bool inTransit;

    private void Start()
    {
        ai = GetComponent<AICharacterControl>();
    }

    public void SetDestination(Vector3 destination, float waitTime=0) {
        waitUntil = Time.time + waitTime;
        ai.destination = destination;
    }

    void OnDrawGizmos()
    {
        if (inTransit) OnDrawGizmosSelected();
    }

    void OnDrawGizmosSelected()
    {
        if (ai == null) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, ai.agent.steeringTarget);
        Gizmos.DrawSphere(ai.agent.destination, 0.25f);
    }
}
