using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[ExecuteInEditMode]
public class TextOverlay : MonoBehaviour {
	[Tooltip("The input field connected to this overlay")]
	public InputField field;
	[Tooltip("This transform will be moved when the input field is focused")]
	public Transform moveTransform;
	[Tooltip("The object will be moved to this transform")]
	public Transform positionReference;
	[Tooltip("These objects will be hidden when the input field is focused")]
	public GameObject[] hideOnFocus = new GameObject[0];

	private bool focused = false;
	private Vector3 defaultPosition;
	private KeyValuePair<GameObject, bool>[] stateCache;

	private void Awake() {
		//if running in editor: initialize some fields automatically
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			if (field == null) field = GetComponent<InputField>();
			if (moveTransform == null) moveTransform = transform;
			if (positionReference == null) positionReference = GameObject.FindWithTag("PositionReference").transform;
			if (hideOnFocus == null || hideOnFocus.Length == 0) {
				List<GameObject> hide = new List<GameObject>();
				foreach (Transform child in moveTransform.parent) {
					if (child == moveTransform) continue;
					if (child.GetComponentInChildren<Graphic>(true) == null) continue;
					hide.Add(child.gameObject);
				}
				hideOnFocus = hide.ToArray();
			}
		}
		#endif

		defaultPosition = moveTransform.position;
	}

	private void Update() {
		if (field.isFocused && !focused) {
			focused = true;
			OnFocusRecieved();
		}
		else if (!field.isFocused && focused) {
			focused = false;
			OnFocusLost();
		}
	}

	private void OnFocusRecieved() {
		stateCache = new KeyValuePair<GameObject, bool>[hideOnFocus.Length];

		for (int i = 0; i < stateCache.Length; i++) {
			var current = hideOnFocus[i];
			stateCache[i] = new KeyValuePair<GameObject, bool>(current, current.activeSelf);
			current.SetActive(false);
		}

		moveTransform.position = positionReference.position;
	}

	private void OnFocusLost() {
		for (int i = 0; i < stateCache.Length; i++) {
			var current = stateCache[i];
			current.Key.SetActive(current.Value);
		}

		moveTransform.position = defaultPosition;
		stateCache = null;
	}
}
