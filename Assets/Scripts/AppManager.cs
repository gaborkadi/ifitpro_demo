using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using iFitPro;
using Unity.Notifications.iOS;

#if UNITY_IOS
//using UnityEngine.iOS;
using Unity.Notifications.iOS;
#endif

public class AppManager : MonoBehaviour {
	//private const string alertAction = "Go!";
	private const string alertBody = "Time for your exercise!";
	private const string alertTitle = "iFitPro";

	public static AppManager Instance { get; private set; }

	[HideInInspector] public CalendarData calendarData;

	private List<User> changedUsers;
	private bool launchedFromNotification;
	private bool isHandlingFocus = false;

	public bool LaunchedFromNotification { get => launchedFromNotification; }

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);
			return;
		}

		changedUsers = new List<User>();
		
#if UNITY_IOS
		//UnityEngine.iOS.NotificationServices.RegisterForNotifications(NotificationType.Alert | NotificationType.Badge | NotificationType.Sound);
#endif

		calendarData = Serializer.LoadCalendar();
		if (calendarData == null) calendarData = new CalendarData();

		Detector.UseGyro = SystemInfo.supportsGyroscope;

		OnAppFocusGained();
	}

	public void UserDataChanged(User user) {
		if (!changedUsers.Contains(user)) {
			changedUsers.Add(user);
		}
	}

	/// <summary>
	/// Adds the next notification. Calling this method will clear all previously scheduled notifications.
	/// </summary>
	public void AddNextNotification() {
#if UNITY_IOS
		//NotificationServices.CancelAllLocalNotifications();
		iOSNotificationCenter.RemoveAllDeliveredNotifications();
		iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif

		if (calendarData == null) return;
		DateTime next = CalendarData.Never;
		if (!calendarData.NextAppointment(out next)) return;

		//Dictionary<string, string> userInfo = new Dictionary<string, string>();
		//userInfo.Add("OpenMode", "Notification");

#if UNITY_IOS
		/*LocalNotification notification = new LocalNotification();
		notification.alertAction = alertAction;
		notification.alertBody = alertBody;
		notification.fireDate = next;
		notification.hasAction = true;
		notification.userInfo = userInfo;
		NotificationServices.ScheduleLocalNotification(notification);*/

		iOSNotificationCalendarTrigger trigger = new iOSNotificationCalendarTrigger() {
			Year = next.Year,
			Month = next.Month,
			Day = next.Day,
			Hour = next.Hour,
			Minute = next.Minute,
			Second = next.Second,
			Repeats = false
		};

		iOSNotification notification = new iOSNotification(CreateNotificationID(next)) {
			Body = alertBody,
			Title = alertTitle,
			ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
			Trigger = trigger,
			ShowInForeground = true
		};

		iOSNotificationCenter.ScheduleNotification(notification);
#endif
	}

	private void Update () {
		if (changedUsers.Count > 0) {
			for (int i = 0; i < changedUsers.Count; i++) {
				Serializer.SaveUser(changedUsers[i]);
			}
			changedUsers.Clear();
		}

		if (Input.GetKeyDown(KeyCode.Escape)) {
			Quit();
		}
	}

	private void OnAppFocusLost() {
		isHandlingFocus = false;

#if UNITY_IOS
		iOSNotificationCenter.RemoveAllDeliveredNotifications();
		iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif

		if (calendarData != null && calendarData.notificationsEnabled) {
			AddNextNotification();
			calendarData.Cleanup();
			Serializer.SaveCalendar(calendarData);
		}
	}

	private void OnAppFocusGained() {
		if (isHandlingFocus) return;

		isHandlingFocus = true;
		iOSNotification notification = iOSNotificationCenter.GetLastRespondedNotification();
		launchedFromNotification = notification != null;

		if (launchedFromNotification) {
			if (User.ActiveUser == null || User.IsForcedLogin) {
				if (!string.IsNullOrEmpty(User.LastUserName)) {
					User user = Serializer.LoadUser(User.LastUserName);
					if (user != null) {
						User.ForceLoginUser(user);
					}
					else {
						Debug.LogError("Last user is null but notifications are not cancelled!");
					}
				}
			}

			if (User.ActiveUser != null) {
				AnimScreen.exercise = Resources.Load<ExerciseGroup>("Exercises/AllExercises").GetRandom();
				LoadingScreen.LoadAnimationScreen();
			}
			else {
				LoadingScreen.LoadMenu();
			}
		}
		else {
			LoadingScreen.LoadMenu();
		}

		iOSNotificationCenter.RemoveAllDeliveredNotifications();
		iOSNotificationCenter.RemoveAllScheduledNotifications();
	}

	public void OnAnimScreenLoaded() {
		isHandlingFocus = false;
	}

	public void Quit() {
		OnAppFocusLost();

		//whatever needs to be done before quit

		Application.Quit();
	}

	private void OnApplicationPause(bool status) {
		if (status) {
			OnAppFocusLost();
		}
		else {
			OnAppFocusGained();
		}
	}

	private void OnApplicationFocus(bool hasFocus) {
		if (hasFocus) {
			OnAppFocusGained();
		}
		else {
			OnAppFocusLost();
		}
	}

	public static string CreateNotificationID(DateTime time) {
		return string.Format("n_{0}_{1}_{2}_{3}_{4}_{5}", time.Year, time.Month, time.Day, time.Hour, time.Minute, time.Second);
	}
}
