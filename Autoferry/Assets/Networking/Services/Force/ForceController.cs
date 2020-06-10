using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Grpc.Core;


namespace Assets.Networking.Services
{
    public class ForceController: MonoBehaviour
    {

        public string host = "localhost";

        private int _port = ServicePortGenerator.GenPort();

        public int Port
        {
            get => _port;
        }

        private Server server;
        private ForceServiceImpl serviceImpl;

        private Vector3 _force = new Vector3();
        private Vector3 _torque = new Vector3();

        private Rigidbody _rigidBody;

        // Start is called before the first frame update
        void Start()
        {
            if (gameObject.GetComponent<Rigidbody>())
            {
                _rigidBody = gameObject.GetComponent<Rigidbody>();
            }

            serviceImpl = new ForceServiceImpl();

            server = new Server
            {
                Services = { Force.Force.BindService(serviceImpl) },
                Ports = { new ServerPort(host, _port, ServerCredentials.Insecure) }
            };

            Debug.Log("Force server listening on port: " + _port);
            server.Start();
        }

        void FixedUpdate()
        {
            _rigidBody.AddRelativeForce(_force);
            _rigidBody.AddRelativeTorque(_torque);

        }

        public void SetForce(Vector3 force)
        {
            _force = force; 
        }

        public void SetTorque(Vector3 torque)
        {
            _torque = torque;
        }

    }
}
