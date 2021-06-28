using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryAutopilot : BoatAutopilot {
	public FerryController ferry { get; private set; }

	protected override void Start() {
		base.Start();
		ferry = GetComponent<FerryController>();
	}

	public override void Stop() {
		base.Stop();
		if (!ferry.TryConnectToDock()) {
			Debug.LogError("Docking failed");
		}
	}

	public override bool Play() {
		if (ferry.ManualControl) {
			Debug.LogError("Manual control engaged");
			return false;
		}
		base.Play();

		if (!ferry.TryDisconnectFromDock()) {
			Debug.LogError("Departure failed");
			return false;
		}

		return true;
	}
}
