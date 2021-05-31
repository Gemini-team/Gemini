using System.Collections;
using UnityEngine;

public abstract class AudioManager : MonoBehaviour {
    public int nChannels = 1;

    private AudioSource[] channels;
    private int nextChannel;

    protected AudioSource ReserveChannel() {
        AudioSource channel = channels[nextChannel];
        nextChannel = (nextChannel + 1) % nChannels;
        return channel;
    }

    protected void PlayUntil(float interval, AudioClip sound, System.Func<bool> predicate, float minDuration = 0) {
        IEnumerator Task() {
            float startTime = Time.time;

            AudioSource channel = ReserveChannel();
            channel.clip = sound;

            while (Time.time - startTime < minDuration || predicate.Invoke()) {
                channel.Play();
                yield return new WaitForSeconds(interval);
            }
        }
        StartCoroutine(Task());
    }

    protected void PlayOnce(AudioClip sound) {
        AudioSource channel = ReserveChannel();
        channel.PlayOneShot(sound);
    }
    private void Start() {
        channels = new AudioSource[nChannels];

        for (int i = 0; i < channels.Length; i++) {
            channels[i] = gameObject.AddComponent<AudioSource>();
            channels[i].playOnAwake = false;
            channels[i].spatialBlend = 1;
        }

        AudioSetup();
    }

    protected abstract void AudioSetup();
}
