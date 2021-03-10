using UnityEngine;

namespace Gemini.Networking.Services {

    [RequireComponent(typeof(Rigidbody))]
    public class ForceController : MonoBehaviour
    {

        private Vector3 _force;
        private Vector3 _torque;

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

        void FixedUpdate()
        {
            gameObject.GetComponent<Rigidbody>().AddForce(_force);
            gameObject.GetComponent<Rigidbody>().AddTorque(_torque);
        }
    }

}
