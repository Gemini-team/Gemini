using UnityEngine;

namespace Gemini.Misc
{
    public class CircularMovement : MonoBehaviour
    {

        private Vector3 startPos;
        public float speed = 0.05f;
        public float radius = 2;

        // Start is called before the first frame update
        void Start()
        {
            startPos = transform.position;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = startPos + radius * new Vector3(Mathf.Cos(speed * Time.time), 0, Mathf.Sin(speed * Time.time));
        }
    }
}
