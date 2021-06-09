using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component should be placed on the compass needle
/// </summary>
public class CompassWidget : MonoBehaviour {
    private const float NORTH = -30;

    private Transform ferry;

    private void Start() {
        ferry = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update() {
        transform.rotation = Quaternion.Euler(0, 0, NORTH - ferry.eulerAngles.y);
    }
}
