using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePath : MonoBehaviour {
    public BoatController boat;
    public bool closedPath, loop;

    public int atIndex { get; private set; }
    public int NodeCount => transform.childCount;

    private Transform From => transform.GetChild(atIndex);
    private Transform To => transform.GetChild(atIndex + 1);

    private bool EndReached => atIndex >= transform.childCount - 1;
    public bool Playing { get; private set; }

    // Start is called before the first frame update
    void Start() {
        if (transform.childCount == 0) return;

        if (closedPath) {
            Instantiate(From, transform);
        }
    }

    // Update is called once per frame
    void Update() {
        if (!boat.Destination.HasValue || EndReached) return;

        boat.Destination = To.position;

        if (Vector3.Distance(boat.transform.position, To.position) <= BoatController.STOP_DISTANCE) {
            atIndex++;
            Travel();
        }
    }

    public void Stop() {
        Playing = false;
        MoveToNode(0);
    }

    public void Play(int startNode = 0) {
        MoveToNode(startNode);
        Playing = true;
        atIndex = Mathf.Clamp(startNode, 0, transform.childCount - 1);
        boat.transform.position = From.position;
        Travel();
    }

    private void MoveToNode(int index) {
        atIndex = index;
        boat.Destination = null;
        boat.transform.position = From.position;

        PathNode node = From.GetComponent<PathNode>();
        if (node != null) {
            boat.targetRotation = Quaternion.Euler(node.rotation);
            boat.transform.rotation = boat.targetRotation;
        }
    }
    private void Travel() {
        IEnumerator BeginTravel(float waitTime) {
            if (waitTime > 0) boat.Destination = null;
            yield return new WaitForSeconds(waitTime);
            if (Playing) boat.Destination = To.position;
        }

        PathNode node = From.GetComponent<PathNode>();
        if (node != null) {
            boat.targetRotation = Quaternion.Euler(node.rotation);
        }

        if (EndReached) {
            Debug.Log("End of travel");
            Playing = false;
            boat.Destination = null;
            if (loop) Play();
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

        if (closedPath) {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
        }

        if (boat != null) {
            Gizmos.color = Color.green;
            if (boat.Destination.HasValue) {
                Gizmos.DrawLine(boat.transform.position, boat.Destination.Value);
                Gizmos.DrawSphere(boat.Destination.Value, 0.75f);
            }
        }
    }
}
