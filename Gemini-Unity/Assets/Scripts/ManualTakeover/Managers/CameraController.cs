using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    private const float LERP_SPEED = 3f;

    public Transform[] mounts;
    public float speed, mouseSensitivity;

    private Transform mount;

    private void Update() {
        for (int i = 0; i < mounts.Length; i++) {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                mount = mounts[i];
                break;
            }
        }

        if (Input.GetMouseButtonDown(2)) {
            mount = null;
        }

        if (mount == null) {
            Vector2 input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            transform.rotation = Quaternion.Euler(Mathf.Clamp(transform.eulerAngles.x - input.y, -89.9f, 89.8f), transform.eulerAngles.y + input.x, 0);

            bool leftClick = Input.GetMouseButton(0), rightClick = Input.GetMouseButton(1);
            if (leftClick || rightClick) {
                transform.position += transform.forward * speed * Time.deltaTime * (leftClick ? -1 : 1);
            }
        } else {
            transform.position = Vector3.Lerp(transform.position, mount.position, LERP_SPEED * Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, mount.rotation, LERP_SPEED * Time.deltaTime);
        }
    }
}
