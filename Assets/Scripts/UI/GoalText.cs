using Growable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class GoalText : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI amountText;
		[SerializeField] private Image speciesIcon;
		[SerializeField] private TextMeshProUGUI seedText;

		private ExportGoal goal;
		public ExportGoal Goal {
			set {
				goal = value;
				
				// Set the text and the icon
				UpdateValue();
				speciesIcon.sprite = value.Species.Icon;
			}
			private get => goal;
		}

		public void UpdateValue() {
			if (Goal.IsReached) {
				// Update amount one last time
				amountText.text = Goal.Current + "/" + Goal.Goal;
				
				// If the goal is reached, add strikethrough effect (if it wasn't already)
				if (! amountText.text.Contains("<s>")) {
					amountText.text = "<s>" + amountText.text + "</s>";
				}
			}
			else {
				// Else keep updating the values
				amountText.text = Goal.Current + "/" + Goal.Goal;
				seedText.text = Goal.Seeds + " seeds";
			}
		}
	}
}