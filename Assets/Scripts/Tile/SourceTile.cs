using System.Collections.Generic;
using UnityEngine;

namespace Tile {
	public class SourceTile : BaseTile {
		[SerializeField] private int waterAmt = 3;

		private List<RiverTile> rivers;

		public int AvailableRivers => waterAmt - rivers.Count;

		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);

			rivers = new List<RiverTile>();

			SetColor(Color.cyan);
		}

		public void AddRiver(RiverTile river) {
			rivers.Add(river);
		}
	}
}