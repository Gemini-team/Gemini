using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

public abstract class FollowPath : MonoBehaviour {
    public PathCreator route;
    [Range(0, 0.5f)]
    public float easeIn = 0, easeOut = 0;
    public float stopTime = 0.001f, minThrottle = 0.005f;

    [HideInInspector]
    public float throttleScale = 1;
    public bool Reversed { get; private set; }

    private bool playing;

    [HideInInspector]
    public UnityEvent OnPlay, OnStop, OnEndReached;

    public virtual bool Playing {
        get => playing;
        set => playing = value;
    }

    public float TimeLeft {
        get {
            float closestTime = route.path.GetClosestTimeOnPath(transform.position);
            return Reversed ? closestTime : 1 - closestTime;
        }
    }

    protected float Throttle {
        get {
            float throttle = 1;

            if (TimeLeft >= 0.5f) {
                if (easeIn > 0) {
                    throttle = (1 - TimeLeft) / easeIn;
                }
            }
            else {
                if (easeOut > 0) {
                    throttle = TimeLeft / easeOut;
                }
            }

            return Mathf.Clamp(throttle * throttleScale, minThrottle, 1);
        }
    }

    protected virtual void Start() { }

    private void Update() {
        if (!Playing) return;

        Move();

        if (TimeLeft <= stopTime) {
            SkipToEnd();
        }
    }

    protected abstract void Move();

    public virtual bool Play() {
        Playing = true;

        OnPlay?.Invoke();
        return true;
    }

    public virtual void Stop() {
        Playing = false;
        OnStop?.Invoke();
    }

    public virtual void SkipToEnd() {
        Stop();

        Reversed = !Reversed;
        OnEndReached?.Invoke();
    }
}
