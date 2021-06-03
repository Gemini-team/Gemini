using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryAudio : AudioManager {
    private const float MIN_ENGINE_PITCH = 0.75f, MAX_ENGINE_PITCH = 1f, MIN_IMPULSE = 0.5f, MAX_IMPULSE = 2.5f;

    public AudioClip takeoverSound, engineSound, impactSound;

	private AudioSource engineChannel;
    private FerryController ferry;

    protected override void AudioSetup() {
        Scenario scenario = FindObjectOfType<Scenario>();
        ferry = FindObjectOfType<FerryController>();

		engineChannel = PlayInfinite(engineSound);

		ferry.OnCollision.AddListener(collision => {
			float volume = Mathf.Clamp01((collision.relativeVelocity.magnitude - MIN_IMPULSE) / (MAX_IMPULSE - MIN_IMPULSE));
			if (volume > 0) {
				PlayOnce(impactSound, volume);
			}
		});

        scenario.OnManualTakeover.AddListener(() => {
            PlayUntil(1f, takeoverSound, () => scenario.Ferry.input == Vector2.zero, minDuration: 3);
        });
    }

    private void Update() {
        engineChannel.pitch = MIN_ENGINE_PITCH + (MAX_ENGINE_PITCH - MIN_ENGINE_PITCH) * ferry.Speed / ferry.maxSpeed;
    }
}
