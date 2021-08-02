using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Exercise : ScriptableObject {
	private const string listSign = "- ";

	[SerializeField] private string displayName;
	[SerializeField] private int index;
	[SerializeField] private Sprite sprite;
	[SerializeField] private GestureGroup[] stages;

	public string DisplayName {
		get {
			return displayName;
		}
	}

	public int Index {
		get {
			return index;
		}
	}

	public Sprite ShownSprite {
		get {
			return sprite;
		}
	}

	public GestureGroup[] Stages {
		get {
			return stages;
		}
	}

	public string Instructions {
		get {
			var txt = new System.Text.StringBuilder();
			for (int i = 0; i < stages.Length; i++) {
				txt.Append(listSign);
				txt.Append(stages[i].instructionDetails);
				txt.Append(Environment.NewLine);
			}
			return txt.ToString();
		}
	}
}

[Serializable]
public class GestureGroup {
	/// <summary> minimum segment count difference required to generate new gesture</summary>
	private const int gestureMinDifference = 5;

	private static Coroutine currentJob;

	public string instructionName = "Instruction 1";
	public string instructionDetails;
	public AnimationClip animation;
	[Tooltip("Specify accepted gesture names")]
	public Gesture[] acceptedGestures;

	public static WorkState State { get; private set; }

	static GestureGroup() {
		State = WorkState.None;
	}

	public CatmullRom[] GetSplines() {
		CatmullRom[] splines = new CatmullRom[acceptedGestures.Length];
		for (int i = 0; i < acceptedGestures.Length; i++) {
			splines[i] = acceptedGestures[i].GetSpline(false);
		}
		return splines;
	}

	public float GetAvgTimeSpan() {
		float time = 0f;
		for (int i = 0; i < acceptedGestures.Length; i++) {
			time += acceptedGestures[i].TimeSpan;
		}
		return time / acceptedGestures.Length;
	}

	/*public void StartCompare(Segment[] recorded) {
		currentJob = AppManager.Instance.StartCoroutine(CompareInFrames(recorded));
	}

	public static void StopCompare() {
		if (currentJob != null) {
			if (AppManager.Instance) AppManager.Instance.StopCoroutine(currentJob);
			currentJob = null;
		}

		State = WorkState.None;
	}*/

	/*public IEnumerator CompareInFrames(Segment[] recorded) {
		State = WorkState.Working;

		for (int i = 0; i < acceptedGestures.Length; i++) {
			if (acceptedGestures[i].Compare(recorded)) {
				State = WorkState.Accepted;
				break;
			}
		}

		yield return null;

		if (State != WorkState.Accepted) {
			Debug.Log("Trying advanced compare method...");
			for (int i = 0; i < acceptedGestures.Length; i++) {
				Gesture original = acceptedGestures[i];
				int difference = original.Segments.Length - recorded.Segments.Length;

				if (difference >= gestureMinDifference) {
					Segment[] spline = new Segment[original.Segments.Length]; //59

					//Array.Copy(recorded.Segments, difference, spline, 0, recorded.Segments.Length); //argument exception
					Array.Copy(recorded.Segments, 0, spline, difference, recorded.Segments.Length);

					Gesture generated = ScriptableObject.CreateInstance<Gesture>();

					yield return null;

					generated.Initialize(original.Orientation, spline);
					if (acceptedGestures[i].Compare(generated)) {
						State = WorkState.Accepted;
						break;
					}
				}

				yield return null;
			}
		}

		if (State != WorkState.Accepted) State = WorkState.Declined;
	}*/

	public enum WorkState {
		None,
		Working,
		Accepted,
		Declined
	}
}