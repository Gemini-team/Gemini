using Grpc.Core;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Sensormanager;

public class SensorManagerServiceImpl : SensorManager.SensorManagerBase
{
    List<Sensor> sensors;

    public SensorManagerServiceImpl()
    {

    }

    public override async Task<AllSensorsOnVesselResponse> GetAllSensorsOnVessel(
        AllSensorsOnVesselRequest request, ServerCallContext context)
    {
        return await Task.FromResult(new AllSensorsOnVesselResponse
        {
            Sensors = { sensors }
        });
    }

}
