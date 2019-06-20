using UnityEngine;

namespace Tile {
	public class TileGrid : MonoBehaviour {
		[SerializeField] private BaseTile standardTilePrefab;
		[SerializeField] private SourceTile sourceTilePrefab;

		[SerializeField] private int gridSizeX = 5;
		[SerializeField] private int gridSizeZ = 5;
		private BaseTile[,] grid;

		[SerializeField] private float tileSize = 1;
		public float TileSize => tileSize;
		[SerializeField] private float yOffset = -0.5f;
		public float YOffset => yOffset;

		private void Start() {
			Generate();
		}

		public void Generate() {
			// Clear all children before generating new grid
			DestroyGrid();

			// Initialize grid array
			grid = new BaseTile[gridSizeX, gridSizeZ];

			for (int i, x = 0; x < gridSizeX; x++) {
				for (int z = 0; z < gridSizeZ; z++) {
					// Generate new tile at current grid point
					grid[x, z] = Instantiate(standardTilePrefab);
					// Make child of grid root
					grid[x, z].transform.parent = transform;
					
					// Initialize the object at it's correct position
					grid[x, z].Initialize(x, z, this);
					// Set the color (for testing in alternating pattern) TODO
					grid[x, z].SetColor((x * gridSizeZ + z) % 2 == 0 ? Color.red : Color.blue);
				}
			}
		}

		public void DestroyGrid() {
			if (grid == null) return;

			// Destroy all children that have a BaseTile component attached
			foreach (BaseTile tile in GetComponentsInChildren<BaseTile>()) {
				// Use DestroyImmediate in edit mode because you can't use destroy
				if (Application.isPlaying) {
					Destroy(tile.gameObject);
				}
				else {
					DestroyImmediate(tile.gameObject);
				}
			}
		}

		private void OnDrawGizmos() {
			// Draw grid of spheres at corner points of grid
			for (int x = 0; x < gridSizeX + 1; x++) {
				for (int z = 0; z < gridSizeZ + 1; z++) {
					Gizmos.color = Color.black;
					Gizmos.DrawSphere(GetRelativeCoords(new Vector3(x, 0, z)), 0.05f);
				}
			}
		}

		public Vector3 GetRelativeCoords(Vector3 pos) {
			return pos + transform.position;
		}
	}
}