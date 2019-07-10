using UI;
using UnityEngine;

namespace Tile {
	public class BaseTile : MonoBehaviour {
		private int gridPosX, gridPosZ = -1;
		public int X => gridPosX;
		public int Z => gridPosZ;
		public TileGrid Grid { get; protected set; }
	
		public virtual void Initialize(int x, int z, TileGrid grid) {
			OnHoverExit();
			
			// Save this tile's x and z index in the Grid and save reference to parent Grid
			gridPosX = x;
			gridPosZ = z;
			Grid = grid;
			
			// Set initial position (relative to parent Grid).
			UpdatePosition();
		}

		public virtual void OnClick() {
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

		public void UpdatePosition() {
			// Position is determined by this tile's x and z index in the Grid, multiplied by half the tilesize so it
			// ends up in between four corner points. YOffset is the offset on the y axis relative to the parent transform
			transform.localPosition = new Vector3((gridPosX + .5f) * Grid.TileSize, Grid.YOffset, (gridPosZ +.5f) * Grid.TileSize);
		}

		public void OnHoverEnter(float hoverStrength) {
			// Apply hover effect
			SetHover(hoverStrength);
		}

		public void OnHoverExit() {
			// Remove hover effect
			SetHover(0);
		}

		private void SetHover(float hoverAmt) {
			// Change color using matPropBlock
			Renderer rend = GetComponent<Renderer>();
			MaterialPropertyBlock matPropBlock = new MaterialPropertyBlock();
		
			rend.GetPropertyBlock(matPropBlock);
			// Use hover shader with property _HoverStrength to set hover amount
			matPropBlock.SetFloat("_HoverStrength", hoverAmt);
			rend.SetPropertyBlock(matPropBlock);
		}
	
		public void SetColor(Color c) {
			// Change color using matPropBlock
			Renderer rend = GetComponent<Renderer>();
			MaterialPropertyBlock matPropBlock = new MaterialPropertyBlock();
		
			rend.GetPropertyBlock(matPropBlock);
			// _BaseColor is the HDRP lit shader's albedo color property
			matPropBlock.SetColor("_BaseColor", c);
			rend.SetPropertyBlock(matPropBlock);
		}

		public bool SurroundedByType<T>() {
			return Grid.TileHasSurroundingType<T>(X, Z);
		}
	}
} 