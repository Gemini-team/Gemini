using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using UnityEngine.Events;

[RequireComponent(typeof(BoatController))]
public abstract class BoatAutopilot : FollowPath {
	private const float LOOK_AHEAD_TIME = 0.1f, CORRECTION_DAMPENING = 10f, RUDDER_CUTOFF = 0.1f;
	
	public bool omniDirectional;

	protected BoatController controller { get; private set; }

	private Vector3 target;

    protected override void Start() {
		base.Start();

		controller = GetComponent<BoatController>();
	}

    protected override void Move() {
		float closestTime = route.path.GetClosestTimeOnPath(transform.position);
		target = route.path.GetPointAtTime(closestTime + LOOK_AHEAD_TIME * (Reversed ? -1 : 1), EndOfPathInstruction.Stop);

		Quaternion idealHeading = route.path.GetRotation(closestTime, EndOfPathInstruction.Stop);
		Vector2 input = Vector2.up;

		if (omniDirectional) {
			Vector3 direction = Quaternion.Euler(0, -transform.eulerAngles.y, 0) * (target - transform.position);
			input = new Vector2(direction.x, direction.z).normalized;
		}

		controller.input = Throttle * input;

		float correctionAngle = idealHeading.eulerAngles.y - transform.eulerAngles.y;
		float rudder = Mathf.Clamp(correctionAngle / CORRECTION_DAMPENING, -1f, 1f);
		if (Mathf.Abs(rudder) < RUDDER_CUTOFF) rudder = 0;

		controller.rudder = rudder;
	}

    private void OnDrawGizmos() {
		if (Playing) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, target);
			Gizmos.DrawWireSphere(target, 0.75f);

			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(transform.position, transform.position + (transform.right * controller.input.x + transform.forward * controller.input.y) * 20);
		}
	}
}
