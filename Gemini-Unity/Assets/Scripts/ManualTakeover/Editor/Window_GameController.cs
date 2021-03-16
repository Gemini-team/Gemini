#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

public class MyWindow : EditorWindow {
    private bool setup;

    private FerryTrip ferryTrip;
    private DockController[] passengerControllers;

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
			ferryTrip = GameObject.FindGameObjectWithTag("Player").GetComponent<FerryTrip>();
            passengerControllers = FindObjectsOfType<DockController>();
        }

        Header("Ferry");
        if (GUILayout.Button((ferryTrip.Playing ? "Stop" : "Start") + " ferry travel")) {
            if (ferryTrip.Playing) ferryTrip.Stop();
            else ferryTrip.Play();
        }

        foreach (DockController controller in passengerControllers) {
            Header(controller.gameObject.name);

            LabelField($"{controller.queue.Count} in queue");
            if (GUILayout.Button("Enqueue passenger")) {
                controller.MoveToQueue();
            }
            if (GUILayout.Button("Assemble queue")) {
                controller.queue.AssembleQueue();
            }

            Space();
            if (GUILayout.Button("Board passengers")) {
                controller.EmbarkAll();
            }
        }
    }
}
#endif
