using System.Collections;
using UnityEngine;

public abstract class AudioManager : MonoBehaviour {
    public int nChannels = 1;
	public float volume = 1;

    private AudioSource[] channels;
    private int nextChannel;

    protected AudioSource CreateChannel() {
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

    public void PlayUntil(float interval, AudioClip sound, System.Func<bool> predicate, float minDuration = 0, float? volume = null, float spatialBlend = 1) {
        IEnumerator Task() {
            float startTime = Time.time;

            AudioSource channel = ReserveChannel();
            channel.volume = volume.GetValueOrDefault(this.volume);
            channel.spatialBlend = spatialBlend;
            channel.pitch = 1;
            channel.clip = sound;

            while (Time.time - startTime < minDuration || predicate.Invoke()) {
                channel.Play();
                yield return new WaitForSeconds(interval);
            }
        }
        StartCoroutine(Task());
    }

    public void PlayOnce(AudioClip sound, float? volume=null, float spatialBlend = 1, float randomPitchRange = 0) {
        AudioSource channel = ReserveChannel();
		channel.volume = volume.GetValueOrDefault(this.volume);
        channel.spatialBlend = spatialBlend;
		channel.pitch = randomPitchRange == 0 ? 1f : Random.Range(1f - randomPitchRange, 1f + randomPitchRange);
        channel.PlayOneShot(sound);
    }

    public AudioSource PlayInfinite(AudioClip sound) {
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
