using Growable;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;

	[SerializeField] private Species[] species;
	public Species[] Species => species;
	
	private bool paused;
	public bool Paused {
		get => paused;
		set {
			// More code here
			paused = value;
		}
	}

	private void Awake() {
		Instance = this;
	} 
}