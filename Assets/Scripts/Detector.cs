using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Detector : MonoBehaviour {
	public const float detectionTime = 0.1f;

	private static bool useGyro;
	private static Action recordingAction;
	private static bool isRecording = false;
	private static float timeLeft;

	public static Vector3 Gravity { get; private set; }
	public static Vector3 Movement { get; private set; }

	public static DeviceOrientation Orientation { get; private set; }

	public static bool UseGyro {
		get {
			return useGyro;
		}
		set {
			useGyro = value;
			Input.gyro.enabled = useGyro;
			if (useGyro) recordingAction = RecordGyro;
			else recordingAction = RecordAccelero;
		}
	}

	public static bool IsRecording {
		get {
			return isRecording;
		}
	}

	private void OnEnable() {
		Orientation = DeviceOrientation.LandscapeRight;
	}

	public static void StartRecording() {
		VectorMap.current = new VectorMap(Orientation);
		isRecording = true;
		timeLeft = detectionTime;
	}

	public static void StopRecording() {
		isRecording = false;
	}

	private void Update() {
		SetOrientation();
		recordingAction();
	}

	private static void SetOrientation() {
		var ori = Input.deviceOrientation;
		if (ori == DeviceOrientation.LandscapeLeft || ori == DeviceOrientation.LandscapeRight) Orientation = ori;
	}

	private static void RecordGyro() {
		Movement = Input.gyro.userAcceleration;
		Gravity = Input.gyro.gravity;

		if (isRecording) {
			if (timeLeft <= 0f) {
				timeLeft = detectionTime + timeLeft - Time.deltaTime;
				print("recording gyro");
				VectorMap.current.AddVector(Movement);
				
			}
			else {
				timeLeft -= Time.deltaTime;
			}
		}
	}

	private static void RecordAccelero() {
		Vector3 rawAcceleration = Input.acceleration;
		Vector3 acceleration = new Vector3 (rawAcceleration.x, rawAcceleration.z, rawAcceleration.y);
		Gravity = 0.9f * Gravity + 0.1f * acceleration;
		Movement = acceleration - Gravity;

		if (isRecording) {
			if (timeLeft <= 0f) {
				print("recording accelero");
				timeLeft = detectionTime + timeLeft - Time.deltaTime;
				VectorMap.current.AddVector(Movement);
			}
			else {
				timeLeft -= Time.deltaTime;
			}
		}
	}
}