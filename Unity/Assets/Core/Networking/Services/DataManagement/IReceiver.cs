using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReceiver 
{
    void SetForce(Vector3 force);

    void SetTorque(Vector3 torque);
    void SetForces(List<Vector3> forces, List<Vector3> forcePoints);
}
