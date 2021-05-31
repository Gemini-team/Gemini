using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class FerryController : MonoBehaviour {
    private const float DOCK_DIST_LIMIT = 2.5f, DOCK_ALIGN_THRESHOLD = 0.9825f;

    [HideInInspector]
    public UnityEvent OnConnectToDock, OnDisconnectFromDock, OnControlChange;
    public float force, rudderStrength = 1, maxSpeed;

    private bool manualControl = true;
    public bool ManualControl {
        get => manualControl;
        set {
            if (value && !manualControl) {
                ui.Alert("Manual takeover required", 3);
            }
            ui.Toggle("ManualIndicator", value);
            manualControl = value;

            OnControlChange?.Invoke();
        }
    }
    
    [HideInInspector]
    public bool boarding;

    private UIManager ui;
    private Rigidbody rb;
    private FerryTrip automatedTrip;
    private Animator[] animators;

    public DockController dock { get; private set; }
    public int DockDirection => dock == null ? 0 : (int)Mathf.Sign(Vector3.Dot(transform.position - dock.transform.Find("DockingArea").position, transform.forward));

    void Start() {
        ui = GameObject.FindGameObjectWithTag("GameController").GetComponent<UIManager>();
        rb = GetComponent<Rigidbody>();
        automatedTrip = GetComponent<FerryTrip>();
        animators = GetComponentsInChildren<Animator>();

        TryConnectToDock();
    }

    void Update() {
        if (!manualControl || boarding || automatedTrip.Playing) return;

        if (Input.GetButtonDown("Dock")) {
            if (dock == null) TryConnectToDock();
            else TryDisconnectFromDock();
        }

        // Prevent ferry movement when docked
        if (dock != null) return;
        
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * force;
        float rudder = Input.GetAxisRaw("Rudder");
            
        rb.AddForce(Quaternion.Euler(0, transform.eulerAngles.y, 0) * new Vector3(input.x, 0, input.y));
        rb.AddTorque(Vector3.up * force * rudder * rudderStrength);

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

    public DockController ClosestDock(out float dist) {
        dist = float.MaxValue;
        DockController closestDock = null;

        foreach (DockController dock in FindObjectsOfType<DockController>()) {
            float dist_ = Vector3.Distance(transform.position, dock.transform.Find("DockingArea").position);
            if (dist_ < dist) {
                closestDock = dock;
                dist = dist_;
            }
        }

        return closestDock;
    }
    public DockController ClosestDock() => ClosestDock(out float _);

    public bool TryConnectToDock() {
        DockController closestDock = ClosestDock(out float dist);

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
