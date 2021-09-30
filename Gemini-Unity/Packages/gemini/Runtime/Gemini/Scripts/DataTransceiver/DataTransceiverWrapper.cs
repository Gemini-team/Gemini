using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class DataTransceiverWrapper : MonoBehaviour {

    // The plugin referenced by `DLLImport` must be compiled
    // and placed in your project's Assets/Plugins/ folder.

    // Make sure that you rename the lib<name>.dylib to
    // lib<name>.bundle so that Unity can find it.

    [DllImport("data_transceiver")]
    private static extern int double_input(int x);

    [DllImport("data_transceiver")]
    private static extern int triple_input(int x);

    [DllImport("data_transceiver")]
    private static extern uint send_external(byte[] byteArr);

    [DllImport("data_transceiver")]
    private static extern void listen_external();

    void Start() {
        //listen_external();
    }

    void Update() {
        byte[] buf = new byte[1]; 
        buf[0] = 0xFF;

        uint res = send_external(buf);
        Debug.Log(res);
    }
}