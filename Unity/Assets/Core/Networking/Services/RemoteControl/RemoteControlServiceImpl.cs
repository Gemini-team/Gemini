using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Grpc.Core;
using System.Threading.Tasks;
using UnityEngine.EventSystems;
using System;

public class RemoteControlServiceImpl : Remotecontrol.RemoteControl.RemoteControlBase
{

    Dictionary<string, IReceiver> receivers = Receiver.receivers;

    public RemoteControlServiceImpl()
    {
    }

    public override async Task<Remotecontrol.ForceResponse> ApplyForce(Remotecontrol.ForceRequest request, ServerCallContext context)
    {

        if (receivers.ContainsKey(request.VesselId))
        {

            // These axes are in Unity left hand coordinate space, should be converted
            // to the standard used in vessel controlling.

            receivers[request.VesselId].SetForce(
                new Vector3(
                    request.GeneralizedForce.X,
                    request.GeneralizedForce.Y,
                    request.GeneralizedForce.Z
                    )
                );

            receivers[request.VesselId].SetTorque(
                new Vector3(
                    request.GeneralizedForce.K,
                    request.GeneralizedForce.M,
                    request.GeneralizedForce.N
                    )
                );

            return await Task.FromResult(new Remotecontrol.ForceResponse
            {
                Success = true 
            });

        } else
        {
            return await Task.FromResult(new Remotecontrol.ForceResponse
            {
                Success = false 
            });
        }

    }


}
