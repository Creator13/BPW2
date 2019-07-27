using UnityEngine;
using UnityEngine.UI;

namespace Growable {
	[CreateAssetMenu(menuName = "Growables/Generic Species", fileName = "New Species")]
	public class Species : ScriptableObject {

		[SerializeField] private new string name;
		public string Name => name;
		[SerializeField] private Sprite icon;
		public Sprite Icon => icon;
		[SerializeField] private Button button;
		public Button Button => button;

		[Space(10), SerializeField] private float germinationTime;
		public float GerminationTime => germinationTime;
		[SerializeField] private float growSpeed;
		public float GrowSpeed => growSpeed;
		
		[Space(10), SerializeField] private GameObject[] growthStages;
		public int StageCount => growthStages.Length;
		[SerializeField] private GameObject deadStage;
		public GameObject DeadStage => deadStage;
		[SerializeField] private GameObject harvestedStage;
		public GameObject HarvestedStage => harvestedStage;

		public GameObject GetStage(int index) {
			return growthStages[index];
		}
	}
}