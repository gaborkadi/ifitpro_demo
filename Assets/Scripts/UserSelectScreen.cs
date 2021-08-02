using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iFitPro;

public class UserSelectScreen : MonoBehaviour {
	private const string freeUserText = "FREE SLOT";

	private List<string> userNames;
	public UserButton[] userButtons;

	private void OnEnable() {
		userNames = Serializer.GetLoadableUserNames();
		int i = 0;
		while (i < userNames.Count) {
			UserButton b = userButtons[i];
			b.label.text = userNames[i];
			MenuController.Instance.ChangeButtonState(b.button, b.label, true);
			i++;
		}
		while (i < userButtons.Length) {
			UserButton b = userButtons[i];
			b.label.text = freeUserText;
			MenuController.Instance.ChangeButtonState(b.button, b.label, false);
			i++;
		}
	}

	private void OnDisable() {
		userNames = null;
	}
}
