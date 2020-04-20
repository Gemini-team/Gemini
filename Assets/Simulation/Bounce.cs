using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    Rigidbody rb;
    public float timeOffset = 1f;
    private float timer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown("space"))
        if (timer >= Time.time)
        {
            rb.AddForce(new Vector3(Random.Range(-2f,2f), Random.Range(-2f,2f), Random.Range(-2f, 2f)),ForceMode.Impulse);
            timer = Time.time + timeOffset;
        }
    }
}
