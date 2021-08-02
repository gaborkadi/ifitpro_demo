using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iFitPro;

public class StartupScreen : MonoBehaviour {
	public Button registerBtn;
	public Button loginBtn;

	private void OnEnable() {
		List<string> users = Serializer.GetLoadableUserNames();

		MenuController.Instance.ChangeButtonState(registerBtn, users.Count < User.maxUsers);
		MenuController.Instance.ChangeButtonState(loginBtn, users.Count > 0);
	}
}
