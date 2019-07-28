using System.Collections.Generic;
using System.Linq;
using Growable;
using Tile;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	[SerializeField] private TileGrid grid;

	[Space(10)] [SerializeField] private string nextLevel;
	public string NextLevel => nextLevel;

	[SerializeField] private string mainMenuScene;
	public string MainMenuScene => mainMenuScene;

	[SerializeField] private bool isLastLevel;

	[Space(10)]
	[Range(0, 1), SerializeField]
	private float seedYieldChance = .2f;

	[Space(10)] [SerializeField] private List<ExportGoal> goals;
	public List<ExportGoal> Goals => goals;
	public Species[] Species {
		get {
			// Add all the species in the list of goals but only once.
			HashSet<Species> s = new HashSet<Species>();
			goals.ForEach(goal => s.Add(goal.Species));

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

	private bool won;
	public bool Won {
		get => won;
		set {
			// Set time to 0 to pause any possible running coroutines
			Time.timeScale = value ? 0 : 1;

			// Show the win screen
			UIController.Instance.ShowWinScreen(isLastLevel);

			// Save the value for reference
			won = value;
		}
	}

	private bool lost;
	public bool Lost {
		get => lost;
		set {
			// Set time to 0 to pause any possible running coroutines
			Time.timeScale = value ? 0 : 1;

			// Show the win screen
			UIController.Instance.ShowGameOverScreen();

			// Save the value for reference
			lost = value;
		}
	}

	// Check if there are still any elements in the goals list where the goal is not reached
	private bool AllGoalsReached => !Goals.Any(goal => !goal.IsReached);

	// Check if there are still any elements in the goals list where the number of seeds is higher than 0, then there
	// are seeds left
	private bool HasSeedsLeft => goals.Any(goal => goal.Seeds > 0);

	private void Awake() {
		// When the game is won, the global timeScale is set to 0. So change it back to 1 once a scene loads.
		Time.timeScale = 1;

		// Initialize singleton
		Instance = this;
	}

	private void Update() {
		if (!Won && Input.GetKeyDown("escape")) {
			// Toggle pause value when user presses escape key
			Paused = !Paused;
		}

		// Do not continue execution beyond here if the game is paused (saves processing power)
		if (Paused) return;

		// Check if the goals are reached or if the player is out of seeds
		WatchGoalsReached();
	}

	private void WatchGoalsReached() {
		if (AllGoalsReached) {
			// Player wins the game if all goals have been reached
			Won = true;
		}
		else if (!HasSeedsLeft) {
			// Player has lost when he's out of seeds and the map can't yield enough anymore
			if (!GridHasEnoughYield()) Lost = true;
		}
	}

	private bool GridHasEnoughYield() {
		List<GrowableTile> growables = new List<GrowableTile>(grid.GetTiles<GrowableTile>());

		foreach (ExportGoal goal in goals) {
			// Add together the expected yield values for all tiles with this species planted
			int availableYield = growables.Where(tile => tile.Species == goal.Species).Sum(tile => tile.ExpectedYield);

			// Calculate the yield needed to reach this goal
			int yieldToGoal = goal.Goal - goal.Current;

			// If this goal can't be reached, the level can no longer be won so return false
			if (yieldToGoal > availableYield) {
				return false;
			}
		}

		return true;
	}

	public void RegisterHarvest(Species s, int amt) {
		// Count the harvested species towards the goal in the list of goals.
		Goals.Find(goal => goal.Species == s).Current += amt;

		YieldSeeds(s);
	}

	public void YieldSeeds(Species s) {
		// TODO Make this species dependent
		// Give the player seeds at random, based on a chance setting
		if (Random.value <= seedYieldChance) {
			Goals.Find(goal => goal.Species == s).Seeds += 1;
		}
	}

	public bool HasSeed(Species s) {
		// Find if the goals list has a goal for the species s with seeds available
		return goals.Any(goal => goal.Species == s && goal.Seeds > 0);
	}

	public bool UseSeed(Species s) {
		bool hasSeed = HasSeed(s);

		if (hasSeed) {
			goals.Find(goal => goal.Species == s).Seeds--;
		}

		return hasSeed;
	}

	public void Exit() {
		Application.Quit();
	}
}