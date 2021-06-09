using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KnotsWidget : MonoBehaviour {
    private const float MPS_TO_KNOTS = 1.94384449f;
    private Text text;
    private FerryController ferry;

    // Start is called before the first frame update
    private void Start() {
        ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update() {
        text.text = System.Math.Round(ferry.Speed * MPS_TO_KNOTS, 1) + " kn";
    }
}
