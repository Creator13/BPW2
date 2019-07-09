using Tile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class UIController : MonoBehaviour {
		[SerializeField] private TileDialog dialogPrefab;
		[SerializeField] private TextMeshProUGUI waterText;
		[SerializeField] private TileGrid grid;

		private Camera cam;
		private Canvas canvas;

		private TileDialog dialog;

		private void Start() {
			cam = Camera.main;
			canvas = GetComponent<Canvas>();
		}

		private void Update() {
			waterText.text = "Water: " + grid.Source.AvailableRivers;
		}

		public void ShowDialog(GameObject obj) {

			if (!dialog) {
				dialog = Instantiate(dialogPrefab, canvas.transform, false);
			}

			if (!dialog.gameObject.activeSelf) {
				dialog.gameObject.SetActive(true);
			}

			BaseTile tile = obj.GetComponent<BaseTile>();
			if (tile) {
				dialog.SetTile(tile);
			}
		}

		public void HideDialog() {
			if (dialog) {
				Destroy(dialog.gameObject);
			}
		}
	}
}