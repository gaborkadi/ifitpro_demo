using UnityEngine;

[System.Serializable]
public struct Segment {
	[SerializeField] private Vector3 direction;

	public Vector3 Direction {
		get {
			return direction;
		}
	}

	public Segment(Vector3 direction) {
		this.direction = direction;
	}
}