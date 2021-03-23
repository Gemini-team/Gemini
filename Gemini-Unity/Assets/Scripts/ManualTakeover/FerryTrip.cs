using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerryTrip : AnimatedTrip {
    [HideInInspector]
    public bool boarding;
    public DockController dock { get; private set; }
    
    public override bool Playing {
        get => base.Playing;
        set {
            base.Playing = value;
            foreach (Animator anim in animators) {
                anim.SetBool("inTransit", value);
                anim.SetBool("reverse", reverse);
            }
        }
    }

    private Animator[] animators;

    protected override void Start() {
        base.Start();
        animators = GetComponentsInChildren<Animator>();
        Dock();
    }

    public override void SkipToEnd() {
        reverse = !reverse;
        base.SkipToEnd();
    }

    public override void Stop() {
        base.Stop();
        Dock();
    }

    private void Dock() {
        float dist = float.MaxValue;
        foreach (DockController dock in FindObjectsOfType<DockController>()) {
            float dist_ = Vector3.Distance(transform.position, dock.transform.Find("DockingArea").position);
            if (dist_ < dist) {
                this.dock = dock;
                dist = dist_;
            }
        }
    }

    public override void Play() {
        if (boarding) return;

        base.Play();
        dock = null;
    }
}
