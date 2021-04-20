using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private const float 
        MOUSE_SENSITIVITY = 1,
        SCROLL_SENSITIVITY = 20,
        TRANSITION_DURATION = 1, 
        MIN_FOV = 30;

    public Transform[] mounts;
    public float speed;
    private float maxFOV;
    private Camera cam;

    private int? mountI = null;
    private Transform Mount => mountI.HasValue ? mounts[mountI.Value] : null;

    private Vector3 lookRotation;
    private float transition;

    private Vector3 startPos;
    private Quaternion startRot;

    private void Start() {
        lookRotation = transform.eulerAngles;
        cam = GetComponent<Camera>();
        maxFOV = cam.fieldOfView;

        if (mounts.Length > 0) {
            MountTo(0);
            transition = 1;  // Instantly move to mount
        }
    }

    private void Update() {
        for (int i = 0; i < mounts.Length; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                MountTo(i);
                break;
            }
        }

        float switchCam = Input.GetAxisRaw("SwitchCamera");
        if (switchCam != 0) {
            int index = mountI.GetValueOrDefault(-1) + (int)Mathf.Sign(switchCam);
            MountTo(MathTools.Mod(index, mounts.Length));
        }

        if (Input.GetMouseButtonDown(2)) {
            lookRotation = transform.eulerAngles;
            cam.fieldOfView = maxFOV;
            mountI = null;
        }

        if (!mountI.HasValue) {
            Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * MOUSE_SENSITIVITY;
            // Update euler angles as a vector to avoid quaternion clamping issues
            lookRotation += new Vector3(-input.y, input.x);
            lookRotation.x = Mathf.Clamp(lookRotation.x, -89.9f, 89.9f);
            transform.rotation = Quaternion.Euler(lookRotation);

            bool leftClick = Input.GetMouseButton(0), rightClick = Input.GetMouseButton(1);
            if (leftClick || rightClick) {
                transform.position += transform.forward * speed * Time.deltaTime * (leftClick ? -1 : 1);
            }
        }
        else {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll != 0) {
                cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * SCROLL_SENSITIVITY, MIN_FOV, maxFOV);
            }

            transition = Mathf.Clamp01(transition + Time.deltaTime / TRANSITION_DURATION);
            transform.position = Vector3.Lerp(startPos, Mount.position, transition);
            transform.rotation = Quaternion.Lerp(startRot, Mount.rotation, transition);
        }
    }

    private void MountTo(int index) {
        startPos = transform.position;
        startRot = transform.rotation;
        transition = 0;
        mountI = index;

        cam.fieldOfView = maxFOV;
    }
}
