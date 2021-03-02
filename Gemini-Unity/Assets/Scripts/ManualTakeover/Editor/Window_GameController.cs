#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

public class MyWindow : EditorWindow {
    private bool setup;

    private MovePath path;
    private GameObject[] passengerManagers;
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
            passengerManagers = GameObject.FindGameObjectsWithTag("PassengerManager");
        }

        Header("Ferry");
        startNode = IntField("Start node", startNode);
        BeginHorizontal();
        if (GUILayout.Button((path.Playing ? "Stop" : "Start") + " ferry travel")) {
            if (path.Playing) path.Stop();
            else path.Play(startNode);
        }
        LabelField(path.Playing ? $"On node {path.atIndex} / {path.NodeCount}" : "Idle");
        EndHorizontal();

        foreach (GameObject obj in passengerManagers) {
            Header(obj.name);

            BeginHorizontal();
            if (GUILayout.Button("Enqueue passenger")) {
                obj.transform.Find("PassengerArea").GetComponent<PassengerController>().Embark();
            }
            LabelField($"{obj.transform.Find("Queue").GetComponent<PassengerQueue>().Count} in queue");
            EndHorizontal();
        }
    }
}
#endif
