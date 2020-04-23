using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cameradata;
using Grpc.Core;
using System;

public class CameradataServer : MonoBehaviour
{
    //const int PORT = 50070;
    private Thread cameradataServerThread;
    private Server server;

    // Multiple cameras
    // TODO - This will be dynamically set later.
    const int numCameras = 1;

    CameradataServiceImpl[] cameradataServiceImpls = new CameradataServiceImpl[numCameras];
    int[] ports = new int[numCameras];

    Server[] servers = new Server[numCameras];
    Thread[] threads = new Thread[numCameras];


    public void Start()
    {
        for (int i = 0; i < numCameras; i++)
        {
            cameradataServiceImpls[i] = new CameradataServiceImpl();

            ports[i] = CameraServiceGen.GenPort();

            servers[i] = new Server
            {
                Services = { Cameradata.CameradataService.BindService(cameradataServiceImpls[i]) },
                Ports = { new ServerPort("localhost", ports[i], SslServerCredentials.Insecure) }
            };

            threads[i] = new Thread(ListenForRequests);
            threads[i].IsBackground = true;
            threads[i].Start(servers[i]);

        }
    }

    void ListenForRequests(object serv)
    {
        try
        {
            Server server = (Server) serv;
            server.Start();
            Debug.Log("Press ESC to kill the server");
            string input = Console.ReadLine();
        } catch (InvalidOperationException e)
        {
            Debug.Log(e.StackTrace);
        }
    }

    public void SetCameradata(int id, CameradataImpl cameradata)
    {
        cameradataServiceImpls[id].SetCameradata(cameradata);
    }

}
