using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

public class FerryTrip : MonoBehaviour {
	[HideInInspector]
	public UnityEvent OnPlay, OnEndReached;

	public PathCreator route;
	public float minSpeed = 1, maxSpeed = 2;
	[Range(0, 0.5f)]
	public float ease = 0.1f;
	public bool reverse;
	[HideInInspector]
	public bool boarding;

	public DockController dock { get; private set; }

	public bool Playing { get; private set; }
	public float DistanceRemaining => route.path.length - distanceTravelled;
	public bool EndReached => DistanceRemaining <= 0;

	private float distanceTravelled;

	private void Start() {
		route.pathUpdated += Step;
		Stop();  // First-time setup
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

				OnEndReached?.Invoke();
			}
		}
	}

	private void Dock() {
		float dist = float.MaxValue;
		foreach (DockController dock in FindObjectsOfType<DockController>()) {
			float dist_ = Vector3.Distance(transform.position, dock.transform.Find("DockingArea").position);
			if (dist_ < dist) {
				this.dock = dock;
				dist = dist_;
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
		Step();  // Move to dock
		Dock();  // Connect to dock
	}

	public void Play() {
		if (boarding) return;

		Stop();
		dock = null;
		Playing = true;

		OnPlay?.Invoke();
	}
}
