#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

public class MyWindow : EditorWindow {
    private bool setup;

    private Scenario scenario;
    private FerryTrip ferryTrip;

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
            scenario = FindObjectOfType<Scenario>();
        }

        if (scenario.Playing) {
            LabelField("Playing scenario...");
            if (Time.timeScale > 1 && GUILayout.Button("Normal playblack")) {
                Time.timeScale = 1;
            } else if (Time.timeScale == 1 && GUILayout.Button("Fast forward")) {
                Time.timeScale = 10;
            }

            if (GUILayout.Button("Move passengers to destination")) {
                foreach (Passenger passenger in FindObjectsOfType<Passenger>()) {
                    passenger.transform.position = passenger.agent.destination;
                }
            }
            return;
        }

        string ferryState = "Idle";
        if (ferryTrip.Playing) ferryState = "In transit";
        else if (ferryTrip.ferry.boarding) ferryState = "Boarding";
        else if (ferryTrip.ferry.AtDock != null) ferryState = "Docked at " + ferryTrip.ferry.AtDock.name;
        LabelField(ferryState);

        if (GUILayout.Button("Play scenario")) {
            scenario.Play();
        }
        
        if (GUILayout.Button(ferryTrip.Playing ? "Cancel" : "Start ferry travel")) {
            if (ferryTrip.Playing) ferryTrip.Stop();
            else ferryTrip.Play();
        }

        if (ferryTrip.Playing && GUILayout.Button("Skip to end")) {
            ferryTrip.SkipToEnd();
        }

        if (ferryTrip.ferry.AtDock != null) {
            Space();
            LabelField($"{ferryTrip.ferry.AtDock.queue.Count} in queue");
            if (GUILayout.Button("Spawn passenger")) {
                Passenger passenger = ferryTrip.ferry.AtDock.SpawnPassenger();
            }

            if (GUILayout.Button("Board passengers")) {
                ferryTrip.ferry.AtDock.PassengerDeparture();
            }
        }
    }
}
#endif
