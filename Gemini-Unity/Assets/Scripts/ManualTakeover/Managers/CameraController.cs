using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraController : MonoBehaviour {
	[System.Serializable]
	public class Mount {
		public Transform anchor = null;
		public bool canPan = true;
	}

    private const float 
        AERIAL_CAM_DISTANCE = 25,
        MIN_FOV = 30,
        ZOOM_SPEED = 5,
		PAN_SPEED = 50,
		PAN_RANGE = 179;

	[HideInInspector]
	[SerializeField]
	public List<Mount> mounts;

    public float speed;
	public int startMount;
	public bool enableAerialCam = false;

	private float maxFOV;
    private Camera cam;

	[HideInInspector]
	public UnityEvent OnMount;

	public int? MountI { get; private set; } = null;
	public Mount MountedTo => MountI.HasValue ? mounts[MountI.Value] : null;
    public bool IsMounted => MountI.HasValue;

    private Transform ferry;
	private Vector3 panEuler;
	private int switchCam;

    private void Start() {
        ferry = GameObject.FindGameObjectWithTag("Player").transform;
        cam = GetComponent<Camera>();
        maxFOV = cam.fieldOfView;

		startMount = Mathf.Max(startMount, 0);
		MountTo(startMount);
    }

    private void Update() {
        for (int i = 0; i < mounts.Count; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                MountTo(i);
                break;
            }
        }

		int switchCam_ = MathTools.Sign(Input.GetAxisRaw("SwitchCamera"));
		if (switchCam == 0 && switchCam_ != 0) {
			int index = MathTools.Mod(MountI.GetValueOrDefault(0) + switchCam_, mounts.Count);
			MountTo(index);
        }
		switchCam = switchCam_;

        if (enableAerialCam && Input.GetKeyDown(KeyCode.Alpha0)) {
            MountTo(null);
        }

		if (IsMounted) {
			if (Input.GetButtonDown("CameraReset")) {
				MountTo(MountI);
            }

			if (MountedTo.canPan) {
				float lim = PAN_RANGE / 2,
					panX = Input.GetAxisRaw("CameraPanX"),
					panY = Input.GetAxisRaw("CameraPanY");

				panEuler += new Vector3(-panY, panX, 0) * Time.deltaTime * PAN_SPEED;
				panEuler = new Vector3(Mathf.Clamp(panEuler.x, -lim, lim), Mathf.Clamp(panEuler.y, -lim, lim), 0);

				transform.localRotation = Quaternion.Euler(panEuler);
			}
		} else {
			transform.position = ferry.position + Vector3.up * AERIAL_CAM_DISTANCE;
			transform.rotation = Quaternion.Euler(90, ferry.eulerAngles.y, 0);
		}

		float zoomDelta = Input.GetAxisRaw("CameraZoom");
		if (!Mathf.Approximately(zoomDelta, 0)) {
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - zoomDelta * ZOOM_SPEED, MIN_FOV, maxFOV);
		}
	}

    public void MountTo(int? index) {
        MountI = index;

        cam.fieldOfView = maxFOV;
        transform.parent = MountedTo?.anchor;

        if (index.HasValue) {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
			panEuler = Vector3.zero;
        }

        OnMount?.Invoke();
    }
}
