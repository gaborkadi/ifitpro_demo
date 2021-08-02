using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour {
	private static LoadingScreen instance;
	private static string sceneToLoad;

	public static System.Action<string> onSceneLoaded;

	//yeah... had to do it with invokes because coroutines didn't work on ios for some reason

	//generic load
	public static void Load(string scene) {
		if (instance == null) instance = Instantiate(Resources.Load<LoadingScreen>("LoadingScreen"));
		instance.gameObject.SetActive(true);
		sceneToLoad = scene;
		instance.Invoke("LoadNextFrame", 0.001f);
	}

	private void LoadNextFrame() {
		SceneManager.LoadScene(sceneToLoad);
		instance.Invoke("DisablePanel", 0.001f);
	}

	//menu load
	public static void LoadMenu() {
		if (instance == null) instance = Instantiate(Resources.Load<LoadingScreen>("LoadingScreen"));
		instance.gameObject.SetActive(true);
		sceneToLoad = "Menu";
		instance.Invoke("LoadNextFrame", 0.001f);
	}

	//menu load and go to practice selector
	public static void LoadPracticeSelector() {
		if (instance == null) instance = Instantiate(Resources.Load<LoadingScreen>("LoadingScreen"));
		instance.gameObject.SetActive(true);
		sceneToLoad = "Menu";
		MenuController.SelectedScreen = "Practice";
		instance.Invoke("LoadNextFrame", 0.001f);
	}

	//animation screen load
	public static void LoadAnimationScreen() {
		Load("Animation");
	}

	private void DisablePanel() {
		gameObject.SetActive(false);
	}
}
