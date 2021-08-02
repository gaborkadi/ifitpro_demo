using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainScreen : MonoBehaviour {
	public MenuController menuController;
	public GameObject panelAfterLogout;

	public void Logout() {
		User.LogoutUser();
		menuController.GotoScreen(panelAfterLogout);
	}
}
