using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iFitPro;

public class DebugScreen : MonoBehaviour {
	public GameObject title;
	public Text counter;
	public Text statusBar;
	public Button startRecBtn;
	public Button stopRecBtn;
	public Button saveBtn;
	public InputField saveInput;
	public GameObject recordingItems;
	public GameObject debugItems;
	public Transform markerParent;
	public Transform markerPrefab;
	public Transform debugSphere;
	public Transform debugCube;
	public Text movementText;
	public Text gravityText;
	public Toggle gyroToggle;

	private float _time;
	private int frameCount;
	private Vector3 lastPosition = Vector3.zero;
	private int segmentIndex = 0;

	public float time {
		get {
			return _time;
		}
		set {
			_time = value;
			counter.text = time.ToString("F2");
		}
	}

	private void OnEnable() {
		title.SetActive(false);
		gyroToggle.isOn = Detector.UseGyro;
		StatusData.OnDataChanged = RefreshStatus;
		recordingItems.SetActive(false);
		startRecBtn.gameObject.SetActive(true);
		stopRecBtn.gameObject.SetActive(false);
		saveBtn.interactable = false;
		saveInput.interactable = false;
		time = 0f;
		debugItems.SetActive(true);
	}

	private void OnDisable() {
		title.SetActive(true);
		if (Detector.IsRecording) Detector.StopRecording();
		if (debugItems) debugItems.SetActive(false);
	}

	private void Update() {
		movementText.text = string.Format("Recorded: {0}", Detector.Movement);
		gravityText.text = string.Format("Gravity: {0}", Detector.Gravity);
		debugSphere.localPosition = Detector.Movement * 3f;

		if (Detector.IsRecording) {
			time += Time.deltaTime;

			while (segmentIndex < VectorMap.current.directions.Count) {
				lastPosition += VectorMap.current.directions[segmentIndex];
				Transform instance = Instantiate(markerPrefab, markerParent, true);
				instance.localPosition = lastPosition;
				segmentIndex++;
			}
		}
	}

	private void RefreshStatus(string status) {
		statusBar.text = status;
	}

	public void GyroSettingChanged() {
		Detector.UseGyro = gyroToggle.isOn;
	}

	public void StopRec() {
		Detector.StopRecording();
		recordingItems.SetActive(false);
		startRecBtn.gameObject.SetActive(true);
		stopRecBtn.gameObject.SetActive(false);
		saveBtn.interactable = true;
		saveInput.interactable = true;

		VectorMap map = VectorMap.current;
		Gesture gst = map.ToGesture();
		StatusData.Add("Recorded data:");
		StatusData.Add(gst.GetData());
	}

	public void StartRec() {
		segmentIndex = 0;
		lastPosition = Vector3.zero;
		ClearParent();
		Detector.StartRecording();
		time = 0f;
		recordingItems.SetActive(true);
		startRecBtn.gameObject.SetActive(false);
		stopRecBtn.gameObject.SetActive(true);
		saveBtn.interactable = false;
		saveInput.interactable = false;
		StatusData.Add("Recording...");
		frameCount = 0;
	}

	public void Save() {
		string fileName = saveInput.text;
		if (fileName == "" || fileName.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0) {
			StatusData.Add("Error while saving: Invalid file name");
		}
		else {
			try {
				VectorMap map = VectorMap.current;
				Serializer.SaveVectorMap(map, fileName);
				StatusData.Add(string.Format("Save successful: {0}", fileName));
			}
			catch (System.Exception e) {
				StatusData.Add(string.Format("Error while saving: {0}", e.GetType().Name));
			}
		}
	}

	private void ClearParent() {
		foreach (Transform item in markerParent) {
			Destroy(item.gameObject);
		}
	}

	private static class StatusData {
		private const int maxLines = 9;
		private static List<string> lines = new List<string>(15);

		public static System.Action<string> OnDataChanged;

		public static void Add(string txt) {
			lines.Add(txt);
			if (lines.Count >= maxLines) {
				lines.RemoveAt(0);
			}
			OnDataChanged(GetText());
		}

		public static void Add(IEnumerable<string> txt) {
			foreach(string s in txt) {
				Add(s);
			}
		}

		public static string GetText() {
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach(string s in lines) {
				sb.Append("\n" + s);
			}

			return sb.ToString();
		}
	}
}
