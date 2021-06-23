﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour {
    private const float 
        MOUSE_SENSITIVITY = 1,
        SCROLL_SENSITIVITY = 20,
        SPEED_MULT = 2,
        MIN_FOV = 30;

    public Transform[] mounts;
    public float speed;
	public int startMount;
	public bool enableFreeCam;
    private float maxFOV;
    private Camera cam;

    [HideInInspector]
    public MaybeIntEvent OnMount = new MaybeIntEvent();

	public int? MountI { get; private set; } = null;
    public Transform Mount => MountI.HasValue ? mounts[MountI.Value] : null;
    public bool FreeCam => !MountI.HasValue;

    private Vector3 lookRotation;

    private void Start() {
        lookRotation = transform.eulerAngles;
        cam = GetComponent<Camera>();
        maxFOV = cam.fieldOfView;

		if (!enableFreeCam) startMount = Mathf.Max(startMount, 0);
		MountTo(startMount);
    }

    private void Update() {
        for (int i = 0; i < mounts.Length; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                MountTo(i);
                break;
            }
        }

		float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
		if (scroll != 0) {
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - scroll * SCROLL_SENSITIVITY, MIN_FOV, maxFOV);
		}

		if (enableFreeCam) {
			Vector2 look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * MOUSE_SENSITIVITY;
			Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("CameraYaw"), Input.GetAxis("Vertical"));
			float switchCam = Input.GetAxisRaw("SwitchCamera");

			if (switchCam != 0) {
				int index = MountI.GetValueOrDefault(-1) + (int)Mathf.Sign(switchCam);
				MountTo(MathTools.Mod(index, mounts.Length));
			}

			if (enableFreeCam && !FreeCam && move != Vector3.zero) {
				MountTo(null);
			}

			if (FreeCam) {
				// Update euler angles as a vector to avoid quaternion clamping issues
				lookRotation += new Vector3(-look.y, look.x);
				lookRotation.x = Mathf.Clamp(lookRotation.x, -89.9f, 89.9f);
				transform.rotation = Quaternion.Euler(lookRotation);

				transform.position += transform.rotation * move * speed * (Input.GetKey(KeyCode.LeftShift) ? SPEED_MULT : 1);
			}
		}
	}

    public void MountTo(int? index) {
        MountI = index;

        cam.fieldOfView = maxFOV;
        transform.parent = Mount;

        if (index.HasValue) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        } else {
            lookRotation = transform.eulerAngles;
        }

        OnMount?.Invoke(index);
    }
}
