using UnityEngine;

namespace Tile {
	public class SourceTile : BaseTile {
		[SerializeField] private int waterAmt = 3;
		private RiverTile[] rivers;

		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);

			rivers = new RiverTile[waterAmt];
			SetColor(Color.cyan);
		}
		
		
	}
}