using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryAudio : AudioManager {
    public AudioClip takeoverSound;

    protected override void AudioSetup() {
        Scenario scenario = FindObjectOfType<Scenario>();

        scenario.OnManualTakeover.AddListener(() => {
            PlayUntil(1f, takeoverSound, () => scenario.Ferry.input == Vector2.zero, minDuration: 3);
        });
    }
}
