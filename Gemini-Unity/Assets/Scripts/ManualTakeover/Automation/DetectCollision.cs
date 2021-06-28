using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DetectCollision : MonoBehaviour {
    public CollisionEvent OnCollisionMessage = new CollisionEvent();
    public LayerMask layermask;
    public bool deactivateOnCollision;

    private void OnCollisionEnter(Collision collision) {
        if (enabled && layermask == (layermask | (1 << collision.gameObject.layer))) {
            OnCollisionMessage?.Invoke(collision);
            if (deactivateOnCollision) enabled = false;
        }
    }
}
