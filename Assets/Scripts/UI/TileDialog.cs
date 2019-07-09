using Tile;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class TileDialog : MonoBehaviour {
		[SerializeField] private Text title;
		
		private BaseTile tile;

		private void Update() {
			if (tile) {
				Vector3 boxPos = Camera.main.WorldToScreenPoint(tile.transform.position);
				transform.position = boxPos + new Vector3(0, 15, 0);
			}
		}

		public void SetTile(BaseTile tile) {
			this.tile = tile;
//			title.text = tile.GetType().FullName;

		}
	}
}