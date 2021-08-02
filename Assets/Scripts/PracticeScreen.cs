using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PracticeScreen : MonoBehaviour {
	public enum PracticeContent {
		Animation,
		Text
	}

	public const int animsPerPage = 15;

	[Header("General")]
	public GameObject mainPanel;
	public GameObject closeUpPanel;
	[Header("Main panel")]
	public AnimPage[] pages;
	public Button okButtonMain;
	public Button leftButtonMain;
	public Button rightButtonMain;
	public Text pageNum;
	[Header("Close up panel")]
	public Button leftButtonClose;
	public Button rightButtonClose;
	public Text animNum;
	public Image animImage;
	public Text animTitle;
	public Text animText;
	public GameObject animationContent;
	public GameObject textContent;
	[HideInInspector]
	public AnimButton selectedAnimButton;
	private int activePage;
	private PracticeContent displayedContent;
	public Exercise[] exercises { get; private set; }

	private bool PageLeftAllowed {
		get {
			return activePage > 0;
		}
	}

	private bool PageRightAllowed {
		get {
			return activePage < pages.Length - 1;
		}
	}

	public PracticeContent DisplayedContent {
		get {
			return displayedContent;
		}
		set {
			displayedContent = value;

			if (value == PracticeContent.Animation) {
				animationContent.SetActive(true);
				textContent.SetActive(false);
			}
			else {
				animationContent.SetActive(false);
				textContent.SetActive(true);
			}
		}
	}

	private void OnEnable() {
		exercises = Resources.LoadAll<Exercise>("Exercises");
		System.Array.Sort(exercises, delegate(Exercise x, Exercise y) { return x.Index.CompareTo(y.Index); });
		GotoMainPanel();
		ResetPages();
		SetPage(0);
		DisplayedContent = PracticeContent.Animation;
		AnimButton.practiceScreen = this;
		AnimSelected(null);
	}

	private void OnDisable() {
		selectedAnimButton = null;
	}

	private void ResetPages() {
		activePage = -1;
		for (int i = 1; i < pages.Length; i++) {
			pages[i].SetActive(false);
		}

		SetPageButtons();
		pageNum.text = (activePage + 1).ToString();
	}

	public void SetPage(int page) {
		if (activePage >= 0 && activePage < pages.Length) {
			pages[activePage].SetActive(false);
		}
		if (page >= 0 && page < pages.Length) {
			pages[page].SetActive(true);
			activePage = page;
		}

		SetPageButtons();
		pageNum.text = (activePage + 1).ToString();

		int firstAnimID = activePage * animsPerPage;
		int nextPageAnimID = (activePage + 1) * animsPerPage;
		int animID = 0;
		AnimPage currentPage = pages[activePage];
		for (int i = firstAnimID; i < exercises.Length && i < nextPageAnimID; i++) {
			AnimButton button = currentPage.buttons[animID];
			button.exercise = exercises[i];
			button.image.sprite = exercises[i].ShownSprite;
			animID++;
		}
	}

	public void PageLeft() {
		if (PageLeftAllowed) {
			SetPage(activePage - 1);
		}
	}

	public void PageRight() {
		if (PageRightAllowed) {
			SetPage(activePage + 1);
		}
	}

	private void SetPageButtons() {
		leftButtonMain.interactable = PageLeftAllowed;
		rightButtonMain.interactable = PageRightAllowed;
	}

	public void GotoMainPanel() {
		mainPanel.SetActive(true);
		closeUpPanel.SetActive(false);
	}

	public void GotoCloseUpPanel() {
		if (selectedAnimButton == null) return;

		mainPanel.SetActive(false);
		closeUpPanel.SetActive(true);
		Exercise exercise = selectedAnimButton.exercise;
		animImage.sprite = exercise.ShownSprite;
		animTitle.text = exercise.DisplayName;
		animText.text = exercise.Instructions;
		//		set left/right button
		//		set anim number

		AnimScreen.exercise = exercise;
	}

	public void ToggleContent() {
		if (DisplayedContent == PracticeContent.Animation) DisplayedContent = PracticeContent.Text;
		else DisplayedContent = PracticeContent.Animation;
	}

	public void Back() {
		if (DisplayedContent == PracticeContent.Text) DisplayedContent = PracticeContent.Animation;
		else GotoMainPanel();
	}

	public void AnimSelected(AnimButton button) {
		if (button == null || button.exercise == null) {
			selectedAnimButton = null;
			MenuController.Instance.ChangeButtonState(okButtonMain, false);
		}
		else {
			selectedAnimButton = button;
			MenuController.Instance.ChangeButtonState(okButtonMain, true);
		}
	}
}