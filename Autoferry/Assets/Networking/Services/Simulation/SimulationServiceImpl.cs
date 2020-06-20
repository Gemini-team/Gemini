using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Simulation;
using System.Threading.Tasks;
using Grpc.Core;
using System.Threading;
using System;

namespace Assets.Networking.Services {
    public class SimulationServiceImpl : Simulation.Simulation.SimulationBase
    {
        private Dictionary<string, GameObject> vesselsDict = new Dictionary<string, GameObject>();

        private Rigidbody _rigidbody;
        private SimulationController _simulationController;

        public SimulationServiceImpl(SimulationController simulationController)
        {
            _simulationController = simulationController;
        }

        public override async Task<Simulation.StepResponse> DoStep(
            Simulation.StepRequest request, ServerCallContext context)
        {

            // Create the event that triggers when the execution of the action is finished.
            ManualResetEvent signalEvent = new ManualResetEvent(false);

            Vector3 force = new Vector3(
                request.Force.X,
                request.Force.Y,
                request.Force.Z
                );

            Vector3 torque = new Vector3(
                request.Force.K,
                request.Force.M,
                request.Force.N
                );

            if (vesselsDict.ContainsKey(request.VesselId))
            {
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    // Perform physics.simulate
                    Debug.Log("Doing stepping!");

                    signalEvent.Set();
                });
            }
            else
            {
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    GameObject vessel = GameObject.Find(request.VesselId);

                    // Do not need to check for exception here since we have
                    // already checked wether the vesselId is already in the dictionary
                    vesselsDict.Add(request.VesselId, vessel);

                    signalEvent.Set();
                });

            }

            // Wait for the event to be triggered from the action, signaling that the action is finished
            signalEvent.WaitOne();
            signalEvent.Close();

            _simulationController.SetForce(ForceNEDToUnity(force));
            _simulationController.SetTorque(TorqueNEDToUnity(torque));

            return await Task.FromResult(new Simulation.StepResponse 
            {

            });
        }

        private Vector3 ForceNEDToUnity(Vector3 force)
        {
            return new Vector3(force.y, -force.z , force.x);
        }

        private Vector3 TorqueNEDToUnity(Vector3 torque)
        {
            return new Vector3(-torque.y, torque.z , -torque.x);
        }

        private Vector3 TranslationUnityToNED(Vector3 force)
        {
            return new Vector3(force.z, force.x, -force.y);
        }

        private Vector3 RotationUnityToNED(Vector3 torque)
        {
            return new Vector3(-torque.z, -torque.x, torque.y);
        }

    }
}
