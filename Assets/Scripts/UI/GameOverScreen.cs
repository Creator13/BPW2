using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI {
	public class GameOverScreen : MonoBehaviour {
		[SerializeField] private Button mainMenuButton;
		[SerializeField] private Button exitButton;

		private void Start() {
			exitButton.onClick.AddListener(GameManager.Instance.Exit);
			mainMenuButton.onClick.AddListener(() => SceneManager.LoadScene(GameManager.Instance.MainMenuScene));
		}
	}
}