using UnityEngine;

public class TileGrid : MonoBehaviour {
	[SerializeField] private BaseTile tilePrefab;

	[SerializeField] private int gridSizeX = 5;
	[SerializeField] private int gridSizeZ = 5;
	private BaseTile[,] grid;

	[SerializeField] private float tileSize = 1;

	private void Start() {
		Generate();
	}

	public void Generate() {
		DestroyGrid();

		grid = new BaseTile[gridSizeX, gridSizeZ];

		for (int x = 0; x < gridSizeX; x++) {
			for (int z = 0; z < gridSizeZ; z++) {
				grid[x, z] = Instantiate(tilePrefab);
				grid[x, z].transform.parent = transform;

				grid[x, z].SetPosition(x, z, tileSize);
				grid[x, z].SetColor((x * gridSizeZ + z) % 2 == 0);
			}
		}
	}

	public void DestroyGrid(bool editor = false) {
		if (grid == null) return;

		foreach (BaseTile tile in grid) {
			if (tile) {
				if (Application.isPlaying) {
					Debug.Log("safe destroy");
					Destroy(tile.gameObject);
				}
				else {
					Debug.Log("edit mode destroy");
					DestroyImmediate(tile.gameObject);
				}
			}
		}
	}
}