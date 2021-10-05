using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamWidget : MonoBehaviour {
    private CameraController cam;
    public int camIndex;

	private bool Active => camIndex == cam.MountI;

    void Start() {
        GameObject selector = transform.Find("Selector").gameObject;

        cam = Camera.main.GetComponent<CameraController>();
        cam.OnMount.AddListener(() => selector.SetActive(Active));

        GetComponent<Button>().onClick.AddListener(() => cam.MountTo(camIndex));
    }

	private void Update() {
		transform.localRotation = Quaternion.Euler(0, 0, 
			(Active ? -cam.transform.localEulerAngles.y : 0) + cam.mounts[camIndex].anchor.localEulerAngles.y);
	}
}
