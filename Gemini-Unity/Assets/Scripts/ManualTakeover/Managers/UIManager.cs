using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : ExtendedMonoBehaviour {
    public Canvas canvas;
    public Text alertMessage;

    public void Alert(string message, float? duration=null) {
        alertMessage.text = message;
        if (duration.HasValue) Schedule(() => alertMessage.text = "", duration.Value);
    }

    public void SetBar(string name, float value) {
        canvas.transform.Find(name).GetComponent<Image>().fillAmount = value;
    }

    public void Toggle(string name, bool state) {
        canvas.transform.Find(name).gameObject.SetActive(state);
    }
}
