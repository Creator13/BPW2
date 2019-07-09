using Tile;
using UnityEngine;

namespace UI {
	public class UIController : MonoBehaviour {
		[SerializeField] private TileDialog dialogPrefab;

		private Camera cam;
		private Canvas canvas;

		private TileDialog dialog;

		private void Start() {
			cam = Camera.main;
			canvas = GetComponent<Canvas>();
		}

		public void ShowDialog(GameObject obj) {
			Vector3 boxPos = cam.WorldToScreenPoint(obj.transform.position);

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

			dialog.transform.position = boxPos + new Vector3(0, 15, 0);
		}

		public void HideDialog() {
			if (dialog) {
				Destroy(dialog.gameObject);
			}
		}
	}
}