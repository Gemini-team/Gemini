using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeScreenshot : MonoBehaviour {
    private const string ROOT = "./screenshots/";

    public void Update() {
        if (Input.GetKeyDown(KeyCode.F2)) {
			if (!System.IO.Directory.Exists(ROOT)) {
				System.IO.Directory.CreateDirectory(ROOT);
			}

			string path = System.IO.Path.GetFullPath(ROOT + System.DateTime.Now + ".png");
            ScreenCapture.CaptureScreenshot(path);
            Debug.Log("Screenshot saved at " + path);
        }
    }
}
