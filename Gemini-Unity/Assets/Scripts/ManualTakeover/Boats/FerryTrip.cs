using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryTrip : AnimatedTrip {
	public FerryController ferry { get; private set; }

	protected override void Start() {
		base.Start();
		ferry = GetComponent<FerryController>();
	}

	public override void SkipToEnd() {
		base.SkipToEnd();
		reverse = !reverse;
	}

	public override void Stop() {
		base.Stop();
		if (!ferry.TryConnectToDock()) {
			Debug.LogError("Docking failed");
		}
	}

	public override bool Play() {
		if (!base.Play()) return false;

		if (!ferry.TryDisconnectFromDock()) {
			Debug.LogError("Departure failed");
		}
		return true;
	}
}
