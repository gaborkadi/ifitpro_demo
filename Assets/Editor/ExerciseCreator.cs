using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExerciseCreator {
	[MenuItem("Assets/Create/Exercise")]
	public static void CreateExercise() {
		var exercise = ScriptableObject.CreateInstance<Exercise>();
		AssetDatabase.CreateAsset(exercise, "Assets/Resources/Exercises/NewExercise.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = exercise;
	}

	[MenuItem("Assets/Create/ExerciseGroup")]
	public static void CreateExerciseContainer() {
		var exerciseContainer = ScriptableObject.CreateInstance<ExerciseGroup>();
		AssetDatabase.CreateAsset(exerciseContainer, "Assets/Resources/Exercises/NewGroup.asset");
		AssetDatabase.SaveAssets();

		EditorUtility.FocusProjectWindow();
		Selection.activeObject = exerciseContainer;
	}
}
