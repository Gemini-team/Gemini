using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

public abstract class AnimatedTrip : MonoBehaviour {
    [HideInInspector]
    public UnityEvent OnPlay, OnStop, OnEndReached;

    public PathCreator route;
    public float minSpeed = 1, maxSpeed = 2;
    [Range(0, 0.5f)]
    public float ease = 0.1f;
    public float startDelay = 2;
    [HideInInspector]
    public float speedScale = 1;

    public bool reverse { get; protected set; }
    private bool playing;
    private float waitUntil;

    public float distanceTravelled { get; private set; }
    public float DistanceRemaining => route.path.length - distanceTravelled;
    public bool EndReached => DistanceRemaining <= 0;
    public float Speed {
        get {
            float speed = maxSpeed;
            if (ease > 0) {
                float easeTime = route.path.GetClosestTimeOnPath(transform.position);
                if (easeTime > 0.5) easeTime = 1 - easeTime;
                speed = (maxSpeed - minSpeed) * Mathf.Clamp01(easeTime / ease) + minSpeed;
            }
            return speed * speedScale;
        }
    }

    public virtual bool Playing {
        get => playing;
        set => playing = value;
    }

    protected virtual void Start() {
        Step();  // Move to start of path
    }

    protected virtual void Update() {
        if (!Playing || Time.time < waitUntil) return;

        distanceTravelled += Speed * Time.deltaTime;
        Step();

        if (EndReached) {
            SkipToEnd();
        }
    }

    private void Step() {
        float dist = distanceTravelled;
        if (reverse) dist = route.path.length - dist;

        transform.position = route.path.GetPointAtDistance(dist, EndOfPathInstruction.Stop);
        transform.rotation = route.path.GetRotationAtDistance(dist, EndOfPathInstruction.Stop);
    }

    public virtual void Play() {
        Playing = true;
        distanceTravelled = 0;
        Step();
        waitUntil = Time.time + startDelay;

        OnPlay?.Invoke();
    }

    public virtual void Stop() {
        Playing = false;
        distanceTravelled = 0;
        Step();
        
        OnStop?.Invoke();
    }

    public virtual void SkipToEnd() {
        Stop();

        OnEndReached?.Invoke();
    }
}
