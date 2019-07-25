using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Tile {
	[RequireComponent(typeof(Renderer))]
	public class GroundTile : BaseTile {
		private int waterAvailability;
		private float wetness;
//		private float waterLevel;

//		[SerializeField] private float waterUpdateSpeed;
		[SerializeField] private float maxEfficiencyRiverCount = 3;
//		[SerializeField] private Color dry = new Color(0.63f, 0.50f, 0.38f);
//		[SerializeField] private Color wet = new Color(0.37f, 0.26f, 0.17f);

		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);

			UpdateWaterAvailability();

//		waterLevel = 0;
		}

		public override void LateInitialize() {
			UpdateWaterAvailability();
		}

		public void UpdateWaterAvailability() {
			List<BaseTile> surroundingWaters = new List<BaseTile>(Grid.GetSurroundingTiles(this));

			// Count the number of surrounding rivers
			waterAvailability = surroundingWaters.Count(t => {
				if (t != null) return t.GetType() == typeof(SourceTile) || t.GetType() == typeof(RiverTile);
				return false;
			});
			
			// Calculate the wetness of the tile, i.e. how saturated this tile is with water. This value depends on the
			// surrounding rivers and is maximal (1.0) when it is surrounded by the number of rivers maxEfficiencyRiverCount
			wetness = Mathf.Clamp01(waterAvailability / maxEfficiencyRiverCount);
			
			UpdateColor();
		}

		private void UpdateColor() {
			// Set the color lerped between dry and wet (meaning it will have the dry color when the wetness is 0 and
			// the wet color when it is 1, and something in between when the value is in between 0 and 1)
			SetColor(Color.Lerp(Grid.dryColor, Grid.wetColor, wetness), "Ground");
		}
		
//	public void TickWaterLevel() {
//		
//	}

		public override void OnClick() {
			// Todo add convert to growable tile (species select directly in this box is possible by reopening the dialog with new buttons)
			TileActionButton button = new TileActionButton(UIController.Instance.GetButton("RiverButton"), ConvertToRiver);
			if (Grid.Source.AvailableRivers <= 0 || !(SurroundedByType<RiverTile>() || SurroundedByType<SourceTile>())) {
				button.Button.interactable = false;
			}
			else button.Button.interactable = true;

			UIController.Instance.ShowDialog(this, button);
		}

		private void ConvertToRiver() {
			// Check if the tile is allowed to be converted to a river:
			// Needs to connect to either a source or a river and it shouldn't already be a source or a river
			if ((SurroundedByType<RiverTile>() || SurroundedByType<SourceTile>())
			    && GetType() != typeof(SourceTile) && GetType() != typeof(RiverTile)) {
				// Check if the source can have more rivers
				if (Grid.Source.AvailableRivers > 0) {
					// Build and replace a new river tile and register it to the source.
					RiverTile riverTile = Grid.ReplaceTile(this, Grid.RiverTilePrefab) as RiverTile;
					Grid.Source.AddRiver(riverTile);
				}
			}
		}
	}
}