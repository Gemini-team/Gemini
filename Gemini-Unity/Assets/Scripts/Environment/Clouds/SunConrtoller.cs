using UnityEngine;

namespace Gemini.Environment.Clouds
{
    public class SunConrtoller : MonoBehaviour
    {
        public float SunVelocity = 10;

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown("r"))
            {
                transform.Rotate( new Vector3(30,0,0));

            }
            if (Input.GetKey("f"))
            {
                transform.Rotate(new Vector3(-30, 0, 0));
            }
        }
    }
}
