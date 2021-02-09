using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDP;

public class UDPClient : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UDPSocket client = new UDPSocket();
        client.Client("127.0.0.1", 50090);
        client.Send("TEST!");


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
