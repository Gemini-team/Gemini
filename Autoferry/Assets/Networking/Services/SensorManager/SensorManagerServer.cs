using Grpc.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class SensorManagerServer : MonoBehaviour
{
    public string host = "localhost";
    public int port = 50050;
    Thread thread;
    Server server;

    // Start is called before the first frame update
    void Start()
    {
        server = new Server
        {
            Services = { Sensormanager.SensorManager.BindService(new SensorManagerServiceImpl()) },
            Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
        };

        thread = new Thread(new ThreadStart(ListenForRequests));
        thread.IsBackground = true;
        thread.Start();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ListenForRequests()
    {
        try
        {
            server.Start();
            Debug.Log("SensorManager server listening on port: " + port);

        } catch (InvalidOperationException e)
        {
            Debug.Log(e.StackTrace);
        }
        
    }

}
