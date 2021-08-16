using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour {
    private const float 
        SCROLL_SENSITIVITY = 20,
        AERIAL_CAM_DISTANCE = 25,
        MIN_FOV = 30;

    public Transform[] mounts;
    public float speed;
	public int startMount;
	public bool enableAerialCam;
    private float maxFOV;
    private Camera cam;

    [HideInInspector]
    public MaybeIntEvent OnMount = new MaybeIntEvent();

	public int? MountI { get; private set; } = null;
    public Transform Mount => MountI.HasValue ? mounts[MountI.Value] : null;
    public bool IsMounted => MountI.HasValue;

    private Transform ferry;

    private void Start() {
        ferry = GameObject.FindGameObjectWithTag("Player").transform;
        cam = GetComponent<Camera>();
        maxFOV = cam.fieldOfView;

		startMount = Mathf.Max(startMount, 0);
		MountTo(startMount);
    }

    private void Update() {
        for (int i = 0; i < mounts.Length; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                MountTo(i);
                break;
            }
        }

        if (enableAerialCam && Input.GetKeyDown(KeyCode.Alpha0)) {
            MountTo(null);
        }

        if (!IsMounted) {
            transform.position = ferry.position + Vector3.up * AERIAL_CAM_DISTANCE;
            transform.rotation = Quaternion.Euler(90, ferry.eulerAngles.y, 0);
        }

		float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
		if (scroll != 0) {
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * SCROLL_SENSITIVITY, MIN_FOV, maxFOV);
		}
	}

    public void MountTo(int? index) {
        MountI = index;

        cam.fieldOfView = maxFOV;
        transform.parent = Mount;

        if (index.HasValue) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }

        OnMount?.Invoke(index);
    }
}
