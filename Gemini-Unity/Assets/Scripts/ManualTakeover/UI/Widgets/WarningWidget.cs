using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningWidget : ExtendedMonoBehaviour {
    private const float BLINK_INTERVAL = 1;

    public float duration = 15;

    private List<Coroutine> coroutines = new List<Coroutine>();
    private Image image;
    private Text text;

    private void CancelCoroutines() {
        foreach (Coroutine co in coroutines) {
            StopCoroutine(co);
        }
        coroutines.Clear();
    }

    public void ShowWarning(string message) {
        IEnumerator Blink() {
            bool on = true;
            while (true) {
                image.color = on ? UIColors.Alert : UIColors.Background;
                yield return new WaitForSeconds(BLINK_INTERVAL);
                on = !on;
            }
        }

        CancelCoroutines();

        text.text = message;
        coroutines.Add(StartCoroutine(Blink()));
        coroutines.Add(Schedule(HideWarning, duration));
    }

    public void HideWarning() {
        CancelCoroutines();

        image.color = Color.clear;
        text.text = "";
    }

    private void Start() {
        image = GetComponent<Image>();
        text = GetComponentInChildren<Text>();

        HideWarning();
    }
}
