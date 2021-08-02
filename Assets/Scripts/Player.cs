using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public static Player Instance { get; private set; }

	[SerializeField] private Animator animator;
	public Animator Animator { get => animator; }

	public static Player CreatePlayer() {
		Player prototype = Resources.Load<Player>("Prototypes/Player");
		Instance = Instantiate(prototype);
		DontDestroyOnLoad(Instance);
		return Instance;
	}
}
