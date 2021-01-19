using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Simulation;
using System.Threading.Tasks;
using Grpc.Core;
using System.Threading;
using System;

namespace Assets.Networking.Services {
    /*
    public class SimulationServiceImplOld : Simulation.Simulation.SimulationBase
    {
        private Dictionary<string, GameObject> vesselsDict = new Dictionary<string, GameObject>();

        private SimulationController _simulationController;


        public SimulationServiceImpl(SimulationController simulationController)
        {
            _simulationController = simulationController;
        }

        public override async Task<StepResponse> DoStep(
            StepRequest request, ServerCallContext context)
        {

            // Create the event that triggers when the execution of the action is finished.
            ManualResetEvent signalEvent = new ManualResetEvent(false);

            // Need to convert the force and torque from a Simulation.Force to a Vector3

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

            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 angle = new Vector3(0.0f, 0.0f, 0.0f);

            Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 angularVelocity = new Vector3(0.0f, 0.0f, 0.0f);

            if (vesselsDict.ContainsKey(request.VesselId))
            {

                GameObject vessel = vesselsDict[request.VesselId];

                ThreadManager.ExecuteOnMainThread(() =>
                {

                    // Reads state from the vessel gameobject
                    position = vessel.transform.position;
                    angle = vessel.transform.eulerAngles;

                    velocity = vessel.GetComponent<Rigidbody>().velocity;
                    angularVelocity = vessel.GetComponent<Rigidbody>().angularVelocity;

                    // Setting the signal is required here because it signals that the
                    // signalEvent does not need to block anymore since the action has
                    // executed on the main thread.
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

                    position = vessel.transform.position;
                    angle = vessel.transform.eulerAngles;

                    velocity = vessel.GetComponent<Rigidbody>().velocity;
                    angularVelocity = vessel.GetComponent<Rigidbody>().angularVelocity;
                    
                    // Setting the signal is required here because it signals that the
                    // signalEvent does not need to block anymore since the action has
                    // executed on the main thread.
                    signalEvent.Set();
                });

            }

            // Wait for the event to be triggered from the action, signaling that the action is finished
            // This is required becaue we are reading and depending on state from a resource running on the
            // Unity main thread.
            signalEvent.WaitOne();
            signalEvent.Close();

                
            // Sets the step size in simulation controller
            _simulationController.SetStepSize(request.StepSize);

            // Setting force and torque, gets updated every FixedUpdate by the SimulationController
            _simulationController.SetForce(ForceNEDToUnity(force));
            _simulationController.SetTorque(TorqueNEDToUnity(torque));

            // TODO: Verify that this is the correct way of doing this.
            Vector3 convertedVelocity = TranslationUnityToNED(velocity);
            Vector3 convertedAngularVelocity = RotationUnityToNED(angularVelocity);

            
            return await Task.FromResult(new StepResponse
            {

                // TODO: Verify that these are the correct values.
                // A bit unsure whether this fits
                Pos = new Position { 
                    North = position.x,
                    East = position.y,
                    Down = position.z,
                    Roll = angle.x,
                    Pitch = angle.y,
                    Yaw = angle.z
                }, 

                Vel = new Velocity
                {
                    Surge = convertedVelocity.x,
                    Sway = convertedVelocity.y,
                    Heave = convertedVelocity.z,
                    Roll = convertedAngularVelocity.x,
                    Pitch = convertedAngularVelocity.y,
                    Yaw = convertedAngularVelocity.z
                }
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
    */
}
