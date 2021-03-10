using UnityEngine;
using Grpc.Core;
using Gemini.Forcecontrol;

namespace Gemini.Networking.Services
{
    public class ForceControlManager : MonoBehaviour
    {
        public string host = "192.168.0.93";
        private int _port = 12347;

        public int Port
        {
            get => _port;
        }

        private Server server;
        private ForceControlServiceImpl serviceImpl;
        ForceController[] forceControllers;

        // Start is called before the first frame update
        void Start()
        {
            forceControllers = FindObjectsOfType<ForceController>();

            serviceImpl = new ForceControlServiceImpl(forceControllers);
            
            server = new Server
            {
                Services = { ForceControl.BindService(serviceImpl) },
                Ports = { new ServerPort(host, _port, ServerCredentials.Insecure) }
            };

            Debug.Log("Simulation server listening on port: " + _port);
            server.Start();

        }
    }
}
