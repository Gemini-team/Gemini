using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioBatteryFailure : Scenario {
	public override string ScenarioName => "BatteryFailure";
    public override string FailureWarning => "Battery failure detected";
    private string FailureImminentWarning => "Battery 2 temperature above 80 \u00b0C";

    [Space(10)]
	public float failureDelay = 15;
	public AudioClip explosionSound;

	protected override void ManualTakeover() {
		if (!Playing) return;

		GameObject ferry = GameObject.FindGameObjectWithTag("Player");
		ferry.GetComponentInChildren<FerryAudio>().PlayOnce(explosionSound);
		ferry.transform.Find("Particles/Smoke").GetComponent<ParticleSystem>().Play();

		base.ManualTakeover();
	}

	protected override void TripStartAction() {
		if (tripCount == 0) {
			OnManualTakeoverImminent?.Invoke(failureDelay, FailureImminentWarning);
			Schedule(ManualTakeover, failureDelay);
		}
	}
}
