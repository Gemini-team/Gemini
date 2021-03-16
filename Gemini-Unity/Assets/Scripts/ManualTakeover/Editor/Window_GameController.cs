#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

public class MyWindow : EditorWindow {
    private bool setup;

    private FerryTrip ferryTrip;
    private DockController[] passengerControllers;

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
            passengerControllers = FindObjectsOfType<DockController>();
        }

        string ferryState = "Idle";
        if (ferryTrip.Playing) ferryState = "In transit";
        else if (ferryTrip.boarding) ferryState = "Boarding";
        else if (ferryTrip.dock != null) ferryState = "Docked at " + ferryTrip.dock.name;
        LabelField(ferryState);

        if (GUILayout.Button((ferryTrip.Playing ? "Stop" : "Start") + " ferry travel")) {
            if (ferryTrip.Playing) ferryTrip.Stop();
            else ferryTrip.Play();
        }

        if (ferryTrip.dock != null) {
            Space();
            LabelField($"{ferryTrip.dock.queue.Count} in queue");
            if (GUILayout.Button("Enqueue passenger")) {
                ferryTrip.dock.MoveToQueue();
            }
            if (GUILayout.Button("Assemble queue")) {
                ferryTrip.dock.queue.AssembleQueue();
            }

            if (GUILayout.Button("Board passengers")) {
                ferryTrip.dock.EmbarkAll();
            }
        }
    }
}
#endif
