using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CalendarData {
	public bool notificationsEnabled = true;
	public List<OneTimeAppointment> oneTime = new List<OneTimeAppointment>();
	public List<RecurringAppointment> recurring = new List<RecurringAppointment>();

	public static DateTime Never {
		get {
			return new DateTime(9000, 1, 1);
		}
	}

	/// <summary>
	/// Check if an appointment exists. Sets the "next" parameter to the next time.
	/// </summary>
	/// <returns><c>true</c>, if a valid appointment exists, <c>false</c> otherwise.</returns>
	/// <param name="next">Next appointment in line, or year 9000 if invalid.</param>
	public bool NextAppointment(out DateTime next) {
		DateTime now = DateTime.Now;
		DateTime closest = Never;
		bool valid = false;

		for (int i = 0; i < oneTime.Count; i++) {
			if (!oneTime[i].active) continue;

			DateTime current = oneTime[i].time;
			if (current > now && current < closest) {
				closest = current;
				valid = true;
			}
		}

		for (int i = 0; i < recurring.Count; i++) {
			if (!recurring[i].active) continue;

			DateTime current = recurring[i].Match;
			if (current > now && current < closest) {
				closest = current;
				valid = true;
			}
		}

		next = closest;
		return valid;
	}

	/// <summary>
	/// Clean out any past appointments.
	/// </summary>
	public void Cleanup() {
		DateTime now = DateTime.Now;
		oneTime.RemoveAll(x => x.time <= now);
	}
}

[Serializable]
public abstract class Appointment {
	public bool active = true;
}

[Serializable]
public class OneTimeAppointment : Appointment {
	public DateTime time;
}

[Serializable]
public class RecurringAppointment : Appointment {
	public DayOfWeek day;
	public int minuteOfDay;

	public int HourOfDay {
		get {
			return Mathf.FloorToInt(minuteOfDay / 60f);
		}
	}

	public int MinuteOfHour {
		get {
			return Mathf.FloorToInt(minuteOfDay % 60f);
		}
	}

	public DateTime Match {
		get {
			DateTime now = DateTime.Now;
			int daysToNext = (int) day - (int) now.DayOfWeek;
			if (daysToNext < 0) daysToNext += 7;

			DateTime nextDate = now.Add(new TimeSpan(daysToNext, 0, 0));

			int h = HourOfDay;
			int m = MinuteOfHour;
			int s = 0;

			return new DateTime(nextDate.Year, nextDate.Month, nextDate.Day, h, m, s);
		}
	}

	public void SetTime(int hour, int minute) {
		minuteOfDay = hour * 60 + minute;
	}
}

//calendar:
//on setup check times for next activation -> set up notification
//on activation check times for next activation -> set up notification

//on activation	-> exercise
//				-> skip
//				-> disable notifications