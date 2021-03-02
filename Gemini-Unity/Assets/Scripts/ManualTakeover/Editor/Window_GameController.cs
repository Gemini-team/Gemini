#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

public class MyWindow : EditorWindow {
    private bool setup;

    private MovePath path;
    private PassengerManager[] passengerManagers;
    private int startNode;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/GameController")]
    static void Init() {
        // Get existing open window or if none, make a new one:
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
            path = FindObjectOfType<MovePath>();
            passengerManagers = FindObjectsOfType<PassengerManager>();
        }

        Header("Ferry");
        LabelField(path.Playing ? $"On node {path.atIndex} / {path.NodeCount}" : "Idle");
        startNode = IntField("Start node", startNode);

        if (GUILayout.Button((path.Playing ? "Stop" : "Start") + " ferry travel")) {
            if (path.Playing) path.Stop();
            else path.Play(startNode);
        }
        EditorGUI.BeginDisabledGroup(path.Playing);
        if (GUILayout.Button("Move to start")) {
            path.MoveToNode(startNode);
        }
        EditorGUI.EndDisabledGroup();

        foreach (PassengerManager manager in passengerManagers) {
            Header(manager.gameObject.name);

            LabelField($"{manager.queue.Count} in queue");
            if (GUILayout.Button("Enqueue passenger")) {
                manager.controller.Embark();
            }
            if (GUILayout.Button("Assemble queue")) {
                manager.queue.AssembleQueue();
            }
        }
    }
}
#endif
