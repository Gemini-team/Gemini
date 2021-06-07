using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTrip : AnimatedTrip {
	public enum AnimateOn { AB, BA, Both };

	[Space(10)]
	public AnimateOn animateOn;

	private FerryTrip ferryTrip;
	private bool DoAnimate => (animateOn == AnimateOn.Both) || (animateOn == AnimateOn.BA) == ferryTrip.reverse;

	protected override void Start() {
		base.Start();

		ferryTrip = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryTrip>();

		ferryTrip.OnPlay.AddListener(() => {
			if (!DoAnimate) Stop();
			else {
				Play();
			}
		});

		ferryTrip.OnStop.AddListener(Stop);
	}
}
