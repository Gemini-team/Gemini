using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transmitter : MonoBehaviour, ITransmitter
{
    // Dictionary containing all the transmitters
    public static Dictionary<string, ITransmitter> transmitters = new Dictionary<string, ITransmitter>();

    // Fields that all transmitters have.
    private Vector3 position = new Vector3();
    private Vector3 velocity = new Vector3();
    private Vector3 angularVelocity = new Vector3();
    private Vector3 angle = new Vector3();

    private Bounds bounds = new Bounds();

    private new string name = "";

    public Vector3 GetAngle()
    {
        return angle;
    }

    public Vector3 GetAngularVelocity()
    {
        //return angularVelocity;
        return RotationUnityToNED(angularVelocity);
    }

    public string GetName()
    {
        return name;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public Vector3 GetVelocity()
    {
        //return velocity;
        return TranslationUnityToNED(velocity);
    }

    public Bounds GetBounds()
    {
        return bounds;
    }

    void Awake()
    {
        name = gameObject.name;
        transmitters.Add(name, this);
    }

    void FixedUpdate()
    {
        position = transform.position;
        velocity = gameObject.GetComponent<Rigidbody>().velocity;
        angularVelocity = gameObject.GetComponent<Rigidbody>().angularVelocity;
        angle = gameObject.transform.eulerAngles;
        name = gameObject.name;
        bounds = gameObject.GetComponent<Collider>().bounds;
    }
    private Vector3 TranslationUnityToNED(Vector3 force)
    {
        return new Vector3(force.z, force.x, -force.y);
    }

    private Vector3 RotationUnityToNED(Vector3 torque)
    {
        return new Vector3(-torque.z, -torque.x, torque.y);
    }
}
