﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FerryController : BoatController {
	private const float DOCK_DIST_LIMIT = 2.5f, DOCK_ALIGN_THRESHOLD = 0.95f;

	[HideInInspector]
	public MessageEvent DockMessage = new MessageEvent();
	[HideInInspector]
	public UnityEvent OnConnectToDock, OnDisconnectFromDock, OnControlChange;
	[HideInInspector]

	public DockController AtDock { get; private set; }
	public DockController DestinationDock { get; private set; }
	public Vector3 DockPos(DockController dock) => dock.transform.Find("DockingArea").position;
	public int DockDirection => AtDock == null ? 0 : (int)Mathf.Sign(Vector3.Dot(transform.position - DockPos(AtDock), transform.forward));
	public float RemainingDistance => Vector3.Distance(transform.position, DockPos(DestinationDock));

	private bool manualControl = true;
	public bool ManualControl {
		get => manualControl;
		set {
			manualControl = value;
			OnControlChange?.Invoke();
		}
	}

	[HideInInspector]
	public bool boarding;

	private Animator[] animators;

	private void UpdateDestination() {
		DestinationDock = ClosestDock(dock => !dock.Equals(AtDock));
	}

	private void UpdateAnimators(bool inTransit) {
		foreach (Animator anim in animators) {
			anim.SetBool("inTransit", inTransit);
			anim.SetBool("reverse", DockDirection == -1);
		}
	}

	private DockController ClosestDock(System.Func<DockController, bool> predicate = null) {
		DockController closestDock = null;
		float dist = float.MaxValue;

		foreach (DockController dock in FindObjectsOfType<DockController>()) {
			if (predicate == null || predicate.Invoke(dock)) {

				float dist_ = Vector3.Distance(transform.position, DockPos(dock));
				if (dist_ < dist) {
					closestDock = dock;
					dist = dist_;
				}
			}
		}

		return closestDock;
	}

	public bool TryConnectToDock() {
		DockController dock = ClosestDock();

		if (Vector3.Distance(transform.position, DockPos(dock)) > DOCK_DIST_LIMIT) {
			DockMessage.Invoke("Docking failed (too far away)");
			return false;
		}

		float alignment = Mathf.Abs(Vector3.Dot(dock.transform.Find("DockingArea").forward, transform.forward));
		if (alignment < DOCK_ALIGN_THRESHOLD) {
			DockMessage.Invoke("Docking failed (not aligned)");
			return false;
		}

		if (!dock.Equals(DestinationDock)) {
			DockMessage.Invoke("Docking failed (incorrect dock)");
			return false;
		}

		// Dock to destination, then find next destination
		AtDock = DestinationDock;
		UpdateDestination();

		UpdateAnimators(inTransit: false);
		DockMessage.Invoke("Docking successful");

		OnConnectToDock?.Invoke();
		return true;
	}

	public bool TryDisconnectFromDock() {
		if (boarding || AtDock == null) return false;

		AtDock = null;
		UpdateAnimators(inTransit: true);

		OnDisconnectFromDock?.Invoke();
		return true;
	}

	protected override void Start() {
		base.Start();

		animators = GetComponentsInChildren<Animator>();

		UpdateDestination();
		if (!TryConnectToDock()) {
			Debug.LogError("Docking failed");
		}
	}

	protected override void Update() {
		if (ManualControl) {
			input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			rudder = Input.GetAxis("Rudder");

			if (!boarding && Input.GetButtonDown("Dock")) {
				if (AtDock == null) TryConnectToDock();
				else TryDisconnectFromDock();
			}
		}

		if (AtDock == null) {
			base.Update();
		}
	}
}
