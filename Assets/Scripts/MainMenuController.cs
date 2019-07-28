using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	[SerializeField] private string firstLevelName;
	
	public void StartGame() {
		SceneManager.LoadScene(firstLevelName);
	}

	public void ExitGame() {
		Application.Quit();
	}

}