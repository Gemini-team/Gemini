using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetectionWidget : MonoBehaviour {
    public const float SIZE_MULTIPLIER = 1.5f;

    public Renderer target;
    private Camera cam;

    private RectTransform rt;

    private void Start() {
        cam = Camera.main;
        rt = GetComponent<RectTransform>();
    }

    private void Update() {
        Vector2 min = cam.WorldToScreenPoint(target.bounds.min);
        Vector2 max = cam.WorldToScreenPoint(target.bounds.max);

        rt.localPosition = (min + max) * 0.5f;
        rt.sizeDelta = (max - min) * SIZE_MULTIPLIER;
    }
}
