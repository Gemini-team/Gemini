using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public Canvas canvas;
    public Text alertMessage;

    public void Alert(string message, float? duration=null) {
        IEnumerator Draw() {
            alertMessage.text = message;
            if (duration.HasValue) {
                yield return new WaitForSeconds(duration.Value);
                alertMessage.text = "";
            }
        }
        StartCoroutine(Draw());
    }
}
