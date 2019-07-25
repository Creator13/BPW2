using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Tile {
	public class SourceTile : BaseTile {
		[SerializeField] private int waterAmt = 3;

		private List<RiverTile> rivers;

		// The number of available rivers for this source tile is the maximum minus the number of rivers currently connected
		public int AvailableRivers => waterAmt - rivers.Count;

		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);

			// Initialize rivers list
			rivers = new List<RiverTile>();

			// TODO fix this with a model or something
			SetColor(Color.cyan);
		}

		public void AddRiver(RiverTile river) {
			rivers.Add(river);
		}

		public override void OnClick() {
			// TODO upgrade source tile\
			// for now simply hide the dialog
			UIController.Instance.HideDialog();
		}
	}
}