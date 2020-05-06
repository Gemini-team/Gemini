using Grpc.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Sensormanagement;

namespace Assets.Networking.Services
{
    public class SensorManager : MonoBehaviour
    {

        public static SensorManager instance;

        public string host = "localhost";

        private int port = ServicePortGenerator.GenPort();
        private Server server;
        public static SensorManagementServiceImpl serviceImpl = new SensorManagementServiceImpl();


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exists, destroying object!");
                Destroy(this);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            server = new Server
            {
                Services = { SensorManagement.BindService(serviceImpl) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            Debug.Log("Sensormanager server listening on port: " + port);
            server.Start();

        }

        /// <summary>
        /// Stops specified sensor from rendering. Returns whether it
        /// successfully set the sensor's RenderFlag or not.
        /// </summary>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        public static bool StopSensorRendering(int sensorID)
        {
            var sensors = FindObjectsOfType<Sensor>();

            foreach (Sensor sensor in sensors)
            {
                if (sensor.ID == sensorID)
                {
                    sensor.RenderFlag = false;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Starts specified sensor from rendering. Returns whether it
        /// successfully set the sensor's RenderFlag or not.
        /// </summary>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        public static bool StartSensorRendering(int sensorID)
        {
            var sensors = FindObjectsOfType<Sensor>();

            foreach (Sensor sensor in sensors)
            {
                if (sensor.ID == sensorID)
                {
                    sensor.RenderFlag = true;
                    return true;
                }
            }

            return false;

        }

        /// <summary>
        /// Finds all sensors in the scene.
        /// </summary>
        /// <returns></returns>
        public static Sensor[] FindAllSensors()
        {
            var sensors = FindObjectsOfType<Sensor>();
            return sensors; 
        }

        /// <summary>
        /// Returns all sensors that are located on a vessel
        /// </summary>
        /// <param name="vesselID"></param>
        /// <returns></returns>
        public static Sensor[] FindAllSensorsOnVessel(string vesselID)
        {

            // TODO: The representation of how a sensor is connected to a vessel must
            // be decided. Should the vessel contain a Gameobject with a sensor, and or should
            // the sensor have a reference to the vessel it is connected to?

            return new Sensor[0];
        }

        /// <summary>
        /// Returns array of all sensors of a specified sensortype.
        /// </summary>
        /// <param name="sensorType"></param>
        /// <returns></returns>
        public static Sensor[] FindAllSensorsOfType(SensorType sensorType)
        {
            var allSensors = FindAllSensors();
            List<Sensor> typeSensors = new List<Sensor>();

            foreach (Sensor sensor in allSensors)
            {
                if (sensor.GetType() == sensorType)
                {
                    typeSensors.Add(sensor);
                }
            }

            return typeSensors.ToArray();
        }

    }

}
