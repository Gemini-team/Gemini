using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gemini.Forcecontrol;
using Grpc.Core;
using System.Threading.Tasks;
using Gemini.Core;
using System.Threading;

namespace Gemini.Networking.Services
{
    public class ForceControlServiceImpl : ForceControl.ForceControlBase
    {

        private ForceController[] _forceControllers;

        private Vector3 _force = new Vector3();
        private Vector3 _torque = new Vector3();

        public ForceControlServiceImpl(ForceController[] forceControllers)
        {
            this._forceControllers = forceControllers; 
        }

        public override async Task<ForceResponse> ApplyForce(ForceRequest request, ServerCallContext context)
        {

            _force.x = request.GeneralizedForce.X;
            _force.y = request.GeneralizedForce.Y;
            _force.z = request.GeneralizedForce.Z;

            _torque.x = request.GeneralizedForce.K;
            _torque.y = request.GeneralizedForce.M;
            _torque.z = request.GeneralizedForce.N;

            // Create the event that triggers when the execution of the action is finished.
            ManualResetEvent signalEvent = new ManualResetEvent(false);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                foreach (ForceController forceController in _forceControllers)
                {
                    if (forceController.name == request.VesselId)
                    {
                        forceController.Force = _force;
                        forceController.Torque = _torque;
                    }
                }

                signalEvent.Set();
            });

            // Wait for the event to be triggered from the action, signaling that the action is finished
            // This is required becaue we are reading and depending on state from a resource running on the
            // Unity main thread.
            signalEvent.WaitOne();
            signalEvent.Close();


            return await Task.FromResult(new ForceResponse
            {
                Success = true,
            });

        }
    }
}

