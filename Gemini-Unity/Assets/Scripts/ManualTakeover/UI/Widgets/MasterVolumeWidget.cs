using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterVolumeWidget : MonoBehaviour {
    [Range(0f, 1f)]
    public float defaultVolume = 0.5f;

    private Slider slider;
    private Text text;

    private void Start() {
        slider = GetComponentInChildren<Slider>();
        text = transform.Find("Text").GetComponent<Text>();

        slider.onValueChanged.AddListener(UpdateVolume);
        UpdateVolume(PlayerPrefs.HasKey("MasterVolume") ? PlayerPrefs.GetFloat("MasterVolume") : defaultVolume);
    }

    private void UpdateVolume(float volume) {
        slider.value = volume;
        volume = Mathf.Clamp01(volume);
        text.text = Mathf.RoundToInt(volume * 100) + "%";
        PlayerPrefs.SetFloat("MasterVolume", volume);
        AudioListener.volume = volume;
    }
}
