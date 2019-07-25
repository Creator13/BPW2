using UnityEngine;

namespace Tile {
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Renderer))]
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

		public virtual void LateInitialize() { }
		
		public virtual void OnClick() { }

		public void UpdatePosition() {
			// Position is determined by this tile's x and z index in the Grid, multiplied by half the tilesize so it
			// ends up in between four corner points. YOffset is the offset on the y axis relative to the parent transform
			transform.localPosition = new Vector3((gridPosX + .5f) * Grid.TileSize, Grid.YOffset, (gridPosZ + .5f) * Grid.TileSize);
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

			// Set each material individually to have the same parameters (creates support for editing materials separately)
			for (int i = 0; i < rend.materials.Length; i++) {
				rend.GetPropertyBlock(matPropBlock, i);
				// Use hover shader with property _HoverStrength to set hover amount
				matPropBlock.SetFloat("_HoverStrength", hoverAmt);
				rend.SetPropertyBlock(matPropBlock, i);
			}
		}

		public void SetColor(Color c) {
			// Change color using matPropBlock
			Renderer rend = GetComponent<Renderer>();
			MaterialPropertyBlock matPropBlock = new MaterialPropertyBlock();
			
			// Set each material individually to have the same parameters (creates support for editing materials separately)
			for (int i = 0; i < rend.materials.Length; i++) {
				rend.GetPropertyBlock(matPropBlock, i);
				// _BaseColor is the HDRP lit shader's albedo color property
				matPropBlock.SetColor("_BaseColor", c);
				rend.SetPropertyBlock(matPropBlock, i);
			}
		}
		
		public void SetColor(Color c, string matName) {
			// Change color using matPropBlock
			Renderer rend = GetComponent<Renderer>();

			// Find the index of the material with the provided name
			int matIndex = -1;
			for (int i = 0; i < rend.materials.Length; i++) {
				if (rend.materials[i].name == matName + " (Instance)") {
					matIndex = i;
					break;
				}
			}
			
			MaterialPropertyBlock matPropBlock = new MaterialPropertyBlock();
			rend.GetPropertyBlock(matPropBlock, matIndex);
			// _BaseColor is the HDRP lit shader's albedo color property
			matPropBlock.SetColor("_BaseColor", c);
			rend.SetPropertyBlock(matPropBlock, matIndex);
		}

		public bool SurroundedByType<T>() {
			return Grid.TileHasSurroundingTile<T>(X, Z);
		}
	}
}