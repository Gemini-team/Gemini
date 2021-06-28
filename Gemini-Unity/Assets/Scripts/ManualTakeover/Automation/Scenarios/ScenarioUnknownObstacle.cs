using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crest;

public class ScenarioUnknownObstacle : Scenario {
	public override string ScenarioName => "UnknownObstacle";

	public override string FailureWarning => "Collision detected";

	private string FailureImminentWarning => "Possible object detected of unknown classification\nStand by for possible takeover";

	public float warningDistance = 20;

    [Space(10)]
	public GameObject obstacle;

	private Transform ferry;
	private bool warningSent;

    private void Start() {
		if (obstacle == null) throw new System.ArgumentNullException("Obstacle is not set");

		ferry = GameObject.FindGameObjectWithTag("Player").transform;
		BoatProbes floatingObject = obstacle.GetComponent<BoatProbes>();
		DetectCollision detectCollision = obstacle.GetComponent<DetectCollision>();

		detectCollision.OnCollisionMessage.AddListener(_ => TriggerManualTakeoverEvent());
		floatingObject._playerControlled = false;

		detectCollision.enabled = false;
		obstacle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
    }

    private void Update() {
        if (Playing && !warningSent && Vector3.Distance(ferry.position, obstacle.transform.position) <= warningDistance) {
			warningSent = true;
			OnManualTakeoverImminent?.Invoke(0, FailureImminentWarning);
		}
    }

    protected override void TripStartAction() {
		base.TripStartAction();

		if (tripCount == 0) {
			Debug.Log("Moving floating obstacle");
			obstacle.GetComponent<DetectCollision>().enabled = true;
			obstacle.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			obstacle.GetComponent<AnimatedAutopilot>().Play();
		}
	}

    protected override void TriggerManualTakeoverEvent() {
        base.TriggerManualTakeoverEvent();
		obstacle.GetComponent<AnimatedAutopilot>().Stop();
	}
}
