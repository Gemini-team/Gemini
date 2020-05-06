using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UDP;

public class UDPServer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UDPSocket socket = new UDPSocket();
        socket.Server("127.0.0.1", 50090);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
