using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tile {
	public class TileGrid : MonoBehaviour {
		[SerializeField] private GroundTile groundTilePrefab;
		[SerializeField] private SourceTile sourceTilePrefab;
		[SerializeField] private RiverTile riverTilePrefab;
		public RiverTile RiverTilePrefab => riverTilePrefab;

		public Color dryColor = new Color(0.63f, 0.50f, 0.38f);
		public Color wetColor = new Color(0.37f, 0.26f, 0.17f);

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

		[SerializeField] private float waterSpreadTickTime;
		
		public SourceTile Source { get; private set; }

		private void Start() {
			// Create a newly generated grid when the scene loads.
			Generate();

			// Set camera position to center of grid.
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
						CreateTile(x, z, groundTilePrefab);
					}
				}
			}
			
			// After all tiles have been created, call their LateInitialize function to perform actions that require
			// surrounding tiles to exist
			foreach (BaseTile tile in grid) {
				tile.LateInitialize();
			}
		}

//		private IEnumerator TickWaterSpread() {
//			while (true) {
//				yield return GameManager.Instance.Paused ? null : new WaitForSeconds(waterSpreadTickTime);
//				if (GameManager.Instance.Paused) continue;
//
//				Debug.Log("tick");
//				
//				foreach (BaseTile tile in grid) {
//					if (tile is GroundTile gTile) {
//						gTile.TickWaterLevel();
//					}
//				}
//			}
//		}
		
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
			// Update the transform.positions of all the tiles in the grid
			if (grid == null) return;

			foreach (BaseTile tile in grid) {
				tile.UpdatePosition();
			}
		}

		private Vector3 GetRelativeCoords(Vector3 pos) {
			pos *= TileSize;
			return pos + transform.position;
		}

		private IEnumerable<BaseTile> GetSurroundingTiles(int x, int z) {
			List<BaseTile> tiles = new List<BaseTile>();

			// Add all the surrounding tiles
			// If statements are there to check if the tile in that direction exists.
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

			return tiles;
		}

		private IEnumerable<T> GetSurroundingTiles<T>(int x, int z) where T : BaseTile {
			IEnumerable<BaseTile> tiles = GetSurroundingTiles(x, z);

			// Retrieve all the tiles where the type is the same as T
			List<T> specificTiles = new List<T>();
			specificTiles.AddRange(tiles.OfType<T>());
			
			return specificTiles.ToArray();
		}

		public IEnumerable<BaseTile> GetSurroundingTiles(BaseTile tile) {
			// Shorthand that uses an existing tile to get the x and y.
			return GetSurroundingTiles(tile.X, tile.Z);
		}

		public IEnumerable<T> GetSurroundingTiles<T>(BaseTile tile) where T : BaseTile {
			// Shorthand that uses an existing tile to get the x and y.
			return GetSurroundingTiles<T>(tile.X, tile.Z);
		}

		public bool TileHasSurroundingTile<T>(int x, int z) {
			// Retrieve all the surrounding tiles
			IEnumerable<BaseTile> tiles = GetSurroundingTiles(x, z);

			// Check if any of the tiles matches the type parameter
			return tiles.Any(tile => tile.GetType() == typeof(T));
		}

		public BaseTile ReplaceTile(BaseTile tile, BaseTile newTile) {
			try {
				// Create the requested new tile at the place of the old tile.
				BaseTile bt = CreateTile(tile.X, tile.Z, newTile);
				
				// Destroy the old tile
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