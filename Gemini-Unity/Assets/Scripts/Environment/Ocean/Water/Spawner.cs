using UnityEngine;

namespace Gemini.Environment.Ocean
{
    public class Spawner : MonoBehaviour
    {
        public GameObject WaterPrefab;
        public int size;
        // Start is called before the first frame update
        void Start()
        {
            Vector3 parentPosition = transform.position;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Instantiate(WaterPrefab, parentPosition + new Vector3(5 * i, 0, 5 * j), Quaternion.identity);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
