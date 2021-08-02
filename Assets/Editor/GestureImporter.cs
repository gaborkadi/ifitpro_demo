using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using iFitPro;

public class GestureImporter : EditorWindow {
	//assets relative
	private const string rawFileLocation = "Editor/RecordedGestures";
	private const string importedFileLocation = "Editor/RecordedGestures/Imported";
	private const string gestureLocation = "Resources/Gestures";
	private const string gestureExtension = ".asset";

	//project relative
	private const string assetsRawFileLocation = "Assets/Editor/RecordedGestures";
	private const string assetsImportedFileLocation = "Assets/Editor/RecordedGestures/Imported";
	private const string assetsGestureLocation = "Assets/Resources/Gestures";

	//system
	private static string fullRawFileLocation;
	private static string fullImportedFileLocation;
	private static string fullgestureLocation;

	private static string newFileName = "New gesture";

	private string selectedFile = "";

	[MenuItem("Window/Gesture importer")]
	public static void Init() {
		fullRawFileLocation = Path.Combine(Application.dataPath, rawFileLocation);
		fullImportedFileLocation = Path.Combine(Application.dataPath, importedFileLocation);
		fullgestureLocation = Path.Combine(Application.dataPath, gestureLocation);

		GestureImporter window = (GestureImporter)EditorWindow.GetWindow(typeof(GestureImporter));
		window.Show();
	}

	private void OnGUI() {
		GUIStyle titleStyle = new GUIStyle();
		titleStyle.fontSize = 18;
		titleStyle.fontStyle = FontStyle.Bold;
		GUIStyle textStyle = new GUIStyle();
		titleStyle.fontSize = 14;
		titleStyle.fontStyle = FontStyle.Normal;
		GUIStyle box = new GUIStyle();
		box.margin = new RectOffset(5, 5, 5, 5);

		//title, labels
		EditorGUILayout.BeginVertical(box);
		GUILayout.Label("Gesture importer", titleStyle);
		GUILayout.Space(20f);
		GUILayout.Label("Raw files location: " + rawFileLocation, textStyle);
		GUILayout.Label("Gesture location: " + gestureLocation, textStyle);
		GUILayout.Space(20f);

		//check files
		DirectoryInfo dirInfo = new DirectoryInfo(fullRawFileLocation);
		FileInfo[] files = dirInfo.GetFiles("*.gst", SearchOption.TopDirectoryOnly);
		if (files == null || files.Length == 0) GUI.enabled = false;
		else GUI.enabled = true;

		//convert all
		if (GUILayout.Button("Convert all")) {
			for (int i = 0; i < files.Length; i++) {
				FileInfo file = files[i];
				ImportFile(file);
			}
			AssetDatabase.Refresh(ImportAssetOptions.Default);
		}
		GUI.enabled = true;
		GUILayout.Space(10f);

		//convert selected
		GUILayout.BeginHorizontal();
		selectedFile = GUILayout.TextField(selectedFile, GUILayout.Width(200));
		if (GUILayout.Button("Open", GUILayout.Width(60)))
			selectedFile = EditorUtility.OpenFilePanel("Select file", fullRawFileLocation, "gst");
		if (selectedFile == "") GUI.enabled = false;
		if (GUILayout.Button("Convert")) {
			FileInfo file = new FileInfo(selectedFile);
			ImportFile(file);
			AssetDatabase.Refresh(ImportAssetOptions.Default);
		}
		GUILayout.EndHorizontal();
		GUI.enabled = true;
		GUILayout.Space(10f);

		//create gesture
		GUILayout.BeginHorizontal();
		newFileName = GUILayout.TextField(newFileName, GUILayout.Width(200));
		FileInfo newGesture = new FileInfo(CombineFileName(fullgestureLocation, newFileName, gestureExtension));
		if (newGesture.Exists || newFileName == "") GUI.enabled = false;
		else GUI.enabled = true;
		if (GUILayout.Button("Create new")) {
			CreateGesture();
		}
		GUILayout.EndHorizontal();
		GUI.enabled = true;
		GUILayout.Space(20f);
		EditorGUILayout.EndVertical();
	}

	private static string CombineFileName(string path, string fileName, string extension) {
		if (!extension.StartsWith(".")) extension = "." + extension;
		if (fileName.EndsWith(extension)) return Path.Combine(path, fileName);
		else return Path.Combine(path, fileName) + extension;
	}

	private static string CombineAssetName(string path, string fileName, string extension) {
		if (!extension.StartsWith(".")) extension = "." + extension;
		if (!path.EndsWith("/")) path += "/";
		return path + fileName + extension;
	}

	private static void ImportFile(FileInfo file) {
		//load map and create gesture
		VectorMap map = Serializer.LoadVectorMap(file.FullName);
		Gesture gesture = map.ToGesture();
		string gestureName = Path.GetFileNameWithoutExtension(file.FullName);
		gesture.name = gestureName;

		//create gesture asset
		string assetPath = CombineAssetName(assetsGestureLocation, gestureName, gestureExtension);
		AssetDatabase.CreateAsset(gesture, assetPath);
		Debug.Log("Imported gesture: " + gesture.name);

		//move imported file
		string movedFilePath = CombineFileName(fullImportedFileLocation, gestureName, Serializer.vectorMapExt);
		File.Move(file.FullName, movedFilePath);
	}

	private static void CreateGesture() {
		Gesture gesture = ScriptableObject.CreateInstance<Gesture>();
		string assetPath = CombineAssetName(assetsGestureLocation, newFileName, gestureExtension);
		AssetDatabase.CreateAsset(gesture, assetPath);
		Debug.Log("Created new gesture: " + gesture.name);
	}
}