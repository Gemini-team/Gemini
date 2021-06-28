using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(AnimatedAutopilot))]
public class Editor_AnimatedAutopilot : Editor {
    private AnimatedAutopilot t;

    private void Awake() {
        t = (AnimatedAutopilot)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        if (t.animateOn == AnimatedAutopilot.AnimateOn.Random) {
            Space();
            MinMaxSlider("Wait Time", ref t.minWaitTime, ref t.maxWaitTime, 0, 60);
        }
    }
}
