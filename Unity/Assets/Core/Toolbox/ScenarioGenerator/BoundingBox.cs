using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundingBox : MonoBehaviour
{
    // Visual display of the bounding box for testing.
    [SerializeField] private float height = 0;
    [SerializeField] private float width = 0;
    [SerializeField] private float length = 0;
    [SerializeField] private float Sealine = 0;


    private void OnDrawGizmos()
    {
        // Draw each child's bounds as a green box.
        
        Gizmos.color = new Color(0f, 1f, 0f, 0.3f);
        
        foreach (var child in GetComponentsInChildren<Collider>())
        {
            Gizmos.DrawCube(child.bounds.center, child.bounds.size);
        }
        

        // Draw total bounds of all the children as a white box.
        Gizmos.color = new Color(1f, 1f, 1f, 0.1f);
        var maxBounds = GetMaxBounds(gameObject);
        Gizmos.DrawCube(maxBounds.center, maxBounds.size);
        height = maxBounds.size.y;
        width = maxBounds.size.z;
        length = maxBounds.size.x;
        Vector3 SealinePosition = maxBounds.center - new Vector3(0, height / 2, 0);
        Sealine = SealinePosition.y;
        Gizmos.color = new Color(1f, 0, 0, 0.5f);
        Gizmos.DrawSphere(SealinePosition,0.1f);
        Debug.Log("Total height is " + maxBounds.size.y);
    }


    Bounds GetMaxBounds(GameObject parent)
    {
        var total = new Bounds(parent.transform.position, Vector3.zero);
        foreach (var child in parent.GetComponentsInChildren<Collider>())
        {
            total.Encapsulate(child.bounds);
        }
        return total;
    }
}
