using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimButton : MonoBehaviour {
	public static PracticeScreen practiceScreen;

	[HideInInspector] public Image image;
	[HideInInspector] public Exercise exercise;

	public void OnClick() {
		practiceScreen.AnimSelected(this);
	}
}
