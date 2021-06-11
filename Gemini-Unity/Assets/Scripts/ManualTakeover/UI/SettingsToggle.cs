using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SettingsToggle : MonoBehaviour {
    public bool defaultState;
    public string key;

    private void Start() {
        if (!PlayerPrefs.HasKey(key)) {
            PlayerPrefs.SetInt(key, defaultState ? 1 : 0);
        }

        Toggle toggle = GetComponent<Toggle>();
        toggle.isOn = PlayerPrefs.GetInt(key) == 1;
        toggle.onValueChanged.AddListener(state => PlayerPrefs.SetInt(key, state ? 1 : 0));
    }
}
