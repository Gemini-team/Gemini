using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrusterWidget : MonoBehaviour {
    private const float DIR_LERP_SPEED = 3, THROTTLE_LERP_SPEED = 2;

    private Image bar;
    private FerryController ferry;

    public enum Position { Fore, Aft };
    public Position position = Position.Fore;

    // Start is called before the first frame update
    void Start() {
        ferry = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryController>();
        bar = transform.Find("Fill").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update() {
        float power = Mathf.Clamp01(ferry.input.magnitude + Mathf.Abs(ferry.rudder));
        Vector2 dir = ferry.input + ferry.rudder * (position == Position.Fore ? Vector2.right : Vector2.left);

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, -Vector2.SignedAngle(dir, Vector2.up)), Time.deltaTime * DIR_LERP_SPEED);
        bar.fillAmount = Mathf.Lerp(bar.fillAmount, power, Time.deltaTime * THROTTLE_LERP_SPEED);
    }
}
