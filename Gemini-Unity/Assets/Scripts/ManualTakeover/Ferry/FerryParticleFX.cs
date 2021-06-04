using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryParticleFX : MonoBehaviour {
	private ParticleSystem smoke;

	private void Start() {
		smoke = transform.Find("Smoke").GetComponent<ParticleSystem>();
		FindObjectOfType<Scenario>().OnManualTakeover.AddListener(smoke.Play);
	}
}
