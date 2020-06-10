using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour, IReceiver
{
    private Vector3 force = new Vector3();
    private Vector3 torque = new Vector3();

    private List<Vector3> forces = new List<Vector3>();
    private List<Vector3> forcePoints = new List<Vector3>();

    private string name;
    private Rigidbody rigidBody = new Rigidbody();

    public static Dictionary<string, IReceiver> receivers = new Dictionary<string, IReceiver>();


    public void Awake()
    {
        name = gameObject.name;
        receivers.Add(name, this);
        rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    public void SetForce(Vector3 force)
    {
        //this.force = force;
        this.force = ForceNEDToUnity(force);
    }

    public void SetForces(List<Vector3> forces, List<Vector3> forcePoints)
    {
        this.forces = forces;
        this.forcePoints = forcePoints;
    }

    public void FixedUpdate()
    {
        // Adds force in regards to the world coordinate system
        //rigidBody.AddForce(force);
        //rigidBody.AddTorque(torque);

        // Adds force in regards to the relative to its coordinate system
        rigidBody.AddRelativeForce(force);
        rigidBody.AddRelativeTorque(torque);
    }

    
    public void SetTorque(Vector3 torque)
    {
        this.torque = TorqueNEDToUnity(torque);
    }

    private Vector3 ForceNEDToUnity(Vector3 force)
    {
        return new Vector3(force.y, -force.z , force.x);
    }

    private Vector3 TorqueNEDToUnity(Vector3 torque)
    {
        return new Vector3(-torque.y, torque.z , -torque.x);
    }

}

