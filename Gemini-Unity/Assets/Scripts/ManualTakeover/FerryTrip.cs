using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryTrip : AnimatedTrip {
    public FerryController ferry { get; private set; }

    protected override void Start() {
        base.Start();
        ferry = GetComponent<FerryController>();
    }

    public override void SkipToEnd() {
        reverse = !reverse;
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
