using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestureViewer : MonoBehaviour {
	[SerializeField] private Gesture gesture;
	[Header("Setup")]
	[SerializeField] private Transform arrowPrefab;
	[SerializeField] private Transform cameraTransform;

	private void Awake() {
		if (gesture == null) return;

		Vector3 position = Vector3.zero;
		foreach (var item in gesture.Segments) {
			Transform arrowInstance = Instantiate(arrowPrefab, transform, true);
			arrowInstance.position = position;
			position += item.Direction;
			arrowInstance.LookAt(position);
		}

		Vector3 cameraPosition = position / 2f;
		cameraPosition.z -= position.y + 5f;
		cameraTransform.position = cameraPosition;
	}
}
