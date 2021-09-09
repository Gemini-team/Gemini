using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetupScene
{
    public void SpawnSphere(float radius, Vector3 transform)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // TODO: Scale by radius
        sphere.transform = transform;
    }
}
