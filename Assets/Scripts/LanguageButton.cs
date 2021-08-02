using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour {
	[SerializeField] private GameObject dropdown;
	[SerializeField] private Image selectedImage;
	[SerializeField] private Sprite[] flags;

	private bool isOpen = false;

	private void OnEnable() {
		Close();
	}

	public void Open() {
		isOpen = true;
		dropdown.SetActive(isOpen);
	}

	public void Close() {
		isOpen = false;
		dropdown.SetActive(isOpen);
	}

	public void Toggle() {
		isOpen = !isOpen;
		dropdown.SetActive(isOpen);
	}

	public void Select(int id) {
		selectedImage.sprite = flags[id];
		Close();
	}
}
