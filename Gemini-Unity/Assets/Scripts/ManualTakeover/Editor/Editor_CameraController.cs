using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEditor.EditorGUILayout;

[CustomEditor(typeof(CameraController))]
public class Editor_CameraController : Editor {
	private CameraController t;

	private void Awake() {
		t = (CameraController)target;
	}

	private bool TrySwap<T>(List<T> list, int i, int j) {
		if (i < 0 || i >= list.Count || j < 0 || j >= list.Count) return false;

		T other = list[j];
		list[j] = list[i];
		list[i] = other;

		return true;
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		Space();

		if (GUILayout.Button("+ Add mount")) {
			t.mounts.Add(new CameraController.Mount());
		}

		if (t.mounts.Count > 0) {
			BeginHorizontal();
			LabelField("Mount");
			GUILayout.FlexibleSpace();
			LabelField("Pan", GUILayout.Width(35));
			LabelField("", GUILayout.Width(60));
			EndHorizontal();
		}

		for (int i = 0; i < t.mounts.Count; i++) {
			CameraController.Mount mount = t.mounts[i];

			BeginHorizontal();

			mount.anchor = ObjectField(mount.anchor, typeof(Transform), true) as Transform;
			mount.canPan = Toggle(mount.canPan, GUILayout.Width(20));

			if (GUILayout.Button("/\\", GUILayout.Width(20))) {
				TrySwap(t.mounts, i, i - 1);
				break;
			}

			if (GUILayout.Button("\\/", GUILayout.Width(20))) {
				TrySwap(t.mounts, i, i + 1);
				break;
			}

			if (GUILayout.Button("X", GUILayout.Width(20))) {
				t.mounts.RemoveAt(i);
				break;  // Don't loop over modified collection
			}

			EndHorizontal();
		}

		if (GUI.changed) {
			EditorUtility.SetDirty(t);
		}
	}
}
