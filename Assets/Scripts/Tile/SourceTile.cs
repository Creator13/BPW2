using System;
using System.Collections.Generic;
using System.Linq;
using UI;
using UnityEngine;

namespace Tile {
	[RequireComponent(typeof(MeshFilter))]
	[RequireComponent(typeof(MeshCollider))]
	public class SourceTile : BaseTile {
		private enum SourceShapes {
			None,
			Out1,
			Out2Straight,
			Out2Bend,
			Out3,
			Out4
		}

		[SerializeField] private int waterAmt = 3;

		private List<RiverTile> rivers;

		// The number of available rivers for this source tile is the maximum minus the number of rivers currently connected
		public int AvailableRivers => waterAmt - rivers.Count;

		private SourceShapes shape;
		private RiverTile.RiverDirections dir;
		private MeshFilter mf;

		[SerializeField] private Mesh none;
		[SerializeField] private Mesh out1;
		[SerializeField] private Mesh out2Straight;
		[SerializeField] private Mesh out2Bend;
		[SerializeField] private Mesh out3;
		[SerializeField] private Mesh out4;

		private void OnEnable() {
			mf = GetComponent<MeshFilter>();
		}

		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);

			// Initialize rivers list
			rivers = new List<RiverTile>();
			
			SetColor(Grid.WetColor, "Ground");
		}

		public void AddRiver(RiverTile river) {
			rivers.Add(river);
		}

		public override void OnClick() {
			// TODO upgrade source tile\
			// for now simply hide the dialog
			UIController.Instance.HideDialog();
		}
		
		public void UpdateTileDirection() {
			List<BaseTile> tiles = new List<BaseTile>(Grid.GetSurroundingTiles(this));
			// Get all the water tiles (all the tiles a river can connect to)
			List<BaseTile> waters = new List<BaseTile>(tiles.Where(t => 
				t.GetType() == typeof(RiverTile) || t.GetType() == typeof(SourceTile)
			));
			int surroundingRivers = waters.Count;

			if (surroundingRivers == 0) {
				shape = SourceShapes.None;
				// No direction needed for four-way splitting, default to north
				dir = RiverTile.RiverDirections.North;
			}
			else if (surroundingRivers == 1) {
				// With a single surrounding tile, this tile will always be the end of a river branch
				shape = SourceShapes.Out1;

				// Tile points west when this x is smaller than the other x
				if (X < waters[0].X) {
					dir = RiverTile.RiverDirections.West;
				}
				// Tile points east when this x is larger than the other x
				else if (X > waters[0].X) {
					dir = RiverTile.RiverDirections.East;
				}
				// Tile points south when this z is smaller than the other z
				else if (Z < waters[0].Z) {
					dir = RiverTile.RiverDirections.South;
				}
				// Tile points north when this z is larger than the other z
				else if (Z > waters[0].Z) {
					dir = RiverTile.RiverDirections.North;
				}
			}
			else if (surroundingRivers == 2) {
				// If the two surrounding tiles are both on the same X coordinate (row), river is straight and runs east/west
				if (waters[0].X == waters[1].X) {
					shape = SourceShapes.Out2Straight;
					dir = RiverTile.RiverDirections.North;
				}
				// If the two surrounding tiles are both on the same Z coordinate (col), river is straight and runs north/south
				else if (waters[0].Z == waters[1].Z) {
					shape = SourceShapes.Out3;
					dir = RiverTile.RiverDirections.East;
				}
				else {
					// The river is a bend
					shape = SourceShapes.Out2Bend;

					// Find direction
					// Bend direction is arbitrary. Current model is oriented/mapped as follows:
					// 0deg/North:   east and south
					// 90deg/East:   west and south
					// 180deg/South: west and north
					// 270deg/West:  east and north
					if (waters[0].X < X && waters[1].Z > Z || waters[1].X < X && waters[0].Z > Z) {
						dir = RiverTile.RiverDirections.South;
					}
					else if (waters[0].X < X && waters[1].Z < Z || waters[1].X < X && waters[0].Z < Z) {
						dir = RiverTile.RiverDirections.East;
					}
					else if (waters[0].X > X && waters[1].Z > Z || waters[1].X > X && waters[0].Z > Z) {
						dir = RiverTile.RiverDirections.West;
					}
					else if (waters[0].X > X && waters[1].Z < Z || waters[1].X > X && waters[0].Z < Z) {
						dir = RiverTile.RiverDirections.North;
					}
				}
			}
			else if (surroundingRivers == 3) {
				shape = SourceShapes.Out3;

				// Find out which two coords have either the same X or the same Z
				List<BaseTile> sameX = waters.FindAll(t => t.X == X);
				List<BaseTile> sameZ = waters.FindAll(t => t.Z == Z);

				if (sameX.Count == 2) {
					// Splitting is oriented west/east/along x axis
					if (sameZ[0].X > X) {
						// Single points north if other tile has a larger Z
						dir = RiverTile.RiverDirections.West;
					}
					else if (sameZ[0].X < X) {
						// Single points south if other tile has a smaller Z
						dir = RiverTile.RiverDirections.East;
					}
				}
				else if (sameZ.Count == 2) {
					// Splitting is oriented north/south/along z axis
					if (sameX[0].Z > Z) {
						// Single points east if other tile has a larger X
						dir = RiverTile.RiverDirections.South;
					}
					else if (sameX[0].Z < Z) {
						// Single points west if other tile has a smaller X
						dir = RiverTile.RiverDirections.North;
					}
				}
			}
			else if (surroundingRivers == 4) {
				shape = SourceShapes.Out4;
				// No direction needed for four-way splitting, default to north
				dir = RiverTile.RiverDirections.North;
			}

			UpdateMesh();
		}

		private void UpdateMesh() {
			Mesh mesh = null;
			try {
				switch (shape) {
					case SourceShapes.None:
						mesh = none;
						break;
					case SourceShapes.Out1:
						mesh = out1;
						break;
					case SourceShapes.Out2Bend:
						mesh = out2Bend;
						break;
					case SourceShapes.Out2Straight:
						mesh = out2Straight;
						break;
					case SourceShapes.Out3:
						mesh = out3;
						break;
					case SourceShapes.Out4:
						mesh = out4;
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
			catch (IndexOutOfRangeException e) {
				Debug.Log("Could not find mesh");
			}

			// Change the mesh
			mf.mesh = mesh;
			GetComponent<MeshCollider>().sharedMesh = mesh;
			
			// Apply the rotation
			transform.localRotation = Quaternion.Euler(0, (float) dir, 0);
		}
	}
}