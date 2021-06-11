using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Crest;

public class ScenarioUnknownObstacle : Scenario {
	public override string ScenarioName => "UnknownObstacle";

	public override string FailureWarning => "Collision detected";

    [Space(10)]
	public GameObject obstacle;

	private BoatProbes floatingObject;

    private void Start() {
		if (obstacle == null) throw new System.ArgumentNullException("Obstacle is not set");

		obstacle.GetComponent<DetectCollision>().OnCollision.AddListener(_ => ManualTakeover());

		floatingObject = obstacle.GetComponent<BoatProbes>();
		floatingObject._playerControlled = false;
		obstacle.SetActive(false);
    }

    protected override void TripStartAction() {
		base.TripStartAction();

		if (tripCount == 0) {
			Debug.Log("Moving floating obstacle");
			obstacle.SetActive(true);
			floatingObject._engineBias = 1;
		}
	}

    protected override void ManualTakeover() {
        base.ManualTakeover();

		floatingObject._engineBias = 0.5f;
    }

    private void OnDrawGizmos() {
		if (obstacle != null) {
			Gizmos.color = Color.magenta;
			Gizmos.DrawWireSphere(obstacle.transform.position, 2);
			Gizmos.DrawLine(obstacle.transform.position, obstacle.transform.position + obstacle.transform.rotation * Vector3.forward * 500);
		}
	}
}
