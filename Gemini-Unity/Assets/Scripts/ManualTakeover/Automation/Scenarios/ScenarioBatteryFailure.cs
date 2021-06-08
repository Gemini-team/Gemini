using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioBatteryFailure : Scenario {
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
			Schedule(ManualTakeover, failureDelay);
		}
	}
}
