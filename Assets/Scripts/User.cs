using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class User {
	public const int maxUsers = 5;
	private static User activeUser;
	private static string activeUserPW;
	private static bool isForcedLogin;

	[SerializeField]
	private string name;
	[SerializeField]
	private string pass;
	[SerializeField]
	private bool remember;
	[SerializeField]
	private Calendar calendar;
	[SerializeField]
	private int birthYear;
	[SerializeField]
	private int birthMonth;

	public static User ActiveUser {
		get {
			return activeUser;
		}
	}

	public static Calendar ActiveUserCalendar {
		get {
			if (activeUser == null) return null;
			return activeUser.GetCalendar(activeUserPW);
		}
	}

	public static bool ActiveUserRemember {
		get {
			if (activeUser == null) return false;
			return activeUser.Remember;
		}
		set {
			if (activeUser == null) return;
			activeUser.SetRemember(value, activeUserPW);
		}
	}

	public static string LastUserName {
		get {
			return PlayerPrefs.GetString("LastUser", null);
		}
	}

	public static bool IsForcedLogin {
		get {
			return isForcedLogin;
		}
	}

	public string Name {
		get {
			return name;
		}
	}

	public bool Remember {
		get {
			return remember;
		}
	}

	public int BirthYear {
		get {
			return birthYear;
		}
	}

	public int BirthMonth {
		get {
			return birthMonth;
		}
	}

	public string FakePassword {
		get {
			if (remember) {
				return new String('X', 10);
			}
			else {
				return "";
			}
		}
	}

	public User () { }

	public User (string name, string pass, bool remember) {
		this.name = name;
		this.pass = pass;
		this.remember = remember;
	}

	public static void LogoutUser() {
		activeUser = null;
		activeUserPW = "";
	}

	/// <summary>
	/// Log in specified user with given password.
	/// </summary>
	public static bool LoginUser(User user, string pass) {
		if (user.PasswordMatches(pass) || user.Remember) {
			activeUser = user;
			activeUserPW = user.pass;
			isForcedLogin = false;
			PlayerPrefs.SetString("LastUser", user.name);
			return true;
		}
		else {
			return false;
		}
	}

	/// <summary>
	/// Temporarily log in specified user without password.
	/// Used for Animation scene.
	/// </summary>
	public static bool ForceLoginUser(User user) {
		isForcedLogin = true;
		activeUser = user;
		return true;
	}

	/// <summary>
	/// Log in this user automatically if they selected "remember me".
	/// </summary>
	public bool AutoLogin() {
		if (remember) {
			return LoginUser(this, pass);
		}
		else {
			return false;
		}
	}

	public bool PasswordMatches(string pass) {
		return this.pass == pass;
	}

	public bool SetRemember(bool remember, string pass) {
		if (PasswordMatches(pass)) {
			this.remember = remember;
			AppManager.Instance.UserDataChanged(this);
			return true;
		}
		else {
			return false;
		}
	}

	public bool ChangePassword(string oldPass, string newPass) {
		if (!isForcedLogin && PasswordMatches(oldPass)) {
			this.pass = newPass;
			if (this == activeUser) activeUserPW = newPass;
			AppManager.Instance.UserDataChanged(this);
			return true;
		}
		else {
			return false;
		}
	}

	public Calendar GetCalendar(string pass) {
		if (!isForcedLogin && PasswordMatches(pass)) {
			return calendar;
		}
		else {
			return null;
		}
	}
}

[Serializable]
public class Calendar {
	[SerializeField]
	private List<DateTime> exercises = new List<DateTime>();

	public bool HasExercises {
		get {
			return exercises.Count > 0;
		}
	}

	public DateTime Next {
		get {
			int count = exercises.Count;
			if (count == 0) return DateTime.MinValue;

			DateTime now = DateTime.Now;

			for (int i = 0; i < count; i++) {
				if (exercises[i] < now) continue;
				return exercises[i];
			}

			return DateTime.MinValue;
		}
	}

	public void AddDate(DateTime date) {
		exercises.Add(date);
		exercises.Sort();
	}

	public void RemoveDate(DateTime date) {
		exercises.RemoveAll(x => x == date);
	}

	public void RemoveObsolete() {
		DateTime now = DateTime.Now;
		exercises.RemoveAll(x => x < now);
	}
}