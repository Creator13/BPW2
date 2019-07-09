using Tile;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class TileDialog : MonoBehaviour {
		[SerializeField] private Text title;
		
		private BaseTile tile;
		
		public void SetTile(BaseTile tile) {
			title.text = tile.GetType().FullName;
		}
		
	}
}