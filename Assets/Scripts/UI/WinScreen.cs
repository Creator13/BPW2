using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
	public class WinScreen : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI header;
		[SerializeField] private Button continueButton;
		[SerializeField] private Button exitButton;

		private void Start() {
			// Bind exit button to game exit (handled by GameManager)
			exitButton.onClick.AddListener(GameManager.Instance.Exit);
		}

		public void BindContinueToLevel(string sceneName) {
			// Bind loading the given index to the button, and make sure it's the only action bound to it
			continueButton.onClick.RemoveAllListeners();
			continueButton.onClick.AddListener(() => SceneManager.LoadScene(sceneName));
		}

		public void SetLastLevel() {
			// Set header header to game won instead of level won
			header.text = "Game won!";

			// Make the continue button point to the main menu instead
			continueButton.GetComponentInChildren<TextMeshProUGUI>().text = "Main menu";
			continueButton.onClick.RemoveAllListeners();
			continueButton.onClick.AddListener(() => 
				SceneManager.LoadScene(GameManager.Instance.MainMenuScene)
			);
		}
	}
}