using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private const float TRANSITION_DURATION = 1;

    public Transform[] mounts;
    public float speed, mouseSensitivity;

    private Transform mount;
    private Vector3 lookRotation;
    private float transition;

    private Vector3 startPos;
    private Quaternion startRot;

    private void Start() {
        lookRotation = transform.eulerAngles;
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

        if (Input.GetMouseButtonDown(2)) {
            lookRotation = transform.eulerAngles;
            mount = null;
        }

        if (mount == null) {
            Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
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
            transition = Mathf.Clamp01(transition + Time.deltaTime / TRANSITION_DURATION);
            transform.position = Vector3.Lerp(startPos, mount.position, transition);
            transform.rotation = Quaternion.Lerp(startRot, mount.rotation, transition);
        }
    }

    private void MountTo(int index) {
        startPos = transform.position;
        startRot = transform.rotation;
        transition = 0;
        mount = mounts[index];
    }
}
