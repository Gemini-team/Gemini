using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamWidget : MonoBehaviour {
    private CameraController cam;
    public int camIndex;

    void Start() {
        GameObject selector = transform.Find("Selector").gameObject;

        cam = Camera.main.GetComponent<CameraController>();
        cam.OnMount.AddListener(i => selector.SetActive(i == camIndex));

        GetComponent<Button>().onClick.AddListener(() => cam.MountTo(camIndex));
    }
}
