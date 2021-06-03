using System.Collections;
using UnityEngine;

public abstract class AudioManager : MonoBehaviour {
    public int nChannels = 1;
	public float volume = 1;

    private AudioSource[] channels;
    private int nextChannel;

    private AudioSource CreateChannel() {
        AudioSource channel = gameObject.AddComponent<AudioSource>();
        channel.playOnAwake = false;
        channel.spatialBlend = 1;
		channel.volume = volume;
        return channel;
    }

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

    protected void PlayOnce(AudioClip sound, float? volume=null) {
        AudioSource channel = ReserveChannel();
		channel.volume = volume.GetValueOrDefault(this.volume);
        channel.PlayOneShot(sound);
    }

    protected AudioSource PlayInfinite(AudioClip sound) {
        AudioSource dedicatedChannel = CreateChannel();
        dedicatedChannel.loop = true;
        dedicatedChannel.clip = sound;
        dedicatedChannel.Play();

        return dedicatedChannel;
    }

    private void Start() {
        channels = new AudioSource[nChannels];

        for (int i = 0; i < channels.Length; i++) {
            channels[i] = CreateChannel();
        }

        AudioSetup();
    }

    protected abstract void AudioSetup();
}
