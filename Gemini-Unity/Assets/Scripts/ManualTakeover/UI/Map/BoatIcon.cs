using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatIcon : MonoBehaviour {
    private const float DRAW_INTERVAL = 1 / 60;

    public Camera cam;
    public GameObject target;

    private RectTransform rt;
    private RectTransform container;

    public void Start() {
        if (target == null) {
            target = GameObject.FindGameObjectWithTag("Player");
        }

        container = transform.parent.GetComponent<RectTransform>();
        rt = GetComponent<RectTransform>();

        StartCoroutine(Draw());
    }

    private Vector2 WorldToMapPoint(Vector3 worldPos) {
        Vector2 p = cam.WorldToViewportPoint(worldPos);
        return new Vector2(p.x * container.rect.width, p.y * container.rect.height);
    }

    private IEnumerator Draw() {
        while (true) {
            rt.anchoredPosition = WorldToMapPoint(target.transform.position);
            transform.localRotation = Quaternion.Euler(0, 0, -target.transform.eulerAngles.y);

            yield return new WaitForSeconds(DRAW_INTERVAL);
        }
    }
}
