using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MovePath : MonoBehaviour {
    public BoatController boat;

    public int atIndex { get; private set; }
    public int NodeCount => transform.childCount;

    private Transform From => transform.GetChild(atIndex);
    private Transform To => transform.GetChild(atIndex + (backTrip ? -1 : 1));

    private bool EndReached => backTrip ? (atIndex <= 0) : (atIndex >= transform.childCount - 1);
    public bool Playing { get; private set; }

    public bool backTrip { get; private set; }

    // Start is called before the first frame update
    void Start() {
        if (transform.childCount == 0) return;

        // TODO: Find percentage values for each node through binary search
    }

    // Update is called once per frame
    void Update() {
        if (!Playing) return;

        boat.Destination = To.position;

        if (Vector3.Distance(boat.transform.position, To.position) <= BoatController.STOP_DISTANCE) {
            atIndex += backTrip ? -1 : 1;
            Travel();
        }
    }

    public void Stop() {
        Playing = false;
        atIndex = backTrip ? transform.childCount - 1 : 0;
        boat.Destination = null;
        boat.transform.position = From.position;

        PathNode node = From.GetComponent<PathNode>();
        if (node != null) {
            boat.targetRotation = Quaternion.Euler(node.rotation);
            boat.transform.rotation = boat.targetRotation;
        }
    }

    public void Play() {
        Stop();  // Reset progress before playing
        Playing = true;
        Travel();
    }

    public void MoveToNode(int index) {
        if (Playing) return;

        
    }
    private void Travel() {
        IEnumerator BeginTravel(float waitTime) {
            if (waitTime > 0) {
                Playing = false;
                boat.Destination = null;
            }
            yield return new WaitForSeconds(waitTime);
            Playing = true;
        }

        PathNode node = From.GetComponent<PathNode>();
        if (node != null) {
            boat.targetRotation = Quaternion.Euler(node.rotation);
        }

        if (EndReached) {
            backTrip = !backTrip;
            boat.reversing = backTrip;
            Debug.Log("End of travel");
            Stop();
            return;
        }

        StartCoroutine(BeginTravel(node == null ? 0 : node.waitTime));
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < transform.childCount - 1; i++) {
            Vector3 a = transform.GetChild(i).position, b = transform.GetChild(i + 1).position;
            Gizmos.color = i == atIndex ? Color.cyan : Color.yellow;
            Gizmos.DrawLine(a, b);
            Gizmos.DrawSphere(b, 0.75f);
        }

        if (Playing && boat != null) {
            Gizmos.color = Color.green;
            if (boat.Destination != null) {
                Gizmos.DrawLine(boat.transform.position, boat.Destination.Value);
                Gizmos.DrawSphere(boat.Destination.Value, 0.75f);
            }
        }
    }
}
