using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Grpc.Core;
using System.Threading;
using System;
using GeminiOSPInterface;

namespace Assets.Networking.Services {
    public class SimulationServiceImpl : Simulation.SimulationBase
    {

        private SimulationController _simulationController;
        private GameObject[] _boats;

        public SimulationServiceImpl(SimulationController simulationController, GameObject[] boats)
        {
            _simulationController = simulationController;
            _boats = boats;
        }

        public override async Task<StepResponse> DoStep(
            StepRequest request, ServerCallContext context)
        {
            Debug.Log("Inside DoStep");

            // Create the event that triggers when the execution of the action is finished.
            ManualResetEvent signalEvent = new ManualResetEvent(false);

            List<Pose> poses = new List<Pose>();

            // Converting the poses that to a simple List<Pose> instead of ReapeatedField
            foreach (GeminiOSPInterface.Pose pose in request.VesselPoses)
            {
                poses.Add(new Pose(pose.North, pose.East, pose.Heading));
            }

            float time = request.Time;
            float stepSize = request.StepSize;


            ThreadManager.ExecuteOnMainThread(() =>
            {
                for (int boatIdx = 0; boatIdx < _boats.Length; boatIdx++)
                {
                    _boats[boatIdx].transform.position = new Vector3(poses[boatIdx].East,0,poses[boatIdx].North);
                    float Heading = poses[boatIdx].Heading;
                    Quaternion QuaternionRot = Quaternion.AngleAxis(Heading, new Vector3(0, 1, 0));
                    _boats[boatIdx].transform.rotation = QuaternionRot;
                    //Debug.Log(Position);
                    //boats[boatIdx].transform.position = Position;

                }
                signalEvent.Set();

            });

            // Wait for the event to be triggered from the action, signaling that the action is finished
            // This is required becaue we are reading and depending on state from a resource running on the
            // Unity main thread.
            signalEvent.WaitOne();
            signalEvent.Close();

                
            // Sets the step size in simulation controller
            //_simulationController.SetStepSize(request.StepSize);


            return await Task.FromResult(new StepResponse
            {
                Success = true,

                /*
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
                */
            });
        }

        public override async Task<SetStartTimeResponse> SetStartTime(
            SetStartTimeRequest request, ServerCallContext context)
        {
            Debug.Log("Inside SetStartTime");

            // Create the event that triggers when the execution of the action is finished.
            ManualResetEvent signalEvent = new ManualResetEvent(false);
            var startTime = request.Time;

            ThreadManager.ExecuteOnMainThread(() =>
            {
                GameObject.FindObjectOfType<Camera>();
                signalEvent.Set();

            });

            // Wait for the event to be triggered from the action, signaling that the action is finished
            // This is required becaue we are reading and depending on state from a resource running on the
            // Unity main thread.
            signalEvent.WaitOne();
            signalEvent.Close();


            // Sets the step size in simulation controller


            return await Task.FromResult(new SetStartTimeResponse
            {
                Success = true,
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
