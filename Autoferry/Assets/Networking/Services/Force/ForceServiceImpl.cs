using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Force;
using System.Threading.Tasks;
using Grpc.Core;
using System.Threading;

namespace Assets.Networking.Services { 
    public class ForceServiceImpl : Force.Force.ForceBase
    {
        private ForceResponse _forceResponse;
        private Rigidbody _rigidbody;
        private ForceController _forceController; 
        public ForceServiceImpl()
        {
        }

        public override async Task<Force.ForceResponse> ApplyForce(
            Force.ForceRequest request, ServerCallContext context)
        {
            
            // Create the event that triggers when the execution of the action is finished.
            ManualResetEvent signalEvent = new ManualResetEvent(false);

            Vector3 force = new Vector3(
                request.GeneralizedForce.X,
                request.GeneralizedForce.Y,
                request.GeneralizedForce.Z
                );

            Vector3 torque = new Vector3(
                request.GeneralizedForce.K,
                request.GeneralizedForce.M,
                request.GeneralizedForce.N
                );

            ThreadManager.ExecuteOnMainThread(() =>
            {
                // TODO: This is a bit slow, should have a dictionary of
                // vesselID -> gameObject and use that instead.
                GameObject vessel = GameObject.Find(request.VesselId);
                if (vessel)
                {
                    // TODO: should probably add check for wheter the vessel
                    // has the components or not.
                    _forceController = vessel.GetComponent<ForceController>();
                    _rigidbody = vessel.GetComponent<Rigidbody>();
                }

            });

            _forceController.SetForce(ForceNEDToUnity(force));
            _forceController.SetTorque(TorqueNEDToUnity(torque));   

            // Wait for the event to be triggered from the action, signaling that the action is finished
            signalEvent.WaitOne();
            signalEvent.Close();

            return await Task.FromResult(new Force.ForceResponse
            {
                Position = new Position { N = 0.0f, E = 0.0f, D = 0.0f},
                Orientation = new Orientation { Phi = 0.0f, Theta = 0.0f, Psi = 0.0f},
                Velocity = new Velocity { U = 0.0f, V = 0.0f, W = 0.0f },
                AngularVelocity = new AngularVelocity { P = 0.0f, Q = 0.0f, R = 0.0f },
                AngularAcceleration = new AngularAcceleration { PDot = 0.0f, QDot = 0.0f, RDot = 0.0f },
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
