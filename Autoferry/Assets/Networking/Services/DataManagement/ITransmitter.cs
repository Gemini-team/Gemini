using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransmitter
{

    string GetName();

    Vector3 GetPosition();

    Vector3 GetVelocity();

    Vector3 GetAngularVelocity();

    Vector3 GetAngle();

    Bounds GetBounds();
}
