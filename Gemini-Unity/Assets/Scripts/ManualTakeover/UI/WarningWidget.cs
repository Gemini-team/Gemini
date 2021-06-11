using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningWidget : ExtendedMonoBehaviour {
    private const float BLINK_INTERVAL = 1;

    private Color BlinkColor(bool on) => on ? new Color(0.85f, 0.2f, 0, 1) : new Color(1, 1, 1, 0.5f);

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
                image.color = BlinkColor(on);
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
