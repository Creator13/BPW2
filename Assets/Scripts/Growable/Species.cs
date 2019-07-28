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
		[SerializeField] private Sprite seedIcon;
		public Sprite SeedIcon => seedIcon;

		[Space(10)] [SerializeField] private int minYield;
		public int MinYield => minYield;

		[Space(10), SerializeField] private float germinationTime;
		public float GerminationTime => germinationTime;
		[Range(0, 1)] [SerializeField] private float germinationChance;
		public float GerminationChance => germinationChance;
		[SerializeField] private float growSpeed;
		public float GrowSpeed => growSpeed;
		[Range(0, 1)] [SerializeField] private float growingChance;
		public float GrowingChance => growingChance;
		
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