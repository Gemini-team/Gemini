using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputMapper : MonoBehaviour {
	public Transform root;

	private Dictionary<string, Text> inputs = new Dictionary<string, Text>();
	private string mapping = null;

	private void Start() {
		foreach (Transform obj in root) {
			obj.GetComponent<Button>().onClick.AddListener(() => {
				inputs[obj.name].text = "<Press key>";
				mapping = obj.name;
			});

			inputs[obj.name] = obj.Find("Input").GetComponent<Text>();
			inputs[obj.name].text = FerryInput.GetBinding(obj.name).ToString();
		}
	}

	private void Update() {
		if (mapping != null) {
			for (int i = 0; i < System.Enum.GetNames(typeof(KeyCode)).Length; i++) {
				KeyCode kc = (KeyCode)i;
				if (Input.GetKeyDown(kc)) {
					FerryInput.SetBinding(mapping, kc);
					inputs[mapping].text = kc.ToString();
					mapping = null;
					break;
				}
			}
		}
	}

	public void ResetToDefault() {
		foreach (Transform obj in root) {
			KeyCode kc = FerryInput.defaultBindings[obj.name];
			FerryInput.SetBinding(obj.name, kc);
			inputs[obj.name].text = kc.ToString();
		}
	}
}
