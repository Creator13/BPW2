using System.Collections.Generic;
using Tile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class TileDialog : MonoBehaviour {
		[SerializeField] private TextMeshProUGUI title;
		[SerializeField] private RectTransform buttonPanel;
		
		private BaseTile tile;

		private List<Button> buttons;

		private void UpdateScreenPos() {
			if (tile) {
				Vector3 boxPos = Camera.main.WorldToScreenPoint(tile.transform.position);
				transform.position = boxPos + new Vector3(0, 15, 0);
			}
		}

		public void ShowDialog(BaseTile tile, IEnumerable<TileActionButton> buttons) {
			this.tile = tile;
			
			UpdateScreenPos();
			
			foreach (TileActionButton tb in buttons) {
				Button button = Instantiate(tb.Button, buttonPanel, false);
				
				button.onClick.AddListener(tb.Action);
				button.onClick.AddListener(ClosePanel);
			}
		}

		private void ClosePanel() {
			UIController.Instance.HideDialog();
		}
	}
}