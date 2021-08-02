using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VectorMap {
	public static VectorMap current;

	public DeviceOrientation orientation = DeviceOrientation.LandscapeRight;
	public List<Point3D> directions = new List<Point3D>();

	public VectorMap () { }

	public VectorMap (DeviceOrientation orientation) {
		this.orientation = orientation;
	}

	private Vector3 LastDirection {
		get {
			if (directions.Count > 0) return directions[directions.Count - 1];
			return Vector3.zero;
		}
	}

	public float TimeSpan {
		get {
			if (directions == null) return 0f;
			else return directions.Count * Detector.detectionTime;
		}
	}

	public void AddVector(Vector3 direction) {
		directions.Add(direction);
	}

	public Gesture ToGesture() {
		Segment[] segments = new Segment[directions.Count];
		int count = directions.Count;
		for (int i = 0; i < count; i++) {
			segments[i] = new Segment(directions[i]);
		}
		Gesture gesture = ScriptableObject.CreateInstance<Gesture>();
		gesture.Initialize(orientation, segments);
		return gesture;
	}

	public Segment[] ToSegments() {
		Segment[] segments = new Segment[directions.Count];
		int count = directions.Count;
		for (int i = 0; i < count; i++) {
			segments[i] = new Segment(directions[i]);
		}
		return segments;
	}
}

[System.Serializable]
public struct Point3D {
	public float x;
	public float y;
	public float z;

	public Point3D(float nX, float nY, float nZ) {
		x = nX;
		y = nY;
		z = nZ;
	}

	public Point3D(Vector3 v3) {
		x = v3.x;
		y = v3.y;
		z = v3.z;
	}

	public override string ToString () {
		return string.Format ("Point3D: {0}, {1}, {2}", x, y, z);
	}

	public static implicit operator Point3D (Vector3 v3) {
		return new Point3D(v3);
	}

	public static implicit operator Vector3 (Point3D p3) {
		return new Vector3(p3.x, p3.y, p3.z);
	}
}