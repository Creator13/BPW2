using UnityEngine;
using UnityEngine.UI;

namespace Growable {
	[CreateAssetMenu(menuName = "Growables/Generic Species", fileName = "New Species")]
	public class Species : ScriptableObject {

		[SerializeField] private string name;
		public string Name => name;
//		[SerializeField] private Sprite icon;
//		public Sprite Icon => icon;
		[SerializeField] private Button button;
		public Button Button => button;

		[SerializeField] private float germinationTime;
		public float GerminationTime => germinationTime;
		[SerializeField] private float growSpeed;
		public float GrowSpeed => growSpeed;
		[SerializeField] private GameObject[] growthStages;
		public int StageCount => growthStages.Length;

		public GameObject GetStage(int index) {
			return growthStages[index];
		}
	}
}