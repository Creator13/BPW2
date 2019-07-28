using System;
using UnityEngine;

namespace Growable {
	[Serializable]
	public class ExportGoal {
		public Species species;
		[SerializeField] private int goal;
		public int Goal => goal;

		private int current;
		public int Current {
			set {
				// Current value can currently be only between 0 and the goal value
				if (! IsReached) current = Mathf.Clamp(value, 0, goal);
			}
			get => current;
		}

		// The goal is reached if the current value is equal (or theoretically larger than) to the goal
		public bool IsReached => Current >= Goal;
	}
}