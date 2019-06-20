using UnityEngine;

namespace Tile {
	public class BaseTile : MonoBehaviour {
		private int gridPosX, gridPosZ = -1;
		private TileGrid grid;
	
		public void Initialize(int x, int z, TileGrid grid) {
			OnHoverExit();
			
			// Save this tile's x and z index in the grid and save reference to parent grid
			gridPosX = x;
			gridPosZ = z;
			this.grid = grid;
			
			// Set initial position (relative to parent grid).
			UpdatePosition();
		}

		public void UpdatePosition() {
			// Position is determined by this tile's x and z index in the grid, multiplied by half the tilesize so it
			// ends up in between four corner points. YOffset is the offset on the y axis relative to the parent transform
			transform.localPosition = new Vector3((gridPosX + .5f) * grid.TileSize, grid.YOffset, (gridPosZ +.5f) * grid.TileSize);
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
	}
} 