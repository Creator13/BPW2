using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour {
	public static AudioController Instance;

	[SerializeField] private AudioClip dig;
	[SerializeField] private AudioClip harvest;
	[SerializeField] private AudioClip water;
	[SerializeField] private AudioClip ding;

	private new AudioSource audio;
	
	private void Awake() {
		// Initialize singleton
		Instance = this;

		audio = GetComponent<AudioSource>();
	}

	public void DiggingSound() {
		if (dig) audio.PlayOneShot(dig);
	}

	public void WaterSound() {
		if (water) audio.PlayOneShot(water);
	}

	public void HarvestSound() {
		if (harvest) audio.PlayOneShot(harvest);
	}

	public void DingSound() {
		if (ding) audio.PlayOneShot(ding);
	}
}