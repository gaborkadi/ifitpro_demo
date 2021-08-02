using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Dial : MonoBehaviour {
	public int minValue = 0;
	public int maxValue = 100;
	public System.Action<Dial> OnValueChange;
	public Text display;

	[SerializeField] private int currentValue;

	private void OnEnable() {
		display.text = currentValue.ToString();
	}

	public int CurrentValue {
		get {
			return currentValue;
		}
		set {
			currentValue = value;
			if (value < minValue) currentValue = maxValue;
			else if (value > maxValue) currentValue = minValue;
			else currentValue = value;

			display.text = currentValue.ToString();
			if (OnValueChange != null) OnValueChange(this);
		}
	}

	public void DialNegative() {
		CurrentValue--;
	}

	public void DialPositive() {
		CurrentValue++;
	}
}