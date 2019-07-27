using System.Collections.Generic;
using System.Linq;
using Growable;
using UI;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

//	[SerializeField] private Species[] species;
	[SerializeField] private List<ExportGoal> goals;
	public List<ExportGoal> Goals => goals;
	public Species[] Species {
		get {
			// Add all the species in the list of goals but only once.
			HashSet<Species> s = new HashSet<Species>();
			goals.ForEach(goal => s.Add(goal.species));

			return s.ToArray();
		}
	}

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

	private bool AllGoalsReached {
		get {
			// Check if there are still any elements in the goals list where the goal is not reached
			return !Goals.Any(goal => !goal.IsReached);
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

		// Do not continue execution beyond here if the game is paused (saves processing power)
		if (Paused) return;
		
		// Check if the goals are reached
		WatchGoalsReached();
	}

	private void WatchGoalsReached() {
		if (AllGoalsReached) {
			// Player wins the game if all goals have been reached
		}
	}

	public void RegisterHarvest(Species s, int amt) {
		Goals.Find(goal => goal.species == s).Current += amt;
	}
}