using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectCollision : MonoBehaviour {
    public CollisionEvent OnCollision = new CollisionEvent();

    private void OnCollisionEnter(Collision collision) {
        OnCollision?.Invoke(collision);
    }
}
