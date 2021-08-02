using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExerciseGroup : ScriptableObject {
	[SerializeField] private Exercise[] all;

	public Exercise GetRandom() {
		int r = Random.Range(0, all.Length);
		return all[r];
	}
}
