using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class AnimPage : MonoBehaviour {
	public AnimButton[] buttons;

	public void SetActive(bool value) {
		gameObject.SetActive(value);
	}

#if UNITY_EDITOR
	public void OnEnable() {
		if (buttons == null || buttons.Length == 0) buttons = GetComponentsInChildren<AnimButton>();
	}
#endif
}
