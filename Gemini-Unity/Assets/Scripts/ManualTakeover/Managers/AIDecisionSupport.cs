using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDecisionSupport : MonoBehaviour {
    private const float WARN_AT_TIME_LEFT = 10f;
    private const string TAG = "AI";

    private NotificationWidget notifications;

    private IEnumerator WarnFailureImminent(float delay, string message) {
        yield return new WaitForSeconds(delay - WARN_AT_TIME_LEFT);
        notifications.PushNotification(message, tag: TAG, bgColor: UIColors.Warning);
    }

    private void Start() {
        if (PlayerPrefs.GetInt("AIDecisionSupport") != 1) {
            enabled = false;
            return;
        }

        GameObject ferry = GameObject.FindGameObjectWithTag("Player");
        notifications = FindObjectOfType<NotificationWidget>();

        notifications.PushNotification("AI decision support system\nOnline", tag: TAG);

        FindObjectOfType<Scenario>().OnManualTakeoverImminent.AddListener((delay, message) => {
            StartCoroutine(WarnFailureImminent(delay, message));
        });

        foreach (AnimatedAutopilot boatTrip in FindObjectsOfType<AnimatedAutopilot>()) {
            boatTrip.OnPlay.AddListener(() => notifications.PushNotification("Moving boat detected\nCollision avoidance system engaged", tag: TAG));
        }
    }
}
