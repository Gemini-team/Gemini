﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Crest;

[RequireComponent(typeof(BoatAlignNormal))]
[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour {
	[HideInInspector]
	public UnityEvent OnControlChange;
	[HideInInspector]
	public CollisionEvent OnCollision = new CollisionEvent();

	private Vector3 prevPos;
	private Rigidbody rb;
	public BoatAlignNormal engine { get; private set; }

	// [HideInInspector]
	public Vector2 input;
	// [HideInInspector]
	public float rudder;

	public float Speed => rb.velocity.magnitude;

	private bool manualControl = true;
	public bool ManualControl {
		get => manualControl;
		set {
			manualControl = value;
			OnControlChange?.Invoke();
		}
	}

	protected virtual void Start() {
		rb = GetComponent<Rigidbody>();
		engine = GetComponent<BoatAlignNormal>();
		prevPos = transform.position;
	}

	protected virtual void Update() {
		// Controls implementation based on Crest.BoatAlignNormal
		Vector3 forcePosition = rb.position + engine._forceHeightOffset * Vector3.up;
		rb.AddForceAtPosition(transform.forward * engine._enginePower * input.y, forcePosition, ForceMode.Acceleration);
		rb.AddForceAtPosition(transform.right * engine._enginePower * input.x, forcePosition, ForceMode.Acceleration);
		rb.AddTorque(transform.up * engine._turnPower * rudder, ForceMode.Acceleration);		
	}

	private void LateUpdate() {
		prevPos = transform.position;
	}

	private void OnCollisionEnter(Collision collision) {
		OnCollision?.Invoke(collision);
	}
}
