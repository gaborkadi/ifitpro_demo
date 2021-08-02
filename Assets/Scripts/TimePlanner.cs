using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TimePlanner : MonoBehaviour {
	public Text textComponent;
	public Button buttonComponent;

	public void Set(CalendarScreen screen, RecurringAppointment appointment) {
		//trigger
		UnityEngine.Events.UnityAction act = new UnityEngine.Events.UnityAction(delegate {
			screen.EditAppointment(appointment);
		});
		buttonComponent.onClick.AddListener(act);

		if (appointment == null) {
			textComponent.text = "New...";
			textComponent.color = MenuController.Instance.disabledTextColor;
		}
		else {
			textComponent.text = appointment.Match.ToShortTimeString();
			textComponent.color = appointment.active ? MenuController.Instance.defaultTextColor : MenuController.Instance.disabledTextColor;
		}
	}


}
