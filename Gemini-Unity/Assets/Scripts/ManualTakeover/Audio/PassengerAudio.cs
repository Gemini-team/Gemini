using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PassengerAudio : AudioManager {
	public AudioClip footsteps;

	private AudioSource channel;
	private NavMeshAgent agent;
	private Vector3 prevPos;

	protected override void AudioSetup() {
		prevPos = transform.localPosition;
		agent = GetComponent<NavMeshAgent>();
		channel = PlayInfinite(footsteps);
	}

	private void Update() {
		float speed = (transform.localPosition - prevPos).magnitude / Time.deltaTime;
		channel.volume = speed / agent.speed;
	}

	private void LateUpdate() {
		prevPos = transform.localPosition;
	}
}
