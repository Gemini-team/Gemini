using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Crest;

//[RequireComponent(typeof(BoatAlignNormal))]
[RequireComponent(typeof(Rigidbody))]
public class BoatController : MonoBehaviour {
	[HideInInspector]
	public CollisionEvent OnCollision = new CollisionEvent();

	private Vector3 prevPos;
	private Rigidbody rb;
	public BoatProbes engine { get; private set; }

	// [HideInInspector]
	public Vector2 input;
	// [HideInInspector]
	public float rudder;

	public float Speed => rb.velocity.magnitude;

	protected virtual void Start() {
		rb = GetComponent<Rigidbody>();
		engine = GetComponent<BoatProbes>();
		prevPos = transform.position;

		engine._playerControlled = false;
	}

	protected virtual void Update() {
		Vector3 forcePosition = transform.position + engine._forceHeightOffset * Vector3.up;
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
