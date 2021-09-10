using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SetupScene
{
    public static void SpawnSphere(float radius, Vector3 position)
    {
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = position;
        sphere.transform.localScale = new Vector3(radius, radius, radius);
    }
}
