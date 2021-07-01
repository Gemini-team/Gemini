using UnityEngine;

namespace Gemini.Networking.Services {

    [RequireComponent(typeof(Rigidbody))]
    public class ForceController : MonoBehaviour
    {

        private Vector3 _force;
        private Vector3 _torque;

        private Rigidbody _rigidBody;

        public Vector3 Force
        {
            set => _force = value;
            get => _force;
        }

        public Vector3 Torque
        {
            set => _torque = value;
            get => _torque;
        }

        void Start()
        {
            _rigidBody = gameObject.GetComponent<Rigidbody>();
        }
        void FixedUpdate()
        {
            _rigidBody.AddRelativeForce(_force);
            _rigidBody.AddRelativeTorque(_torque);
        }
    }

}
