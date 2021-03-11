using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class FollowPath : MonoBehaviour {
	public PathCreator route;
	public float minSpeed = 1, maxSpeed = 2;
	[Range(0, 0.5f)]
	public float ease = 0.1f;
	public bool reverse;
    
	public bool Playing { get; private set; }
	public float DistanceRemaining => route.path.length - distanceTravelled;
	public bool EndReached => DistanceRemaining <= 0;

	private float distanceTravelled;

	private void Start() {
		route.pathUpdated += Step;
	}

	private void Update() {
		if (Playing) {
			float speed = maxSpeed;
			if (ease > 0) {
				float easeTime = route.path.GetClosestTimeOnPath(transform.position);
				if (easeTime > 0.5) easeTime = 1 - easeTime;
				speed = (maxSpeed - minSpeed) * Mathf.Clamp01(easeTime / ease) + minSpeed;
			}

			distanceTravelled += speed * Time.deltaTime;
			Step();

			if (EndReached) {
				reverse = !reverse;
				Stop();
			}
		}
	}

	public void Step() {
		float dist = distanceTravelled;
		if (reverse) dist = route.path.length - dist;

		transform.position = route.path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
		transform.rotation = route.path.GetRotationAtDistance(dist, EndOfPathInstruction.Stop);
	}

	public void Stop() {
		Playing = false;
		distanceTravelled = 0;
		Step();  // Move to start
	}

	public void Play() {
		Stop();
		Playing = true;
	}
}
