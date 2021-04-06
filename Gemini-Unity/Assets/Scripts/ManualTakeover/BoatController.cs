﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController : MonoBehaviour {
	public const float DEAD = 0.1f;
	public const float MIN_TURN_SPEED = 0.5f;
	public const float STOP_DISTANCE = 1f;
	public const float LOOK_AHEAD = 10f;

	public float acceleration = 1, turnSpeed = 5, maxSpeed = 10;
	public bool reversing;
	
	private Vector3? destination = null;
	public Vector3? Destination {
		get => destination;
		set {
			if (!manualControl) destination = value;
        }
    }

	private bool manualControl;
	public bool ManualControl {
		get => manualControl;
		set {
			manualControl = value;
			if (value) {
				destination = null;
            }
        }
    }

	[HideInInspector]
	public Quaternion targetRotation;

	private Rigidbody rb;

	private Vector3 HeadingDirection => transform.forward * (reversing ? -1 : 1);
	private Quaternion HeadingRotation => Quaternion.LookRotation((destination.Value - transform.position) * (reversing ? -1 : 1));

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() {
		// TODO: Implement manual control

		float mag = rb.velocity.magnitude;

		if (destination.HasValue) {
			targetRotation = HeadingRotation;
			rb.AddForce(HeadingDirection * acceleration);

			if (mag > maxSpeed) {
				rb.velocity *= maxSpeed / mag;
				mag = maxSpeed;
			}
		} else {
			rb.velocity = Vector3.zero;
		}
		
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Mathf.Max(turnSpeed * (mag / maxSpeed), MIN_TURN_SPEED) * Time.deltaTime);
    }
}