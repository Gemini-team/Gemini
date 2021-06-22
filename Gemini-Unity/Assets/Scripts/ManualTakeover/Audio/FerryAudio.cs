using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryAudio : AudioManager {
    private const float MIN_IMPULSE = 0.5f, MAX_IMPULSE = 2.5f, LINGER = 0.2f;

    public AudioClip engineSound, engineTailSound, takeoverSound, impactSound, bellSound;

	private AudioSource engineChannel;
    private FerryController ferry;

    protected override void AudioSetup() {
        Scenario scenario = FindObjectOfType<Scenario>();
		ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();

		engineChannel = PlayInfinite(engineSound);
        engineChannel.Stop();

		ferry.OnCollision.AddListener(collision => {
			float volume = Mathf.Clamp01((collision.relativeVelocity.magnitude - MIN_IMPULSE) / (MAX_IMPULSE - MIN_IMPULSE));
			if (volume > 0) {
				PlayOnce(impactSound, volume, randomPitchRange: 0.2f);
			}
		});

		ferry.OnDisconnectFromDock.AddListener(() => PlayOnce(bellSound));

        scenario.OnManualTakeoverRequired.AddListener(() => {
            PlayUntil(1f, takeoverSound, () => scenario.Ferry.input == Vector2.zero, minDuration: 3, spatialBlend: 0);
        });

        StartCoroutine(EngineSound());
    }

    private IEnumerator EngineSound() {
        while (true) {
            yield return new WaitUntil(() => ferry.ReceivingInput);

            engineChannel.Play();
            while (ferry.ReceivingInput) {
                yield return new WaitForSeconds(LINGER);
            }

            engineChannel.Stop();
            engineChannel.PlayOneShot(engineTailSound);
            yield return new WaitForSeconds(engineTailSound.length);
        }
    }
}
