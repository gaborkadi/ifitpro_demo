using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
	public static MenuController Instance { get; private set; }
	public static string SelectedScreen { get; set; }

	public GameObject title;
	public GameObject[] screens;
	public Color defaultTextColor;
	public Color disabledTextColor;
	public Color errorColor;

	private GameObject activeScreen;

	private void OnEnable() {
		Instance = this;

		if (User.ActiveUser == null || User.IsForcedLogin) {
			if (!string.IsNullOrEmpty(User.LastUserName)) {
				User user = iFitPro.Serializer.LoadUser(User.LastUserName);
				if (user != null) user.AutoLogin();
			}
		}

		if (User.ActiveUser != null && !User.IsForcedLogin) {
			if (SelectedScreen != null) GotoScreen(SelectedScreen);
			else GotoScreen("Main");
		}
		else {
			GotoScreen("Startup");
		}

		if (Player.Instance == null) Player.CreatePlayer();
	}

	public void GotoScreen(GameObject screen) {
		if (activeScreen != null) {
			if (activeScreen == screen) return;
			else activeScreen.SetActive(false);
		}

		screen.SetActive(true);
		activeScreen = screen;
		SelectedScreen = null;
	}

	private void GotoScreen(string screenName) {
		if (activeScreen != null) {
			if (activeScreen.name == screenName) return;
			else activeScreen.SetActive(false);
		}

		foreach (GameObject screen in screens) {
			if (screen.name == screenName) {
				screen.SetActive(true);
				activeScreen = screen;
			}
			else {
				screen.SetActive(false);
			}
		}

		SelectedScreen = null;
	}
		
	public void ChangeButtonState(Button btn, bool interactable) {
		if (btn.interactable == interactable) return;

		btn.interactable = interactable;
		Color color;
		if (interactable) color = defaultTextColor;
		else color = disabledTextColor;
		btn.transform.Find("Text").GetComponent<Text>().color = color;
	}

	public void ChangeButtonState(Button btn, Text textComponent, bool interactable) {
		btn.interactable = interactable;
		if (interactable) textComponent.color = defaultTextColor;
		else textComponent.color = disabledTextColor;
	}

	/// <summary>
	/// Loads the animation screen with the selected excersise.
	/// Used by buttons.
	/// </summary>
	public void LoadAnimation() {
		LoadingScreen.LoadAnimationScreen();
	}
}