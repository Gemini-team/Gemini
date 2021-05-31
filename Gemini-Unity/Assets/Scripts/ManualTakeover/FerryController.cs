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
            manualControl = value;
            OnControlChange?.Invoke();
        }
    }
    public Vector2 input { get; private set; }
    public float rudder { get; private set; }

    [HideInInspector]
    public bool boarding;

    private Rigidbody rb;
    private FerryTrip automatedTrip;
    private Animator[] animators;

    public DockController AtDock { get; private set; }
    public DockController DestinationDock { get; private set; }
    public Vector3 DockPos(DockController dock) => dock.transform.Find("DockingArea").position;
    public int DockDirection => AtDock == null ? 0 : (int)Mathf.Sign(Vector3.Dot(transform.position - DockPos(AtDock), transform.forward));
    public float RemainingDistance => Vector3.Distance(transform.position, DockPos(DestinationDock));

    void Start() {
        rb = GetComponent<Rigidbody>();
        automatedTrip = GetComponent<FerryTrip>();
        animators = GetComponentsInChildren<Animator>();

        UpdateDestination();
        TryConnectToDock();
    }

    void Update() {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")) * force;
        rudder = Input.GetAxisRaw("Rudder");

        if (!manualControl || boarding || automatedTrip.Playing) return;

        if (Input.GetButtonDown("Dock")) {
            if (AtDock == null) TryConnectToDock();
            else TryDisconnectFromDock();
        }

        // Prevent ferry movement when docked
        if (AtDock != null) return;

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

    private DockController ClosestDock(System.Func<DockController, bool> predicate=null) {
        DockController closestDock = null;
        float dist = float.MaxValue;

        foreach (DockController dock in FindObjectsOfType<DockController>()) {
            if (predicate == null || predicate.Invoke(dock)) {

                float dist_ = Vector3.Distance(transform.position, DockPos(dock));
                if (dist_ < dist) {
                    closestDock = dock;
                    dist = dist_;
                }
            }
        }

        return closestDock;
    }

    private void UpdateDestination() {
        DestinationDock = ClosestDock(dock => !dock.Equals(AtDock));
    }

    public bool TryConnectToDock() {
        DockController dock = ClosestDock();

        if (Vector3.Distance(transform.position, DockPos(dock)) > DOCK_DIST_LIMIT) {
            Debug.Log("Docking failed (too far away)");
            return false;
        }

        float alignment = Mathf.Abs(Vector3.Dot(dock.transform.Find("DockingArea").forward, transform.forward));
        if (alignment < DOCK_ALIGN_THRESHOLD) {
            Debug.Log("Docking failed (not aligned)");
            return false;
        }

        if (!dock.Equals(DestinationDock)) {
            Debug.Log("Docking failed (incorrect dock)");
            return false;
        }

        // Dock to destination, then find next destination
        AtDock = DestinationDock;
        UpdateDestination();

        UpdateAnimators(inTransit: false);
        Debug.Log("Docking successful");

        OnConnectToDock?.Invoke();
        return true;
    }

    public bool TryDisconnectFromDock() {
        if (boarding || AtDock == null) return false;

        AtDock = null;
        UpdateAnimators(inTransit: true);

        OnDisconnectFromDock?.Invoke();
        return true;
    }
}
