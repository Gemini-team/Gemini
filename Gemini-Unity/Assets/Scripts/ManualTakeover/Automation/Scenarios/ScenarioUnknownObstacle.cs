using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioUnknownObstacle : Scenario {
	[Space(10)]
	public GameObject obstaclePrefab;
	public Vector3 spawnPoint;
	public float distance;

	private GameObject obstacle;

	protected override void TripStartAction() {
		base.TripStartAction();
		if (tripCount == 0) {
			obstacle = Instantiate(obstaclePrefab, spawnPoint, Quaternion.identity);
		}
	}

	protected override void Step() {
		if (obstacle != null && Vector3.Distance(Ferry.transform.position, obstacle.transform.position) <= distance) {
			ManualTakeover();
		}
	}

	private void OnDrawGizmos() {
		if (Playing && tripCount == 0) {
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(Ferry.transform.position, distance);
		}

		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(spawnPoint, 1);
	}
}
