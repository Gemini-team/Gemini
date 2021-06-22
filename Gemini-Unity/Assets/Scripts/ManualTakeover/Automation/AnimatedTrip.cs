using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

[RequireComponent(typeof(BoatController))]
public abstract class AnimatedTrip : MonoBehaviour {
	private const float LOOK_AHEAD_TIME = 0.1f, CORRECTION_DAMPENING = 10f, RUDDER_CUTOFF = 0.1f, HEADING_ALIGNMENT_THRESHOLD = 0.8f;

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
	protected BoatController controller { get; private set; }

	private Vector3 target;

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

		Quaternion idealHeading = route.path.GetRotation(closestTime, EndOfPathInstruction.Stop);
		float correctionAngle = idealHeading.eulerAngles.y - transform.eulerAngles.y;

		float rudder = Mathf.Clamp(correctionAngle / CORRECTION_DAMPENING, -1, 1);
		if (Mathf.Abs(rudder) < RUDDER_CUTOFF) rudder = 0;

		controller.input = new Vector2(0, Throttle);
		controller.rudder = rudder;

		if (TimeLeft <= stopTime) {
			SkipToEnd();
		}
	}

	public virtual bool Play() {
		Playing = true;
		waitUntil = Time.time + startDelay;

		OnPlay?.Invoke();
		return true;
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

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position, transform.position + transform.forward * 20);
		}
	}
}
