using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Grpc.Core;
using System;
using System.IO;

public class RemoteControlServer : MonoBehaviour
{
    //const string HOST = "localhost";
    //const int PORT = 50060;
    public string host = "192.168.1.106";
    public int port = 50060;
    Thread thread;
    Server server;

    // Start is called before the first frame update

    void Start()
    {
        // SECURE
        /*
        var rootCert = File.ReadAllText(Credentials.ClientCertAuthorityPath);
        var keyCertPair = new KeyCertificatePair(
            File.ReadAllText(Credentials.ServerCertChainPath),
            File.ReadAllText(Credentials.ServerPrivateKeyPath));

        SslServerCredentials serverCredentials = new SslServerCredentials(new[] { keyCertPair }, rootCert, true);
        SslCredentials clientCredentials = new SslCredentials(rootCert, keyCertPair);
        */

        server = new Server
        {
    
            // Insecure
            Services = { Remotecontrol.RemoteControl.BindService(new RemoteControlServiceImpl())},
            Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }


            // Secure
            /*
            Services = { Remotecontrol.Controller.BindService(new RemoteControlServiceImpl(rcData))},
            Ports = { { HOST, PORT, serverCredentials } }
            */

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
            Debug.Log("RemoteControlService server listening on port: " + port);

        } catch (InvalidOperationException e)
        {
            Debug.Log(e.StackTrace);
        }
        
    }
}
