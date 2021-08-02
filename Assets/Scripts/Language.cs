using UnityEngine;
using System;

public static class Language {
	private static string activeLanguageID;

	public static Action<string> OnLanguageChanged { get; set; }

	/// <summary>
	/// Currently selected language ID.
	/// </summary>
	public static string SelectedID {
		get {
			if (string.IsNullOrEmpty(activeLanguageID)) {
				activeLanguageID = PlayerPrefs.GetString("Language", "English");
			}
			return activeLanguageID;
		}
		set {
			activeLanguageID = value;
			PlayerPrefs.SetString("Language", value);
			//load
			if (OnLanguageChanged != null) OnLanguageChanged(value);
		}
	}


}
