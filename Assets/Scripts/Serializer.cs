using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace iFitPro {
	public static class Serializer {
		public const string vectorMapExt = ".gst";
		private const string userExt = ".iFitness";
		private const string calendarExt = ".dat";
		private const string gesturesFolder = "Gestures";
		private const string userFolder = "User";
		private const string calendarFolder = "Data";

		public static string GestureSavePath {
			get {
				return Path.Combine(Application.persistentDataPath, gesturesFolder);
			}
		}

		public static string UserPath {
			get {
				return Path.Combine(Application.persistentDataPath, userFolder);
			}
		}

		public static string CalendarDataPath {
			get {
				return Path.Combine(Application.persistentDataPath, calendarFolder);
			}
		}

		public static void SaveVectorMap(VectorMap map, string fileName) {
			DirectoryInfo dir = new DirectoryInfo(GestureSavePath);
			if (!dir.Exists) dir.Create();

			if (!fileName.EndsWith(vectorMapExt)) fileName += vectorMapExt;
			string path = Path.Combine(GestureSavePath, fileName);

			FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, map);
			stream.Close();
		}

		#if UNITY_EDITOR
		public static VectorMap LoadVectorMap(string fullPath) {
			if (!File.Exists(fullPath)) return null;

			FileStream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
			BinaryFormatter formatter = new BinaryFormatter();
			VectorMap map = (VectorMap) formatter.Deserialize(stream);
			stream.Close();

			return map;
		}
		#endif

		public static void SaveUser(User user) {
			DirectoryInfo dir = new DirectoryInfo(UserPath);
			if (!dir.Exists) dir.Create();

			if (user.Name == "") return;

			string fileName = user.Name + userExt;
			string path = Path.Combine(UserPath, fileName);

			FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, user);
			stream.Close();
		}

		public static User LoadUser(string name) {
				DirectoryInfo dir = new DirectoryInfo(UserPath);
				if (!dir.Exists) dir.Create();

				if (name == "") return null;

				if (!name.EndsWith(userExt)) name += userExt;
				string path = Path.Combine(UserPath, name);

				if (!File.Exists(path)) return null;

				FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
				BinaryFormatter formatter = new BinaryFormatter();
				User user = (User) formatter.Deserialize(stream);
				stream.Close();

				return user;
		}

		public static List<string> GetLoadableUserNames() {
			DirectoryInfo dir = new DirectoryInfo(UserPath);
			if (!dir.Exists) dir.Create();

			var files = dir.GetFiles("*" + userExt, SearchOption.TopDirectoryOnly);

			List<string> loadables = new List<string>();
			for (int i = 0; i < files.Length; i++) {
				loadables.Add(Path.GetFileNameWithoutExtension(files[i].Name));
			}

			return loadables;
		}

		public static void SaveCalendar(CalendarData data) {
			DirectoryInfo dir = new DirectoryInfo(CalendarDataPath);
			if (!dir.Exists) dir.Create();

			string file = "Calendar" + calendarExt;
			string path = Path.Combine(CalendarDataPath, file);

			FileStream stream = new FileStream(path, FileMode.Create, FileAccess.Write);
			BinaryFormatter formatter = new BinaryFormatter();
			formatter.Serialize(stream, data);
			stream.Close();
		}

		public static CalendarData LoadCalendar() {
			try {
				DirectoryInfo dir = new DirectoryInfo(CalendarDataPath);
				if (!dir.Exists) return null;

				string file = "Calendar" + calendarExt;
				string path = Path.Combine(CalendarDataPath, file);
				if (!File.Exists(path)) return null;

				FileStream stream = new FileStream(path, FileMode.Open, FileAccess.Read);
				BinaryFormatter formatter = new BinaryFormatter();
				CalendarData data = (CalendarData) formatter.Deserialize(stream);
				stream.Close();

				return data;
			}
			catch {
				return null;
			}
		}
	}
}