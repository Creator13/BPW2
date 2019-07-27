using Growable;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	[SerializeField] private Species[] species;
	public Species[] Species => species;
	
	private bool paused;
	public bool Paused {
		get => paused;
		set {
			// Set timescale to 0 to effectively pause growing coroutines
			Time.timeScale = value ? 0 : 1;
			
			// Show the pause menu
			UIController.Instance.ShowPauseMenu(value);
			
			// Update the field value for others to access
			paused = value;
		}
	}

	private void Awake() {
		Instance = this;
	}

	private void Update() {
		if (Input.GetKeyDown("escape")) {
			// Toggle pause value when user presses escape key
			Paused = !Paused;
		}
	}
}