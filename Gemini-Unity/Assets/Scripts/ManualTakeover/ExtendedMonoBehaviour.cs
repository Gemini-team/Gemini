using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedMonoBehaviour : MonoBehaviour {
    public void Schedule(System.Action action, float delay) {
        IEnumerator Task() {
            yield return new WaitForSeconds(delay);
            action();
        }
        StartCoroutine(Task());
    }

    public void Repeat(System.Action action, int times, float interval = 0, System.Action onCompletion = null) {
        IEnumerator Task() {
            for (int i = 0; i < times; i++) {
                action();
                yield return new WaitForSeconds(interval);
            }
            onCompletion?.Invoke();
        }
        StartCoroutine(Task());
    }
}
