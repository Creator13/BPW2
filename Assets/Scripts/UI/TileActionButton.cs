using UnityEngine.Events;
using UnityEngine.UI;

namespace UI {
	public struct TileActionButton {
		public Button Button { get; }
		public UnityAction Action { get; }

		public TileActionButton(Button button, UnityAction action) {
			Button = button;
			Action = action;
		}
	}
}