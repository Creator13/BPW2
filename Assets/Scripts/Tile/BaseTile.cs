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
			
//			SetColor(x % 2 == 0 && z % 2 == 0 || x % 2 == 1 && z % 2 == 1 ? Color.red : Color.blue);
		}

		public virtual void OnClick() {
			if ((SurroundedByType<RiverTile>() || SurroundedByType<SourceTile>())
			    && GetType() != typeof(SourceTile) && GetType() != typeof(RiverTile)) {
				TileGrid grid = Grid;

				if (grid.Source.AvailableRivers > 0) {
					RiverTile riverTile = grid.ReplaceTile(this, Grid.RiverTilePrefab) as RiverTile;
					grid.Source.AddRiver(riverTile);
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