using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class CalendarDay : MonoBehaviour {
	public Text dayText;
	public Text dataText;

	private void OnEnable() {
		#if UNITY_EDITOR
		dayText = transform.GetChild(0).GetComponent<Text>();
		//dayText.text = "0";
		dataText = GetComponent<Text>();
		//dataText.text = "";
		#endif
	}

	public void Set(System.DateTime date) {
		dayText.text = date.Day.ToString();
	}

	public void Set(System.DateTime date, System.DateTime firstAppointment, bool multipleAppointments) {
		dayText.text = date.Day.ToString();

		if (multipleAppointments) {
			dataText.text = string.Format("{0:D2}:{1:D2}\n...", firstAppointment.Hour, firstAppointment.Minute);
		}
		else {
			dataText.text = string.Format("{0:D2}:{1:D2}", firstAppointment.Hour, firstAppointment.Minute);
		}
	}

	public void Clicked() {

	}
}
