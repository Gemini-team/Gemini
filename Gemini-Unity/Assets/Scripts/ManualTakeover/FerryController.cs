using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FerryController : MonoBehaviour {
    private const float DOCK_DIST_LIMIT = 2.5f, DOCK_ALIGN_THRESHOLD = 0.9825f;

    public float force, maxSpeed, throttleSensitivity = 1;

    [HideInInspector]
    public bool boarding;

    private UIManager ui;
    private Rigidbody rb;
    private FerryTrip automatedTrip;
    private Animator[] animators;
    private float throttle;

    public DockController dock { get; private set; }

    void Start() {
        ui = FindObjectOfType<UIManager>();
        rb = GetComponent<Rigidbody>();
        automatedTrip = GetComponent<FerryTrip>();
        animators = GetComponentsInChildren<Animator>();
    }

    void Update() {
        ui.SetBar("Throttle/Up", Mathf.Max(throttle, 0));
        ui.SetBar("Throttle/Down", Mathf.Max(-throttle, 0));

        // Ignore player input while automated sequence is playing
        if (automatedTrip.Playing) return;

        if (Input.GetKeyDown(KeyCode.F)) {
            if (dock == null) TryConnectToDock();
            else TryDisconnectFromDock();
        }

        // Prevent ferry movement when docked
        if (dock != null) return;
        
        float dir = Input.GetAxisRaw("Vertical");
        float rudder = Input.GetAxis("Horizontal");

        throttle = Mathf.Clamp(throttle + dir * throttleSensitivity * Time.deltaTime, -1, 1);
        if (Input.GetKeyDown(KeyCode.X)) throttle = 0;
            

        rb.AddForce(transform.forward * throttle * force);
        rb.AddTorque(Vector3.up * rudder * force);

        float mag = rb.velocity.magnitude;
        if (mag > maxSpeed) {
            rb.velocity *= maxSpeed / mag;
        }
    }

    private void UpdateAnimators(bool inTransit) {
        foreach (Animator anim in animators) {
            anim.SetBool("inTransit", inTransit);
            anim.SetBool("reverse", !inTransit && Vector3.Dot(dock.transform.Find("DockingArea").position - transform.position, transform.forward) > 0);
        }
    }

    public bool TryConnectToDock() {
        float dist = float.MaxValue;
        DockController closestDock = null;

        foreach (DockController dock in FindObjectsOfType<DockController>()) {
            float dist_ = Vector3.Distance(transform.position, dock.transform.Find("DockingArea").position);
            if (dist_ < dist) {
                closestDock = dock;
                dist = dist_;
            }
        }

        if (dist > DOCK_DIST_LIMIT) {
            Debug.Log("Docking failed (too far away)");
            return false;
        }

        float alignment = Mathf.Abs(Vector3.Dot(closestDock.transform.Find("DockingArea").forward, transform.forward));
        if (alignment < DOCK_ALIGN_THRESHOLD) {
            Debug.Log("Docking failed (not aligned)");
            return false;
        }

        dock = closestDock;
        UpdateAnimators(inTransit: false);
        Debug.Log("Docking successful");
        return true;
    }

    public bool TryDisconnectFromDock() {
        if (boarding || dock == null) return false;

        dock = null;
        UpdateAnimators(inTransit: true);
        return true;
    }
}
