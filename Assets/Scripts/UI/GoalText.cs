using Growable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class GoalText : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI amountText;
		[SerializeField] private Image speciesIcon;

		private ExportGoal goal;
		public ExportGoal Goal {
			set {
				goal = value;
				
				// Set the text and the icon
				UpdateValue();
				speciesIcon.sprite = value.species.Icon;
			}
			private get => goal;
		}

		public void UpdateValue() {
			amountText.text = Goal.Current + "/" + Goal.Goal;

			if (Goal.IsReached) {
				// If the goal is reached, add strikethrough effect (if it wasn't already)
				if (! amountText.text.Contains("<s>")) {
					amountText.text = "<s>" + amountText.text + "</s>";
				}
			}
		}
	}
}