using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Google.Protobuf;
using Grpc.Core;
using GeminiOSPInterface;

namespace Assets.Networking.Services
{
    public class SimulationController : MonoBehaviour
    {
        public string host = "localhost";

        //private int _port = ServicePortGenerator.GenPort();

        private int _port = 12346;

        public int Port
        {
            get => _port;
        }

        public GameObject[] boatPrefabs;


        private Server server;
        private SimulationServiceImpl serviceImpl;

        private Vector3 _force = new Vector3();
        private Vector3 _torque = new Vector3();

        private Rigidbody _rigidBody;
        private GameObject[] _boats;

        private float _timer;
        private float _stepSize;

        // Start is called before the first frame update
        void Start()
        {

            //Physics.autoSimulation = false;

            if (gameObject.GetComponent<Rigidbody>())
            {
                _rigidBody = gameObject.GetComponent<Rigidbody>();
            }

            _boats = new GameObject[boatPrefabs.Length];
            for (int prefabIdx = 0; prefabIdx < boatPrefabs.Length; prefabIdx++)
            {
                _boats[prefabIdx] = Instantiate(boatPrefabs[prefabIdx], new Vector3(0, 0, 0), Quaternion.identity);
            }

            serviceImpl = new SimulationServiceImpl(this, _boats);

            server = new Server
            {
                Services = { Simulation.BindService(serviceImpl) },
                Ports = { new ServerPort(host, _port, ServerCredentials.Insecure) }
            };

            Debug.Log("Simulation server listening on port: " + _port);
            server.Start();
        }

        void Update()
        {

        }

        void FixedUpdate()
        {

        }

        public void SetForce(Vector3 force)
        {
            _force = force;
        }

        public void SetTorque(Vector3 torque)
        {
            _torque = torque;
        }


        public void SetStepSize(float stepSize)
        {
            _stepSize = stepSize;
        }

    }
}
