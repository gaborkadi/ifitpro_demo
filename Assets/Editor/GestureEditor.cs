using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Gesture))]
public class GestureEditor : Editor {
	private static int selectedOption = 0;
	private static bool foldout = false;

	public override void OnInspectorGUI () {
		Gesture gesture = (Gesture) target;
		EditorGUILayout.BeginHorizontal();
		Gesture[] gestures = Resources.LoadAll<Gesture>("Gestures");
		string[] options = gestures.Select(x => x.name).ToArray();
		selectedOption = Mathf.Clamp(selectedOption, 0, options.Length - 1);
		selectedOption = EditorGUILayout.Popup("Compare", selectedOption, options);
		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Compare")) {
			Gesture other = gestures[selectedOption];
			bool match = gesture.Compare(other);
			Debug.Log(string.Format("{0} vs {1} : {2}", gesture.name, other.name, match));
		}

		GUILayout.Space(20f);

		if (GUILayout.Button("Display")) {
			Transform parent = new GameObject(gesture.name).transform;
			Vector3 pos = Vector3.zero;
			for (int i = 0; i < gesture.Segments.Length; i++) {
				Transform t = GameObject.CreatePrimitive(PrimitiveType.Sphere).transform;
				t.SetParent(parent, true);
				t.localScale = new Vector3(0.2f, 0.2f, 0.2f);
				pos += gesture.Segments[i].Direction;
				t.position = pos;
			}
		}

		GUILayout.Space(20f);

		EditorGUILayout.HelpBox("These values are recorded / calculated and can not be modified.", MessageType.Info);

		GUILayout.Space(20f);

		GUILayout.Label(string.Format("Time taken: {0:F2}", gesture.TimeSpan));

		EditorGUILayout.LabelField(string.Format("{0} {1}", "Orientation:", gesture.Orientation));

		foldout = EditorGUILayout.Foldout(foldout, string.Format("Segments ({0})", gesture.Segments.Length));
		if (foldout) {
			for (int i = 0; i < gesture.Segments.Length; i++) {
				DrawSegment(gesture.Segments[i], i);
			}
		}
	}

	private static void DrawSegment(Segment segment, int id) {
		GUILayout.Label("#" + id);
		EditorGUILayout.Vector3Field("Direction", segment.Direction);
		EditorGUILayout.FloatField("Magnitude", segment.Direction.magnitude);
		GUILayout.Space(5f);
	}
}