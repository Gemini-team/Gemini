using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class FerryController : MonoBehaviour {
    private const float DOCK_DIST_LIMIT = 2.5f, DOCK_ALIGN_THRESHOLD = 0.9825f;

    [HideInInspector]
    public UnityEvent OnConnectToDock, OnDisconnectFromDock;
    public float force, maxSpeed, throttleSensitivity = 1;

    [HideInInspector]
    public bool manualControl = true, boarding;

    private UIManager ui;
    private Rigidbody rb;
    private FerryTrip automatedTrip;
    private Animator[] animators;
    private float throttle;

    public DockController dock { get; private set; }
    public int DockDirection => dock == null ? 0 : (int)Mathf.Sign(Vector3.Dot(transform.position - dock.transform.Find("DockingArea").position, transform.forward));

    void Start() {
        ui = FindObjectOfType<UIManager>();
        rb = GetComponent<Rigidbody>();
        automatedTrip = GetComponent<FerryTrip>();
        animators = GetComponentsInChildren<Animator>();
    }

    void Update() {
        ui.SetBar("Throttle/Up", Mathf.Max(throttle, 0));
        ui.SetBar("Throttle/Down", Mathf.Max(-throttle, 0));

        if (!manualControl || boarding || automatedTrip.Playing) return;

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
            anim.SetBool("reverse", DockDirection == -1);
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

        OnConnectToDock?.Invoke();
        return true;
    }

    public bool TryDisconnectFromDock() {
        if (boarding || dock == null) return false;

        dock = null;
        UpdateAnimators(inTransit: true);

        OnDisconnectFromDock?.Invoke();
        return true;
    }
}
