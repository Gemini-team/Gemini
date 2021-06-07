using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncTicket {
	private int nextTicket, nowServing;
	private float minProcessTime, maxProcessTime;

	public SyncTicket(float processTime) {
		minProcessTime = processTime;
		maxProcessTime = processTime;
	}

	public SyncTicket(float minProcessTime, float maxProcessTime) {
		this.minProcessTime = minProcessTime;
		this.maxProcessTime = maxProcessTime;
	}

	public IEnumerator TakeTicketAndWait(System.Action action) {
		int ticket = nextTicket++;
		yield return new WaitUntil(() => nowServing == ticket);
		action();
		yield return new WaitForSeconds(Random.Range(minProcessTime, maxProcessTime));
		nowServing++;
	}
}
