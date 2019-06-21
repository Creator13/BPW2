using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

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

			// Generate the random index of the source tile
			int sourceIndex = Random.Range(0, gridSizeX * gridSizeZ);

			for (int i = 0, x = 0; x < gridSizeX; x++) {
				for (int z = 0; z < gridSizeZ; z++, i++) {
					// Generate new tile at current grid point
					CreateTile(x, z, i == sourceIndex ? sourceTilePrefab : standardTilePrefab);
				}
			}
		}

		private void CreateTile(int x, int z, BaseTile prefab) {
			// Generate new tile at current grid point
			grid[x, z] = Instantiate(prefab);
			// Make tile child of grid root
			grid[x, z].transform.parent = transform;
			// Initialize the object at its correct position
			grid[x, z].Initialize(x, z, this);
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

		private Vector3 GetRelativeCoords(Vector3 pos) {
			pos *= TileSize;
			return pos + transform.position;
		}

		public BaseTile[] GetSurroundingTiles(int x, int z) {
			List<BaseTile> tiles = new List<BaseTile>();

			if (x + 1 < gridSizeX) {
				tiles.Add(grid[x + 1, z]);
			}

			if (x - 1 >= 0) {
				tiles.Add(grid[x - 1, z]);
			}

			if (z + 1 < gridSizeX) {
				tiles.Add(grid[x, z + 1]);
			}

			if (z - 1 >= 0) {
				tiles.Add(grid[x, z - 1]);
			}

			return tiles.ToArray();
		}

		public bool TileHasSurroundingType<T>(int x, int z) {
			BaseTile[] tiles = GetSurroundingTiles(x, z);

			return tiles.Any(tile => tile.GetType() == typeof(T));

		}

		public void ReplaceTile(BaseTile tile, BaseTile newTile) {
			try {
				BaseTile oldTile = grid[tile.X, tile.Z];
//				oldTile.enabled = false;
				
				CreateTile(tile.X, tile.Z, newTile);
				
				Destroy(oldTile.gameObject);
			}
			catch (IndexOutOfRangeException e) {
				Debug.LogError("Tile to be replaced was out of range");
			}
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