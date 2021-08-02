using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimScreen : MonoBehaviour {
	private const bool logsEnabled = true;
	private const int checkFreqFrames = 3;
	private const float allowedOvertime = 2.5f;

	public static Exercise exercise;
	private static int stage;

	public Text instructionText;
	public GameObject exerciseError;
	public Image timeDisplay;
	public float flashingSpeed = 2f;
	public GameObject characterCamera;
	public Animator animator;

	private int framesPassed;
	private float originalTime;
	private float allowedTime;
	private float flashTime;
	private float timePassed;
	private CatmullRom[] originalSplines;
	//private Coroutine evaluation;
	private bool gesturePassed = false;
	private float lastEvalTime;

	private static List<Segment[]> gesturesWaiting;

	private static GestureGroup CurrentGestureGroup {
		get {
			return exercise.Stages[stage];
		}
	}

	private void OnEnable() {
		if (AppManager.Instance != null) AppManager.Instance.OnAnimScreenLoaded();

		if (animator == null) {
			if (Player.Instance == null) animator = Player.CreatePlayer().Animator;
			else animator = Player.Instance.Animator;
		}

		Time.timeScale = 1f;
		instructionText.text = "";
		timeDisplay.gameObject.SetActive(false);
		gesturesWaiting = new List<Segment[]>();
		SetStage(0);
	}

	private void OnDisable() {
		//stop compare
		gesturesWaiting.Clear();
	}

	private void Update() {
		if (!gesturePassed && timePassed < allowedTime && Detector.IsRecording) {
			//check if time to evaluate
			framesPassed++;
			if (framesPassed >= checkFreqFrames) {
				framesPassed = 0;
				if (VectorMap.current.directions.Count > 4) QueueEvaluation();
			}
			timePassed += Time.deltaTime;
		}

		if (Detector.IsRecording) timeDisplay.fillAmount = Mathf.Clamp01(timePassed / allowedTime);
		else timeDisplay.fillAmount = 1f;

		//evaluate
		if (Detector.IsRecording) {
			if (gesturesWaiting.Count > 0) {
				//gestures waiting -> evaluate
				lastEvalTime = Time.realtimeSinceStartup;

				for (int g = 0; g < gesturesWaiting.Count; g++) {
					CatmullRom generatedSpline = new CatmullRom(gesturesWaiting[0], false);
					for (int s = 0; s < originalSplines.Length; s++) {
						float accuracy;
						if (Gesture.Compare(originalSplines[s], generatedSpline, originalTime)) {
							OnPassed();
							return;
						}
						else if (Time.realtimeSinceStartup - lastEvalTime > 0.03f) {
							lastEvalTime = Time.realtimeSinceStartup;
							return;
						}
					}
					gesturesWaiting.RemoveAt(0);
				}
			}
			else if (timePassed >= allowedTime) {
				//time up, no gestures to evaluate -> break out

				OnFailed();
				return;
			}
			else {
				//waiting for gestures to evaluate

				lastEvalTime = Time.realtimeSinceStartup;
				return;
			}
		}
	}

	private static void QueueEvaluation() {
		Segment[] segments = VectorMap.current.ToSegments();

		if (segments.Length < 12) {
			gesturesWaiting.Add(segments);
		}
		else {
			int limit = segments.Length - 4;
			for (int first = 0; first < limit; first++) {
				Segment[] generatedSegments = new Segment[segments.Length - first];
				System.Array.Copy(segments, first, generatedSegments, 0, generatedSegments.Length);
				gesturesWaiting.Add(generatedSegments);
			}
		}
	}

	private void OnPassed() {
		if (logsEnabled) Debug.Log("unity-ifit: Gesture passed");
		StopRecording();
		timeDisplay.gameObject.SetActive(false);
		NextStage();
	}

	private void OnFailed() {
		if (logsEnabled) Debug.Log("unity-ifit: Gesture failed");
		StopRecording();
		characterCamera.SetActive(false);
		timeDisplay.gameObject.SetActive(false);
		exerciseError.SetActive(true);
	}

	public void PlayAnimation(AnimationClip clip) {
		if (animator != null && clip != null) {
			animator.Play(clip.name, 0, 0f);
		}
	}

	private void StartRecording() {
		framesPassed = 0;
		flashTime = 0f;
		Detector.StartRecording();
		timeDisplay.gameObject.SetActive(true);
	}

	private void StopRecording() {
		Detector.StopRecording();
	}

	public void SetStage(int s) {
		if (exercise == null || exercise.Stages == null || exercise.Stages.Length == 0) return;

		stage = s;
		exerciseError.SetActive(false);
		timePassed = 0f;

		if (stage < exercise.Stages.Length) {
			//show next
			instructionText.text = CurrentGestureGroup.instructionDetails;
			originalTime = CurrentGestureGroup.GetAvgTimeSpan();
			allowedTime = originalTime + allowedOvertime;
			originalSplines = CurrentGestureGroup.GetSplines();
			characterCamera.SetActive(true);
			PlayAnimation(CurrentGestureGroup.animation);
			StartRecording();
		}
		else {
			characterCamera.SetActive(false);
			//finish

			if (AppManager.Instance.LaunchedFromNotification) {
				//TODO:
				//show message: "bye" or stg
				Application.Quit();
			}
			else {
				LoadingScreen.LoadPracticeSelector();
			}
		}
	}

	/// <summary>
	/// Used by button.
	/// </summary>
	public void GotoPracticeScreen() {
		LoadingScreen.LoadPracticeSelector();
	}

	public void NextStage() {
		SetStage(stage + 1);
	}

	public void ReloadStage() {
		SetStage(stage);
	}
}
