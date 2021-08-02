using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using iFitPro;

public class RegisterScreen : MonoBehaviour {
	private const string nameExistsError = "User name already exists!";
	private const string nameShortError = "Name too short!";
	private const string emailInvalidError = "Email address is not valid!";
	private const string dateInvalidError = "Birth date is not valid!";
	private const string panelNameDefault = "Register new user";

	[Header("General")]
	public GameObject registrationPanel;
	public GameObject passwordPanel;
	[Header("Registration")]
	public InputField nameField;
	public InputField emailField;
	public InputField monthField;
	public InputField yearField;
	public Button nextBtn;
	public Text panelName;
	public GameObject panelAfterRegister;
	private List<string> userNames;
	[Header("Password")]
	public InputField pwField;
	public InputField confirmPwField;
	public Button okBtn;
	public Toggle rememberToggle;
	public GameObject pwMismatchError;

	private bool NameAllowed {
		get {
			return nameField.text.Length > 0;
		}
	}

	private bool NameFree {
		get {
			return !userNames.Contains(nameField.text);
		}
	}

	private bool DateValid {
		get {
			return monthField.text.Length > 0 && yearField.text.Length > 0;
		}
	}

	private bool EmailValid {
		get {
			return Dobro.Text.RegularExpressions.Email.IsEmail(emailField.text);
		}
	}

	private bool PasswordMatches {
		get {
			return pwField.text == confirmPwField.text;
		}
	}

	private void OnEnable() {
		GotoRegistration();
		nameField.text = "";
		emailField.text = "";
		monthField.text = "";
		yearField.text = "";
		pwField.text = "";
		confirmPwField.text = "";
		userNames = Serializer.GetLoadableUserNames();
		OnPasswordChange();
		panelName.text = panelNameDefault;
		panelName.color = MenuController.Instance.disabledTextColor;
		MenuController.Instance.ChangeButtonState(nextBtn, false);
	}

	private void OnDisable() {
		userNames = null;
	}

	public void TryRegister() {
		if (NameFree && PasswordMatches) {
			User user = new User(nameField.text, pwField.text, rememberToggle.isOn);
			if (User.LoginUser(user, pwField.text)) {
				Serializer.SaveUser(user);
				MenuController.Instance.GotoScreen(panelAfterRegister);
			}
		}
	}

	public void OnMonthChanged() {
		if (monthField.text.Length >= 2) {
			OnMonthEnd();
		}
	}

	public void OnMonthEnd() {
		if (monthField.text.Length == 0) return;

		int month = int.Parse(monthField.text);
		month = Mathf.Clamp(month, 1, 12);
		monthField.text = month.ToString("00");
		yearField.Select();
	}

	public void OnYearEnd() {
		if (yearField.text.Length == 0) return;

		int year = int.Parse(yearField.text);
		year = Mathf.Clamp(year, 1900, System.DateTime.Now.Year);
		yearField.text = year.ToString("0000");

		if (year == System.DateTime.Now.Year) {
			monthField.text = System.DateTime.Now.Month.ToString("00");
		}
	}

	public void OnAnyChange() {
		if (!NameAllowed) {
			panelName.text = nameShortError;
			panelName.color = MenuController.Instance.errorColor;
			MenuController.Instance.ChangeButtonState(nextBtn, false);
		}
		else if (!NameFree) {
			panelName.text = nameExistsError;
			panelName.color = MenuController.Instance.errorColor;
			MenuController.Instance.ChangeButtonState(nextBtn, false);
		}
		else if (!EmailValid) {
			panelName.text = emailInvalidError;
			panelName.color = MenuController.Instance.errorColor;
			MenuController.Instance.ChangeButtonState(nextBtn, false);
		}
		else if (!DateValid) {
			panelName.text = dateInvalidError;
			panelName.color = MenuController.Instance.errorColor;
			MenuController.Instance.ChangeButtonState(nextBtn, false);
		}
		else {
			panelName.text = panelNameDefault;
			panelName.color = MenuController.Instance.disabledTextColor;
			MenuController.Instance.ChangeButtonState(nextBtn, true);
		}
	}

	public void OnPasswordChange() {
		bool match = PasswordMatches;
		MenuController.Instance.ChangeButtonState(okBtn, match);
		pwMismatchError.SetActive(!match);
	}

	public void GotoRegistration() {
		registrationPanel.SetActive(true);
		passwordPanel.SetActive(false);
	}

	public void GotoPassword() {
		registrationPanel.SetActive(false);
		passwordPanel.SetActive(true);
	}
}