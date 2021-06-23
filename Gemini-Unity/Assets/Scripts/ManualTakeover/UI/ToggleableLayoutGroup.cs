using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutGroup))]
public class ToggleableLayoutGroup : MonoBehaviour {
    private Dictionary<string, (Transform transform, int siblingIndex, bool visible)> items = new Dictionary<string, (Transform, int, bool)>();
    private bool setup;

    public bool AnyVisible {
        get {
            foreach ((_, _, bool visible) in items.Values) {
                if (visible) return true;
            }
            return false;
        }
    }

    public HashSet<string> KeysCopy => new HashSet<string>(items.Keys);

    private void Start() {
        Setup();
    }

    public void Setup() {
        if (setup) return;
        setup = true;

        foreach (Transform item in transform) {
            items[item.name] = (item, item.GetSiblingIndex(), false);
        }
        HideAll();
    }

    public void Show(string name) {
        var item = items[name];
        item.transform.parent = transform;
        item.transform.SetSiblingIndex(item.siblingIndex);
        item.visible = true;

        items[name] = item;
    }
    public void Hide(string name) {
        var item = items[name];
        item.transform.parent = null;
        item.visible = false;

        items[name] = item;
    }

    public void Toggle(string name) {
        if (items[name].visible) Hide(name);
        else Show(name);
    }

    public void ShowAll() {
        foreach (string name in KeysCopy) {
            Show(name);
        }
    }

    public void HideAll() {
        foreach (string name in KeysCopy) {
            Hide(name);
        }
    }
}
