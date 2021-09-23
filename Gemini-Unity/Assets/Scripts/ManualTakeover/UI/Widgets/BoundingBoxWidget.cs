using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoundingBoxWidget : MonoBehaviour {
    public GameObject target;
    private Camera cam;

    private Image image;
    private RectTransform rt;

    private IEnumerable<Vector3> BoundingVertices(Bounds bounds) {
        for (int z = -1; z <= 1; z++) {
            for (int y = -1; y <= 1; y++) {
                for (int x = -1; x <= 1; x++) {
                    if (x * y * z == 0) continue;

                    Vector3 v = new Vector3(x, y, z);
                    v.Scale(bounds.extents);

                    yield return bounds.center + v;
                }
            }
        }
    }

    private void Start() {
        cam = Camera.main;
        rt = GetComponent<RectTransform>();
        image = GetComponent<Image>();
    }

    private void Update() {
        Vector3 dir = target.transform.position - cam.transform.position;
        if (Vector3.Dot(dir, cam.transform.forward) < 0) {
            image.enabled = false;
            return;  // Don't draw bounding box if object is behind camera
        }
        image.enabled = true;

        Quaternion rot = target.transform.rotation;
        target.transform.rotation = Quaternion.identity;

        Bounds oobb = new Bounds(target.transform.position, Vector3.zero);
        foreach (Renderer renderer in target.GetComponentsInChildren<Renderer>()) {
            oobb.Encapsulate(renderer.bounds);
        }

        Bounds bounds = new Bounds(cam.WorldToScreenPoint(oobb.center), Vector3.zero);
        foreach (Vector3 p in BoundingVertices(oobb)) {
            bounds.Encapsulate(cam.WorldToScreenPoint(oobb.center + rot * (p - oobb.center)));
        }

        rt.anchoredPosition = bounds.center;
        rt.sizeDelta = bounds.size;

        target.transform.rotation = rot;
    }
}
