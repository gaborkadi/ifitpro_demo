using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Gesture : ScriptableObject {
#if UNITY_EDITOR
	public const bool logsEnabled = true;
#else
	public const bool logsEnabled = false;
#endif

	private const float splineLengthMin = 0.8f;		//original length * min
	private const float splineLengthMax = 1.4f;		//original length * max
	private const float angleThreshold = 180f;     //degrees between original and generated
	private const int evaluationSplitMin = 8;       //this is how many times the spline is checked (min)
	private const int evaluationSplitMax = 32;      //this is how many times the spline is checked (max)
	private const int evaluationsPerSecond = 4;		//this is how many evaluations each second adds
	private const float minPassAccuracy = 0.65f;		//min accuracy required for the comparation to pass

	[Tooltip("Do not modify!")]
	[SerializeField]
	private DeviceOrientation orientation = DeviceOrientation.LandscapeRight;

	[Tooltip("Do not modify!")]
	[SerializeField]
	private Segment[] segments;

	public DeviceOrientation Orientation {
		get {
			return orientation;
		}
	}

	public Segment[] Segments {
		get {
			return segments;
		}
	}

	public float TimeSpan {
		get {
			if (segments == null) return 0f;
			else return segments.Length * Detector.detectionTime;
		}
	}

	public CatmullRom GetSpline(bool closedLoop = false) {
		if (segments.Length < 4) return null;
		else return new CatmullRom(segments, closedLoop);
	}

	public string[] GetData() {
		string[] logs = new string[2];
		logs[0] = "- segment count: " + segments.Length;
		logs[1] = "- approx time: " + TimeSpan;
		return logs;
	}

	public void Initialize(DeviceOrientation orientation, Segment[] spline) {
		this.orientation = orientation;
		this.segments = spline;
	}

	public bool Compare(Gesture other) {
		CatmullRom original = GetSpline(false);
		if (other.segments.Length < 10) {
			return Compare(original, other.GetSpline(), TimeSpan);
		}
		else {
			//set end segment (i) starting from 9
			for (int last = 9; last < other.segments.Length; last++) {
				//set start segment (j) starting from 0, to last - 4 (because 4 segments are minimum required for spline)
				for (int first = 0; first < last - 4; first++) {
					Segment[] generatedSegments = new Segment[last - first];
					System.Array.Copy(other.segments, first, generatedSegments, 0, generatedSegments.Length);
					CatmullRom generated = new CatmullRom(generatedSegments, false);
					if (Compare(original, generated, TimeSpan)) return true;
				}
			}
			return false;
		}
	}

	public static bool Compare(CatmullRom original, CatmullRom generated, float originalTimeSpan) {
#if UNITY_EDITOR
		if (Application.isPlaying) return false;
#endif
		//check length
		int minLength = Mathf.RoundToInt(original.SplinePointCount * splineLengthMin);
		if (generated.SplinePointCount < minLength) {
			return false;
		}

		int maxLength = Mathf.RoundToInt(original.SplinePointCount * splineLengthMax);
		if (generated.SplinePointCount > maxLength) {
			return false;
		}

		int evaluations = Mathf.Clamp(Mathf.RoundToInt(originalTimeSpan * evaluationsPerSecond), evaluationSplitMin, evaluationSplitMax);
		float split = 1f / evaluations;
		float accuracy = 0f;
		for (int i = 0; i < evaluations; i++) {
			float t = split * i;
			float angle = Vector3.Angle(original.GetPoint(t).position, generated.GetPoint(t).position);
			accuracy += 1f - Mathf.Clamp01(angle / angleThreshold);
		}
		accuracy /= evaluations;
		if (logsEnabled) Debug.Log("Compare finished... (accuracy: " + accuracy + ")");

		if (accuracy >= minPassAccuracy) {
			if (logsEnabled) Debug.Log("Compare passed!");
			return true;
		}
		else {
			if (logsEnabled) Debug.Log("Compare failed!");
			return false;
		}
	}








	/*public const float directionMaxValue = 0.4f;
	public const float angleMaxValue = 0.4f;
	public const float timeMaxValue = 0.2f;
	public const float directionThreshold = 45f;
	public const float angleThreshold = 90f;
	public const float timeThreshold = 0.5f;
	public const float acceptTreshold = 0.6f;

	[Tooltip("Do not modify!")]
	[SerializeField]
	private DeviceOrientation orientation = DeviceOrientation.LandscapeRight;

	[Tooltip("Do not modify!")]
	[SerializeField]
	private Segment[] segments;

	public DeviceOrientation Orientation {
		get {
			return orientation;
		}
	}

	public Segment[] Segments {
		get {
			return segments;
		}
	}

	public float TimeSpan {
		get {
			if (segments == null) return 0f;
			else return segments.Length * Detector.detectionTime;
		}
	}

	public static float CompareDirection(Vector3 d1, Vector3 d2) {
		float angle = Vector3.Angle(d1, d2);
		return 1f - Mathf.Clamp01(angle / Gesture.directionThreshold);
	}

	private static float CompareAngles(Vector3 d1Current, Vector3 d1Previous, Vector3 d2Current, Vector3 d2Previous) {
		float angle1 = Vector3.Angle(d1Current, d1Previous);
		float angle2 = Vector3.Angle(d2Current, d2Previous);
		return 1 - Mathf.Clamp01(Mathf.Abs(angle1 - angle2) / angleThreshold);
	}

	private static float CompareTime(int originalSegments, int comparedSegments) {
		float orig = (float) originalSegments;
		float comp = (float) comparedSegments;
		float threshold = orig * timeThreshold;
		float difference = Mathf.Abs(orig - comp);
		return 1 - Mathf.Clamp01(difference / threshold);
	}

	public void Initialize(DeviceOrientation orientation, Segment[] spline) {
		this.orientation = orientation;
		this.segments = spline;
	}

	public string[] GetData() {
		string[] logs = new string[2];
		logs[0] = "- segment count: " + segments.Length;
		logs[1] = "- approx time: " + TimeSpan;
		return logs;
	}

	public Segment GetSegmentAt(float p) {
		if (p <= 0f) return segments[0];
		else if (p >= 1f) return segments[segments.Length - 1];

		float exactPoint = segments.Length * p;
		float remainder = exactPoint % 1f;
		int floor = Mathf.FloorToInt(exactPoint);
		int ceil = Mathf.Min(floor + 1, segments.Length - 1);

		Segment floorSegment = segments[floor];
		Segment ceilSegment = segments[ceil];
		Vector3 direction = Vector3.Slerp(floorSegment.Direction, ceilSegment.Direction, remainder);
		return new Segment(direction);
	}

	public bool Compare(Gesture other) {
		int count = segments.Length;
		float part = 1f / count;
		float directionTotal = 0f;
		float angleTotal = 0f;
		//float compensator = GetRotationCompensator(orientation, other.orientation);
		Vector3 thisPrevious = new Vector3();
		Vector3 otherPrevious = new Vector3();

		for (int i = 0; i < count; i++) {
			Segment otherSegment = other.GetSegmentAt(part * i);
			Vector3 thisDirection = segments[i].Direction;
			Vector3 otherDirection = otherSegment.Direction;
			//Vector3 otherDirection = CompensateOrientation(otherSegment.Direction, compensator);
			directionTotal += CompareDirection(thisDirection, otherDirection);
			angleTotal += CompareAngles(thisDirection, thisPrevious, otherDirection, otherPrevious);

			thisPrevious = thisDirection;
			otherPrevious = otherDirection;
		}

		float directionAccuracy = (directionTotal / count) * directionMaxValue;
		float angleAccuracy = (angleTotal / count) * angleMaxValue;
		float timeAccuracy = CompareTime(count, other.segments.Length) * timeMaxValue;
		float totalAccuracy = directionAccuracy + angleAccuracy + timeAccuracy;

		return totalAccuracy >= acceptTreshold;
	}*/

	/*private static float GetRotationCompensator(DeviceOrientation original, DeviceOrientation recorded) {
		if (original == recorded) return 0f;
		else return 180f;
	}*/

	/*private static Vector3 CompensateOrientation(Vector3 euler, float compensator) {
		return Quaternion.Euler(new Vector3(0f, 0f, compensator)) * euler;
	}*/
}