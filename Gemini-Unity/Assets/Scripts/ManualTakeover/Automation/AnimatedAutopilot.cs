using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedAutopilot : FollowPath {
	public enum AnimateOn { Call, Random, FerryAB, FerryBA, FerryPlay, FerryDocked };

	[Space(10)]
	public AnimateOn animateOn;
	[HideInInspector]
	public float minWaitTime = 5, maxWaitTime = 20;
	public float maxSpeed = 5;
	[Range(0f, 1f)]
	public float playChance = 1;

	private FerryAutopilot ferryTrip;
	private float distanceTravelled;

	protected override void Start() {
		base.Start();

		ferryTrip = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryAutopilot>();

		ferryTrip.OnPlay.AddListener(() => {
			if (animateOn == AnimateOn.Random) return;

			if (animateOn == AnimateOn.FerryPlay || (animateOn == AnimateOn.FerryBA && ferryTrip.Reversed) || (animateOn == AnimateOn.FerryAB && !ferryTrip.Reversed)) {
				Play();
			}
		});
		ferryTrip.GetComponent<FerryController>().OnConnectToDock.AddListener(() => { 
			if (animateOn == AnimateOn.FerryDocked) {
				Play();
			}
		});

		if (animateOn == AnimateOn.Random) {
			StartCoroutine(NextTrip());
		}
	}

    protected override void Move() {
		distanceTravelled += Throttle * maxSpeed * Time.deltaTime;

		float dst = Reversed ? route.path.length - distanceTravelled : distanceTravelled;
		Vector3 pos = route.path.GetPointAtDistance(dst, PathCreation.EndOfPathInstruction.Stop);
		Quaternion rot = route.path.GetRotationAtDistance(dst, PathCreation.EndOfPathInstruction.Stop);

		transform.position = new Vector3(pos.x, transform.position.y, pos.z);
		transform.rotation = Quaternion.Euler(transform.eulerAngles.x, rot.eulerAngles.y + (Reversed ? 180 : 0), transform.eulerAngles.z);
	}

    public override bool Play() {
		if (Playing) {
			Debug.LogError("Attempted to play " + name + ", but animation is already playing");
			return false;
		}

		if (Random.value >= playChance) return false;

		distanceTravelled = 0;
        return base.Play();
    }

    private IEnumerator NextTrip() {
		yield return new WaitForSeconds(Random.Range(minWaitTime, maxWaitTime));
		Play();
    }

    public override void Stop() {
        base.Stop();

		if (animateOn == AnimateOn.Random) {
			StartCoroutine(NextTrip());
		}
    }
}
