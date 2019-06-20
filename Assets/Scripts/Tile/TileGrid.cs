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
					grid[x, z].SetColor(x % 2 == 0 && z % 2 == 0 || x % 2 == 1 && z % 2 == 1 ? Color.red : Color.blue);
				}
			}
		}

		public void DestroyGrid() {
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

		public void UpdateTilePositions() {
			if (grid == null) return;

			foreach (BaseTile tile in grid) {
				tile.UpdatePosition();
			}
		}

		public Vector3 GetRelativeCoords(Vector3 pos) {
			pos *= TileSize;
			return pos + transform.position;
		}
		
		private void OnDrawGizmos() {
			// Draw grid of spheres at corner points of grid
			for (int x = 0; x < gridSizeX + 1; x++) {
				for (int z = 0; z < gridSizeZ + 1; z++) {
					// Draw lines
					Gizmos.color = Color.gray;
					// Draw in both dir
					if (x < gridSizeX && z < gridSizeZ) {
						Gizmos.DrawLine(
							GetRelativeCoords(new Vector3(x, 0, z)),
							GetRelativeCoords(new Vector3(x + 1, 0, z))
						);
						Gizmos.DrawLine(
							GetRelativeCoords(new Vector3(x, 0, z)),
							GetRelativeCoords(new Vector3(x, 0, z + 1))
						);
					}
					// Draw only in x dir
					else if (x < gridSizeX && z >= gridSizeZ) {
						Gizmos.DrawLine(
							GetRelativeCoords(new Vector3(x, 0, z)),
							GetRelativeCoords(new Vector3(x + 1, 0, z))
						);
					}
					// Draw only in z dir
					else if (x >= gridSizeX && z < gridSizeZ) {
						Gizmos.DrawLine(
							GetRelativeCoords(new Vector3(x, 0, z)),
							GetRelativeCoords(new Vector3(x, 0, z + 1))
						);
					}

					// Draw spheres
					Gizmos.color = Color.black;
					Gizmos.DrawSphere(GetRelativeCoords(new Vector3(x, 0, z)), 0.05f);
				}
			}
		}
	}
}