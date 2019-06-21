using UnityEngine;

namespace Tile {
	public class RiverTile : BaseTile {
		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);
			
			SetColor(Color.yellow);
		}
	}
}