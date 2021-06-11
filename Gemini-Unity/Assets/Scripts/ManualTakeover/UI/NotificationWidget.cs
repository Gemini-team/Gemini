﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Scrolling notification feed. Assumes its parent has a UI mask
/// </summary>
[RequireComponent(typeof(VerticalLayoutGroup))]
public class NotificationWidget : MonoBehaviour {
	public GameObject notificationPrefab;
	public int count;
	public float animationDuration = 1;
	
	private Queue<(string, string, Color)> messages = new Queue<(string, string, Color)>();

	private bool animating;
	private bool Cycle => transform.childCount > count;
	
	private float animationDistance, startTime;

	private void Start() {
		if (count <= 0) throw new System.ArgumentOutOfRangeException("Count must be at least 1");
		if (notificationPrefab == null) throw new System.ArgumentNullException("Notification prefab is not set");

		animationDistance = notificationPrefab.GetComponent<RectTransform>().rect.height + GetComponent<VerticalLayoutGroup>().spacing;

		StartCoroutine(DrawNotifications());
	}

	public void PushNotification(string message, string tag=null, Color? bgColor=null) {
		messages.Enqueue((message, tag, bgColor.GetValueOrDefault(UIColors.Background)));
	}

	private IEnumerator DrawNotifications() {
		while (true) {
			yield return new WaitUntil(() => messages.Count > 0 && !animating);
			var (message, tagText, bgColor) = messages.Dequeue();

			if (!Cycle) Instantiate(notificationPrefab, transform);

			Transform notification = transform.GetChild(transform.childCount - 1);
			Transform tag = notification.Find("Tag");

			notification.GetComponent<Image>().color = bgColor;
			notification.Find("Text").GetComponent<Text>().text = message;

			tag.GetComponent<Image>().color = tagText == null ? Color.clear : bgColor;
			tag.Find("Text").GetComponent<Text>().text = tagText ?? "";

			// Animation flicker fix
			if (!Cycle) transform.localPosition = Vector3.down * animationDistance;

			startTime = Time.time;
			animating = true;
		}
	}
	
	private void Update() {
		if (animating) {
			// Check if animation is over first, in case animationDuration <= 0
			if (Time.time - startTime >= animationDuration) {
				if (Cycle) {
					transform.GetChild(0).SetSiblingIndex(transform.childCount - 1);
				}

				transform.localPosition = Vector3.zero;
				animating = false;
				return;  // Return here to prevent possible division by zero (if animationDuration == 0)
			}

			float offset = Mathf.Lerp(0, animationDistance, (Time.time - startTime) / animationDuration);
			if (!Cycle) offset -= animationDistance;
			transform.localPosition = Vector3.up * offset;
		}
	}
}
