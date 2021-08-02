using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;


[ExecuteInEditMode]
public class CalendarScreen : MonoBehaviour {
	private const float contentBorder = 20f;

	public CalendarDay[] buttons;
	public GameObject weekPlanner;
	public Text[] days;
	public GameObject dayPlanner;
	public Text dayTitle;
	public ScrollRect dayScroll;
	public Dial hourDial;
	public Dial minuteDial;
	public Toggle activeToggle;
	public Button saveButton;
	public Button removeButton;
	public TimePlanner timePlannerPrototype;

	private int selectedDay;
	private RecurringAppointment editedAppointment;
	private bool isNewAppointment;
	private Coroutine settingChange;

	private void Start() {
		hourDial.OnValueChange = OnSettingChanged;
		minuteDial.OnValueChange = OnSettingChanged;
	}

	private void OnEnable() {
		#if UNITY_EDITOR
		if (buttons == null || buttons.Length == 0) buttons = GetComponentsInChildren<CalendarDay>(true);
		#endif

		if (Application.isPlaying) {
			GotoWeekPlanner();

//			DateTime start = new DateTime(2018, 1, 1);

			//grid (maybe unnecessary)
//			int daysInMonth = DateTime.DaysInMonth(2018, 1);
//			for (int i = 0; i < buttons.Length; i++) {
//				if (i < daysInMonth) {
//					DateTime date = start.Add(new TimeSpan(i, 0, 0, 0));
//					buttons[i].Set(date);
//					buttons[i].gameObject.SetActive(true);
//				}
//				else {
//					buttons[i].gameObject.SetActive(false);
//				}
//			}

			//week

		}
	}

	private void BuildWeekPlan() {
		var appointments = AppManager.Instance.calendarData.recurring.OrderBy(rec => rec.Match);
		var tex = new StringBuilder[7];

		for (int i = 0; i < tex.Length; i++) {
			tex[i] = new StringBuilder();
		}

		foreach (var item in appointments) {
			int d = (int) item.day;
			tex[d].Append(item.Match.ToShortTimeString() + Environment.NewLine);
		}

		for (int i = 0; i < days.Length; i++) {
			days[i].text = tex[i].ToString();
		}
	}

	private void BuildDayPlan() {
		DayOfWeek currentDay = (DayOfWeek) selectedDay;
		dayTitle.text = currentDay.ToString();

		ClearContent(dayScroll.content);
		var appointments = AppManager.Instance.calendarData.recurring.Where(rec => rec.day == currentDay).OrderBy(rec => rec.Match).ToArray();
		float prototypeHeight = timePlannerPrototype.GetComponent<RectTransform>().rect.height;
		float requiredHeight = prototypeHeight * (appointments.Length + 1);
		float minimumHeight = ((RectTransform)dayScroll.content.parent).rect.height;
		float x = dayScroll.content.sizeDelta.x;
		float y = Mathf.Max(requiredHeight, minimumHeight);
		dayScroll.content.sizeDelta = new Vector2(x, y);

		//existing time plans
		foreach (var item in appointments) {
			TimePlanner instance = Instantiate<TimePlanner>(timePlannerPrototype, dayScroll.content);
			instance.Set(this, item);
		}

		//new time plan
		TimePlanner newTimePlan = Instantiate<TimePlanner>(timePlannerPrototype, dayScroll.content);
		newTimePlan.Set(this, null);

		newTimePlan.buttonComponent.onClick.Invoke();

		dayScroll.verticalNormalizedPosition = 1f;
	}

	public void OnSettingChanged() {
		MenuController.Instance.ChangeButtonState(saveButton, true);
	}

	public void OnSettingChanged(Dial dial) {
		MenuController.Instance.ChangeButtonState(saveButton, true);
	}

	public void SaveChanges() {
		//set appointment time
		editedAppointment.day = (DayOfWeek) selectedDay;
		editedAppointment.SetTime(hourDial.CurrentValue, minuteDial.CurrentValue);
		editedAppointment.active = activeToggle.isOn;

		//refresh screen
		if (isNewAppointment) AppManager.Instance.calendarData.recurring.Add(editedAppointment);
		MenuController.Instance.ChangeButtonState(saveButton, false);
		BuildDayPlan();
	}

	public void RemoveCurrent() {
		if (!isNewAppointment && editedAppointment != null) {
			AppManager.Instance.calendarData.recurring.Remove(editedAppointment);
			BuildDayPlan();
		}
	}

	public void EditAppointment(RecurringAppointment appointment) {
		if (appointment == null) {
			editedAppointment = new RecurringAppointment();
			isNewAppointment = true;
			DateTime now = DateTime.Now;
			hourDial.CurrentValue = now.Hour;
			minuteDial.CurrentValue = now.Minute;
			activeToggle.isOn = true;
			MenuController.Instance.ChangeButtonState(saveButton, true);
			MenuController.Instance.ChangeButtonState(removeButton, false);
		}
		else {
			editedAppointment = appointment;
			isNewAppointment = false;
			hourDial.CurrentValue = editedAppointment.HourOfDay;
			minuteDial.CurrentValue = editedAppointment.MinuteOfHour;
			activeToggle.isOn = editedAppointment.active;
			MenuController.Instance.ChangeButtonState(saveButton, false);
			MenuController.Instance.ChangeButtonState(removeButton, true);
		}
	}

	public void GotoWeekPlanner() {
		weekPlanner.SetActive(true);
		dayPlanner.SetActive(false);
		BuildWeekPlan();
	}

	public void GotoDayPlanner() {
		weekPlanner.SetActive(false);
		dayPlanner.SetActive(true);
		BuildDayPlan();
	}

	public void DateClicked(int identifier) {
		Debug.Log("Date clicked: " + identifier);
		throw new System.NotImplementedException();
	}

	public void DayClicked(int day) {
		selectedDay = day;
		GotoDayPlanner();
	}

	private static void ClearContent(Transform trans) {
		for (int i = trans.childCount - 1; i >= 0; i--) {
			DestroyImmediate(trans.GetChild(i).gameObject, false);
		}
	}
}