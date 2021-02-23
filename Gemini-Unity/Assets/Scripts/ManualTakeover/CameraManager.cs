using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera[] viewpoints;
    private int activeIndex = -1;

    // Start is called before the first frame update
    private void Start()
    {
        SetViewpoint(0);
    }

    private void SetViewpoint(int index)
    {
        if (index == activeIndex) return;

        void Toggle(Camera cam_, bool state)
        {
            cam_.enabled = state;
            cam_.GetComponent<AudioListener>().enabled = state;
        }

        activeIndex = index;
        foreach (Camera camera in viewpoints) {
            Toggle(camera, false);
        }
        Toggle(viewpoints[index], true);
    }

    // Update is called once per frame
    private void Update()
    {
        for (int i = 0; i < viewpoints.Length; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                SetViewpoint(i);
            }
        }
    }
}
