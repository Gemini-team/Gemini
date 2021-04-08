#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

public class MyWindow : EditorWindow {
    private bool setup;

    private FerryTrip ferryTrip;
    private bool instant = true;

    [MenuItem("Window/GameController")]
    static void Init() {
        MyWindow window = (MyWindow)GetWindow(typeof(MyWindow));
        window.titleContent = new GUIContent("Game Controller");
        window.Show();
    }

    void Header(string text, params GUILayoutOption[] options) {
        Space();
        LabelField(text, EditorStyles.boldLabel, options);
    }

    void OnGUI() {
        if (!Application.isPlaying) {
            LabelField("Game controller is only enabled in play mode", EditorStyles.helpBox);
            setup = false;
            return;
        }

        if (!setup) {
            setup = true;
			ferryTrip = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryTrip>();
        }

        string ferryState = "Idle";
        if (ferryTrip.Playing) ferryState = "In transit";
        else if (ferryTrip.ferry.boarding) ferryState = "Boarding";
        else if (ferryTrip.ferry.dock != null) ferryState = "Docked at " + ferryTrip.ferry.dock.name;
        LabelField(ferryState);

        if (GUILayout.Button(ferryTrip.Playing ? "Cancel" : "Start ferry travel")) {
            if (ferryTrip.Playing) ferryTrip.Stop();
            else ferryTrip.Play();
        }

        if (ferryTrip.Playing && GUILayout.Button("Skip to end")) {
            ferryTrip.SkipToEnd();
        }

        if (ferryTrip.ferry.dock != null) {
            Space();
            LabelField($"{ferryTrip.ferry.dock.queue.Count} in queue");
            instant = Toggle("Instant enqueue", instant);
            if (GUILayout.Button("Enqueue passenger")) {
                ferryTrip.ferry.dock.MoveToQueue(instant);
            }

            if (GUILayout.Button("Board passengers")) {
                ferryTrip.ferry.dock.EmbarkAll(instant);
            }
        }
    }
}
#endif
