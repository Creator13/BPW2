using System;
using System.Collections.Generic;
using Growable;
using UnityEngine;

namespace UI {
	public class GoalPanel : MonoBehaviour {
		[SerializeField] private RectTransform goalContainer;
		[SerializeField] private GoalText listPrefab;

		private readonly List<GoalText> entries = new List<GoalText>();
		
		public void CreateList(List<ExportGoal> goals) {
			goals.ForEach(goal => {
				// Create a new entry for each goal
				GoalText entry = Instantiate(listPrefab, goalContainer, false);
				entry.Goal = goal;
				
				// Add it to the list for future reference
				entries.Add(entry);
			});
		}

		public void UpdateList() {
			if (entries != null) {
				entries.ForEach(e => e.UpdateValue());
			}
			else {
				throw new InvalidOperationException("Couldn't update goals because the reference list was not set");
			}
		}
	}
}