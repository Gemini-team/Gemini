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
}
