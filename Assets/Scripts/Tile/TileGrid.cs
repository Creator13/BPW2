using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tile {
	public class TileGrid : MonoBehaviour {
		[SerializeField] private BaseTile standardTilePrefab;
		[SerializeField] private SourceTile sourceTilePrefab;
		[SerializeField] private RiverTile riverTilePrefab;
		public RiverTile RiverTilePrefab => riverTilePrefab;

		[SerializeField] private int gridSizeX = 5;
		[SerializeField] private int gridSizeZ = 5;
		private BaseTile[,] grid;

		[SerializeField] private float tileSize = 1;
		public float TileSize => tileSize;
		[SerializeField] private float yOffset = -0.5f;
		public float YOffset => yOffset;

		public float Width => gridSizeX * tileSize;
		public float Height => gridSizeZ * tileSize;

		[SerializeField] private Camera mainCamera;
		
		public SourceTile Source { get; private set; }

		private void Start() {
			Generate();

			Vector3 currentCamPos = mainCamera.transform.position;
			mainCamera.transform.position = new Vector3((float) (transform.position.x + gridSizeX / 2.0 * tileSize), currentCamPos.y, currentCamPos.z);
		}

		public void Generate() {
			// Clear all children before generating new grid
			DestroyGrid();

			// Initialize grid array
			grid = new BaseTile[gridSizeX, gridSizeZ];

			// Generate the random index of the source tile
			// TODO not at edges/corners? Might fuck up balancing
			int sourceIndex = Random.Range(0, gridSizeX * gridSizeZ);

			for (int i = 0, x = 0; x < gridSizeX; x++) {
				for (int z = 0; z < gridSizeZ; z++, i++) {
					// Generate new tile at current grid point
					if (i == sourceIndex) {
						// Generate one source tile at the randomly generated index
						Source = (SourceTile) CreateTile(x, z, sourceTilePrefab);
					}
					else {
						// Generate standard tile
						CreateTile(x, z, standardTilePrefab);
					}
				}
			}
		}

		private BaseTile CreateTile(int x, int z, BaseTile prefab) {
			// Generate new tile at current grid point
			grid[x, z] = Instantiate(prefab);
			// Make tile child of grid root
			grid[x, z].transform.parent = transform;
			// Initialize the object at its correct position
			grid[x, z].Initialize(x, z, this);

			return grid[x, z];
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

		public IEnumerable<BaseTile> GetSurroundingTiles(int x, int z) {
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

		public IEnumerable<T> GetSurroundingTiles<T>(int x, int z) where T : BaseTile {
			IEnumerable<BaseTile> tiles = GetSurroundingTiles(x, z);

			List<T> specificTiles = new List<T>();
			specificTiles.AddRange(tiles.OfType<T>());
			
			return specificTiles.ToArray();
		}

		public IEnumerable<BaseTile> GetSurroundingTiles(BaseTile tile) {
			return GetSurroundingTiles(tile.X, tile.Z);
		}

		public IEnumerable<T> GetSurroundingTiles<T>(BaseTile tile) where T : BaseTile {
			return GetSurroundingTiles<T>(tile.X, tile.Z);
		}

		public bool TileHasSurroundingType<T>(int x, int z) {
			IEnumerable<BaseTile> tiles = GetSurroundingTiles(x, z);

			return tiles.Any(tile => tile.GetType() == typeof(T));
		}

		public BaseTile ReplaceTile(BaseTile tile, BaseTile newTile) {
			try {
				BaseTile bt = CreateTile(tile.X, tile.Z, newTile);
				
				Destroy(tile.gameObject);

				return bt;
			}
			catch (IndexOutOfRangeException e) {
				Debug.LogError("Tile to be replaced was out of range: x:" + newTile.X + " z: " + newTile.Z);
				throw;
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