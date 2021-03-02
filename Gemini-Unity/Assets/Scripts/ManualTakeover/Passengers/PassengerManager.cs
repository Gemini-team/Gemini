using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassengerManager : MonoBehaviour {
    [HideInInspector]
    public PassengerQueue queue;
    [HideInInspector]
    public PassengerController controller;

    void Start()
    {
        queue = GetComponentInChildren<PassengerQueue>();
        controller = GetComponentInChildren<PassengerController>();
    }
}
