using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryAudio : AudioManager {
    private const float MIN_ENGINE_PITCH = 0.75f, MAX_ENGINE_PITCH = 1f;

    public AudioClip takeoverSound;
    public AudioClip engineSound;

    private AudioSource engineChannel;
    private FerryController ferry;

    protected override void AudioSetup() {
        Scenario scenario = FindObjectOfType<Scenario>();
        ferry = FindObjectOfType<FerryController>();

        scenario.OnManualTakeover.AddListener(() => {
            PlayUntil(1f, takeoverSound, () => scenario.Ferry.input == Vector2.zero, minDuration: 3);
        });

        engineChannel = PlayInfinite(engineSound);
    }

    private void Update() {
        engineChannel.pitch = MIN_ENGINE_PITCH + (MAX_ENGINE_PITCH - MIN_ENGINE_PITCH) * ferry.Speed / ferry.maxSpeed;
    }
}
