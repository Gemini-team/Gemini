using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDetectionWidget : MonoBehaviour {
    public const float SIZE_MULTIPLIER = 1.5f;

    public Renderer target;
    private Camera cam;

    private RectTransform rt;

    private void Start() {
        if (PlayerPrefs.HasKey("ObjectDetection") && PlayerPrefs.GetInt("ObjectDetection") == 0) {
            enabled = false;
            return;
        }

        cam = Camera.main;
        rt = GetComponent<RectTransform>();
    }

    private void Update() {
        Vector2 min = cam.WorldToScreenPoint(target.bounds.min);
        Vector2 max = cam.WorldToScreenPoint(target.bounds.max);

        rt.localPosition = (min + max) * 0.5f;

        Vector2 diff = max - min;
        rt.sizeDelta = new Vector2(Mathf.Abs(diff.x), Mathf.Abs(diff.y)) * SIZE_MULTIPLIER;
    }
}
