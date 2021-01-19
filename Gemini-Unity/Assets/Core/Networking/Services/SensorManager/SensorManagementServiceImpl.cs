using Grpc.Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Assets.Networking.Services;
using System.Threading;
using System;

public class SensorManagementServiceImpl : Sensormanagement.SensorManagement.SensorManagementBase
{

    public SensorManagementServiceImpl()
    {
    }

    public override async Task<Sensormanagement.StartRenderingResponse> StartRendering(
        Sensormanagement.StartRenderingRequest request, ServerCallContext context)
    {

        int sensorID = request.SensorID;

        // Create the event that triggers when the execution of the action is finished.
        ManualResetEvent signalEvent = new ManualResetEvent(false);

        bool success = false;

        ThreadManager.ExecuteOnMainThread(() =>
        {
            success = SensorManager.StartSensorRendering(sensorID);
        });

        // Wait for the event to be triggered from the action, signaling that the action is finished
        signalEvent.WaitOne();
        signalEvent.Close();

        return await Task.FromResult(new Sensormanagement.StartRenderingResponse
        {
            Success = success
        });

    }

    public override async Task<Sensormanagement.StopRenderingResponse> StopRendering(
        Sensormanagement.StopRenderingRequest request, ServerCallContext context)
    {

        int sensorID = request.SensorID;

        // Create the event that triggers when the execution of the action is finished.
        ManualResetEvent signalEvent = new ManualResetEvent(false);

        bool success = false;

        ThreadManager.ExecuteOnMainThread(() =>
        {
            success = SensorManager.StopSensorRendering(sensorID);
        });

        // Wait for the event to be triggered from the action, signaling that the action is finished
        signalEvent.WaitOne();
        signalEvent.Close();

        return await Task.FromResult(new Sensormanagement.StopRenderingResponse
        {
            Success = success
        });

    }

    public override async Task<Sensormanagement.AllSensorsOnVesselResponse> GetAllSensorsOnVessel(
        Sensormanagement.AllSensorsOnVesselRequest request, ServerCallContext context)
    {
        Debug.Log("GetAllSensorsOnVessel vesselID: " + request.VesselID);

        return await Task.FromResult(new Sensormanagement.AllSensorsOnVesselResponse
        {
            
            Sensors = { new Sensormanagement.Sensor[0] }
        });
    }
    
    public override async Task<Sensormanagement.AllSensorsOfTypeResponse> GetAllSensorsOfType(
        Sensormanagement.AllSensorsOfTypeRequest request, ServerCallContext context)
    {
        List<Sensor> sensors = new List<Sensor>();
        List<Sensormanagement.Sensor> protobufSensors = new List<Sensormanagement.Sensor>();

        // Create the event that triggers when the execution of the action is finished.
        ManualResetEvent signalEvent = new ManualResetEvent(false);

        // The action that will we executed on the Unity MainThread
        Action action = () =>
        {
            sensors.AddRange(SensorManager.FindAllSensorsOfType((SensorType)request.Type));

            foreach (Sensor sensor in sensors)
            {
                protobufSensors.Add(ConvertSensorToSensormanagementSensor(sensor));
            }

            signalEvent.Set();
        };

        // Call the exection of the action
        ThreadManager.ExecuteOnMainThread(action);

        // Wait for the event to be triggered from the action, signaling that the action is finished
        signalEvent.WaitOne();
        signalEvent.Close();

        return await Task.FromResult(new Sensormanagement.AllSensorsOfTypeResponse
        {
            Sensors = { protobufSensors.ToArray() }
        });
    }

    /// <summary>
    /// Converts object of the Script Sensor type into a Sensormanagement.Sensor type used by Protobuf.
    /// </summary>
    /// <param name="sensor"></param>
    /// <returns></returns>
    private Sensormanagement.Sensor ConvertSensorToSensormanagementSensor(Sensor sensor)
    {
        Sensormanagement.Sensor managementSensor = new Sensormanagement.Sensor();

        // TODO: sensor.type should be accessed from a get
        SensorType sensorType = sensor.GetType();

        managementSensor.Id = sensor.ID;

        if (sensorType == SensorType.Infrared)
        {
            managementSensor.Type = Sensormanagement.SensorType.Infrared;
        }   
        else if (sensorType == SensorType.Lidar)
        {
            managementSensor.Type = Sensormanagement.SensorType.Lidar;
        } 
        else if (sensorType == SensorType.Optical)
        {
            managementSensor.Type = Sensormanagement.SensorType.Optical;
        } 
        else if (sensorType == SensorType.Radar)
        {
            managementSensor.Type = Sensormanagement.SensorType.Radar;
        }
        else if (sensorType == SensorType.Unknown)
        {

            managementSensor.Type = Sensormanagement.SensorType.Unknown;
        }

        managementSensor.SensorWidth = sensor.RenderTexture.width;
        managementSensor.SensorHeight = sensor.RenderTexture.height;
        managementSensor.IpAddress = sensor.host;
        managementSensor.Port = sensor.Port;

        return managementSensor;
    }

}
