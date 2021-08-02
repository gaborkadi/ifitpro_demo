using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iFitPro;

public class LoginScreen : MonoBehaviour {
	public Text selectedUser;
	public InputField passwordField;
	public Toggle rememberToggle;
	public GameObject incorrectPW;
	public GameObject panelAfterLogin;
	private User loadedUser;

	private void OnDisable() {
		loadedUser = null;
	}

	public void GotoLogin(UserButton button) {
		MenuController.Instance.GotoScreen(gameObject);
		selectedUser.text = button.label.text;

		string userName = selectedUser.text;
		string password = passwordField.text;
		loadedUser = Serializer.LoadUser(userName);
		if (loadedUser == null) Debug.LogError(string.Format("Selected user does not exist ({0})", userName));

		passwordField.text = loadedUser.FakePassword;
		rememberToggle.isOn = loadedUser.Remember;
		incorrectPW.SetActive(false);
	}

	public void TryLogin() {
		string userName = selectedUser.text;
		string password = passwordField.text;

		if (loadedUser.PasswordMatches(password) || loadedUser.Remember) {
			User.LoginUser(loadedUser, password);
			if (User.ActiveUserRemember != rememberToggle.isOn) User.ActiveUserRemember = rememberToggle.isOn;
			MenuController.Instance.GotoScreen(panelAfterLogin);
		}
		else {
			incorrectPW.SetActive(true);
		}
	}
}
