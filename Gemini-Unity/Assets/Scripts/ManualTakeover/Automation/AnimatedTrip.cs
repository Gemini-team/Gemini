using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

[RequireComponent(typeof(BoatController))]
public abstract class AnimatedTrip : MonoBehaviour {
	private const float LOOK_AHEAD_TIME = 0.05f, ALIGNMENT_THRESHOLD = 0.98f;

	[HideInInspector]
	public UnityEvent OnPlay, OnStop, OnEndReached;

	public PathCreator route;
	[Range(0, 0.5f)]
	public float easeIn = 0.1f, easeOut = 0.1f;
	public float startDelay = 2, stopTime = 0.001f;
	public float speedScale = 1;
	public bool canDriveInReverse;

	internal bool reverse;
	private bool playing;
	private float waitUntil;
	private BoatController controller;

	private Vector3 target;
	private float rudder;

	private float TimeLeft {
		get {
			float closestTime = route.path.GetClosestTimeOnPath(transform.position);
			return reverse ? closestTime : 1 - closestTime;
		}
	}

	private float Throttle {
		get {
			float throttle = 1;


			if (TimeLeft >= 0.5f) {
				if (easeIn > 0) {
					throttle = Mathf.Clamp01((1 - TimeLeft) / easeIn);
				}
			}
			else {
				if (easeOut > 0) {
					throttle = Mathf.Clamp01(TimeLeft / easeOut);
				}
			}

			if (reverse && canDriveInReverse) throttle *= -1;
			return throttle * speedScale;
		}
	}

	public virtual bool Playing {
		get => playing;
		set => playing = value;
	}

	protected virtual void Start() {
		controller = GetComponent<BoatController>();
	}

	protected virtual void Update() {
		if (!Playing || Time.time < waitUntil) return;


		float closestTime = route.path.GetClosestTimeOnPath(transform.position);
		target = route.path.GetPointAtTime(closestTime + LOOK_AHEAD_TIME * (reverse ? -1 : 1), EndOfPathInstruction.Stop);

		Debug.Log("Playing " + name + " | time: " + closestTime + " | target: " + target);

		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, route.path.GetRotation(closestTime).eulerAngles.y, transform.eulerAngles.z);

		controller.input = new Vector2(0, Throttle);
		controller.rudder = 0;

		if (TimeLeft <= stopTime) {
			SkipToEnd();
		}
	}

	public virtual void Play() {
		Playing = true;
		waitUntil = Time.time + startDelay;

		OnPlay?.Invoke();
	}

	public virtual void Stop() {
		Playing = false;
		OnStop?.Invoke();
	}

	public virtual void SkipToEnd() {
		Stop();

		OnEndReached?.Invoke();
	}

	private void OnDrawGizmos() {
		if (Playing) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, target);
			Gizmos.DrawWireSphere(target, 0.75f);
		}
	}
}
