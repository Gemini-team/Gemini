using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryTrip : AnimatedTrip {
    private Camera mountedCamera;
    public FerryController ferry { get; private set; }

    protected override void Start() {
        base.Start();
        ferry = GetComponent<FerryController>();
        mountedCamera = GetComponentInChildren<Camera>();
        ferry.TryConnectToDock();
    }

    public override void SkipToEnd() {
        reverse = !reverse;
        mountedCamera.transform.localRotation = Quaternion.Euler(0, reverse ? 180 : 0, 0);
        base.SkipToEnd();
    }

    public override void Stop() {
        base.Stop();
        ferry.TryConnectToDock();
    }

    public override void Play() {
        if (!ferry.TryDisconnectFromDock()) return;
        base.Play();
        
    }
}
