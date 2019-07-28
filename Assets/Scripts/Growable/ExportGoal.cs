using System;
using UnityEngine;

namespace Growable {
	[Serializable]
	public class ExportGoal {
		[SerializeField] private Species species;
		public Species Species => species;
		[SerializeField] private int goal;
		public int Goal => goal;
		[SerializeField] private int seeds;
		public int Seeds {
			get => seeds;
			// Clamp seeds to a minimum of 0
			set => seeds = value < 0 ? 0 : value;
		}

		private int current;
		public int Current {
			set {
				// Current value can currently be only between 0 and the goal value
				if (!IsReached) current = Mathf.Clamp(value, 0, goal);
			}
			get => current;
		}

		// The goal is reached if the current value is equal (or theoretically larger than) to the goal
		public bool IsReached => Current >= Goal;
	}
}