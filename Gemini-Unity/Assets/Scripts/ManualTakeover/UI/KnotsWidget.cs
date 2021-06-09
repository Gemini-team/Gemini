using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnotsWidget : MonoBehaviour {
	private const float MPS_TO_KNOTS = 1.94384449f, UPDATE_INTERVAL = 0.5f;
	private Text text;
	private FerryController ferry;

	private void Start() {
		ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
		text = GetComponent<Text>();

		StartCoroutine(Draw());
	}

	private IEnumerator Draw() {
		while (true) {
			text.text = System.Math.Round(ferry.Speed * MPS_TO_KNOTS, 1) + " kn";
			yield return new WaitForSeconds(UPDATE_INTERVAL);
		}
	}
}
