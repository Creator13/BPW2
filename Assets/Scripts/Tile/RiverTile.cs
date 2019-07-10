using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tile {
	public class RiverTile : BaseTile {
		private enum RiverShapes {
			End,
			Straight,
			Bend,
			Split3,
			Split4
		}

		private enum RiverDirections {
			North = 0,
			East = 90,
			South = 180,
			West = 270
		}

		[SerializeField] private Mesh straight;
		[SerializeField] private Mesh end;
		[SerializeField] private Mesh bend;
		[SerializeField] private Mesh split3;
		[SerializeField] private Mesh split4;

		private MeshFilter mf;
		
		private RiverDirections dir;
		private RiverShapes shape;
		
		private void OnEnable() {
			mf = GetComponent<MeshFilter>();
		}

		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);

			UpdateTileDirection();
			UpdateSurroundingTileDirection();
		}

		private void UpdateSurroundingTileDirection() {
			List<RiverTile> tiles = new List<RiverTile>(Grid.GetSurroundingTiles<RiverTile>(this));
			foreach (RiverTile tile in tiles) {
				tile.UpdateTileDirection();
			}
		}

		private void UpdateTileDirection() {
			List<BaseTile> tiles = new List<BaseTile>(Grid.GetSurroundingTiles(this));
			List<BaseTile> waters = new List<BaseTile>(tiles.Where(t => t.GetType() == typeof(RiverTile) || t.GetType() == typeof(SourceTile)));
			int surroundingRivers = waters.Count;

			if (surroundingRivers == 1) {
				// With a single surrounding tile, this tile will always be the end of a river branch
				shape = RiverShapes.End;

				// Tile points west when this x is smaller than the other x
				if (X < waters[0].X) {
					dir = RiverDirections.West;
				}
				// Tile points east when this x is larger than the other x
				else if (X > waters[0].X) {
					dir = RiverDirections.East;
				}
				// Tile points south when this z is smaller than the other z
				else if (Z < waters[0].Z) {
					dir = RiverDirections.South;
				}
				// Tile points north when this z is larger than the other z
				else if (Z > waters[0].Z) {
					dir = RiverDirections.North;
				}
			}
			else if (surroundingRivers == 2) {
				// If the two surrounding tiles are both on the same X coordinate (row), river is straight and runs east/west
				if (waters[0].X == waters[1].X) {
					shape = RiverShapes.Straight;
					dir = RiverDirections.North;
				}
				// If the two surrounding tiles are both on the same Z coordinate (col), river is straight and runs north/south
				else if (waters[0].Z == waters[1].Z) {
					shape = RiverShapes.Straight;
					dir = RiverDirections.East;
				}
				else {
					// The river is a bend
					shape = RiverShapes.Bend;

					// Find direction
					if (waters[0].X < X && waters[1].Z > Z || waters[1].X < X && waters[0].Z > Z) {
						dir = RiverDirections.South;
					}
					else if (waters[0].X < X && waters[1].Z < Z || waters[1].X < X && waters[0].Z < Z) {
						dir = RiverDirections.East;
					}
					else if (waters[0].X > X && waters[1].Z > Z || waters[1].X > X && waters[0].Z > Z) {
						dir = RiverDirections.West;
					}
					else if (waters[0].X > X && waters[1].Z < Z || waters[1].X > X && waters[0].Z < Z) {
						dir = RiverDirections.North;
					}
				}
			}
			else if (surroundingRivers == 3) {
				shape = RiverShapes.Split3;

				// Find out which two coords have either the same X or Z
				List<BaseTile> sameX = waters.FindAll(t => t.X == X);
				List<BaseTile> sameZ = waters.FindAll(t => t.Z == Z);

				if (sameX.Count == 2) {
					// Splitting is oriented west/east/along x axis
					if (sameZ[0].X > X) {
						// Single points north if other tile has a larger Z
						dir = RiverDirections.West;
					}
					else if (sameZ[0].X < X) {
						// Single points south if other tile has a smaller Z
						dir = RiverDirections.East;
					}
				}
				else if (sameZ.Count == 2) {
					// Splitting is oriented north/south/along z axis
					if (sameX[0].Z > Z) {
						// Single points east if other tile has a larger X
						dir = RiverDirections.South;
					}
					else if (sameX[0].Z < Z) {
						// Single points west if other tile has a smaller X
						dir = RiverDirections.North;
					}
				}
			}
			else if (surroundingRivers == 4) {
				shape = RiverShapes.Split4;
				// No direction needed for four-way splitting, default to north
				dir = RiverDirections.North;
			}

			UpdateMesh();
		}

		private void UpdateMesh() {
			Mesh mesh = null;
			try {
				switch (shape) {
					case RiverShapes.Straight:
						mesh = straight;
						break;
					case RiverShapes.End:
						mesh = end;
						break;
					case RiverShapes.Bend:
						mesh = bend;
						break;
					case RiverShapes.Split3:
						mesh = split3;
						break;
					case RiverShapes.Split4:
						mesh = split4;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (IndexOutOfRangeException e) {
				Debug.Log("Could not find mesh");
			}

			mf.mesh = mesh;
			GetComponent<MeshCollider>().sharedMesh = mesh;
			
			transform.localRotation = Quaternion.Euler(0, (float) dir, 0);
		}

		public override void OnClick() {
//			UpdateDirections();
		}
	}
}