using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePath : MonoBehaviour {
	private const float STOP_DISTANCE = 2f;
	private const float LOOK_AHEAD = 10f;

	public BoatController boat;
	public bool closedPath, loop;
	public int startNode;
	
	private int atIndex;

	private Transform From => transform.GetChild(atIndex);
	private Transform To => transform.GetChild(atIndex + 1);

	private bool EndReached => atIndex >= transform.childCount - 1;

	// Start is called before the first frame update
	void Start() {
		if (transform.childCount == 0) return;

		if (closedPath) {
			Instantiate(From, transform);
		}

		Play();
    }

    // Update is called once per frame
    void Update() {
		if (!boat.destination.HasValue || EndReached) return;

		Vector3 actualHeading = To.position - boat.transform.position,
				idealHeading = To.position - From.position;
		// Project actualHeading onto idealHeading to get heading correction
		Vector3 correction = Vector3.Dot(actualHeading, idealHeading) / Vector3.Dot(idealHeading, idealHeading) * idealHeading;
		boat.destination = To.position - correction + idealHeading.normalized * LOOK_AHEAD;

		if (Vector3.Distance(boat.transform.position, To.position) <= STOP_DISTANCE) {
			atIndex++;
			Travel();
		}
	}

	void Play() {
		atIndex = Mathf.Clamp(startNode, 0, transform.childCount - 1);
		boat.transform.position = From.position;
		Travel();
	}

	void Travel() {
		IEnumerator BeginTravel(float waitTime) {
			if (waitTime > 0) boat.destination = null;
			yield return new WaitForSeconds(waitTime);
			boat.destination = To.position;
		}

		PathNode node = From.GetComponent<PathNode>();
		if (node != null) {
			boat.targetRotation = Quaternion.Euler(node.rotation);
		}

		if (EndReached) {
			Debug.Log("End of travel");
			boat.destination = null;
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
			if (boat.destination.HasValue) {
				Gizmos.DrawLine(boat.transform.position, boat.destination.Value);
				Gizmos.DrawSphere(boat.destination.Value, 0.75f);
			}
		}
	}
}
