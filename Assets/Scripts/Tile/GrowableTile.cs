using System;
using System.Collections;
using System.Collections.Generic;
using Growable;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Tile {
	[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
	public class GrowableTile : GroundTile {
		private enum GrowableState {
			Tilled,
			Seeded,
			Growing,
			FullGrown,
			Dead,
			Harvested
		}

		[SerializeField] private Texture tilledGround;
		[SerializeField] private Texture seededGround;

		[SerializeField] private Transform spawnPoint;

		// Species may be null when in state Tilled or Harvested
		[SerializeField] private Species species;

		[SerializeField] private GrowableState state;
		private GrowableState State {
			get { return state; }
			set {
				if (value == GrowableState.Tilled) {
					SetGroundTexture(tilledGround);
				}
				else if (value == GrowableState.Seeded) {
					SetGroundTexture(seededGround);
				}
				else {
					SetGroundTexture(null);
				}

				state = value;
			}
		}

		private int growthStage;

		private MeshRenderer rend;
		private MeshFilter mf;

		public override void Initialize(int x, int z, TileGrid grid) {
			base.Initialize(x, z, grid);

			// Make sure the tile starts out as tilled
			State = GrowableState.Tilled;

			// Load meshrenderer and meshfilter component
			rend = GetComponent<MeshRenderer>();
			mf = GetComponent<MeshFilter>();

			// Reset some state-dependent fields
			species = null;
			growthStage = -1;
		}

		public override void OnClick() {
			List<TileActionButton> buttons = new List<TileActionButton>();
			if (state == GrowableState.Tilled) {
				// Show species selection buttons
				foreach (Species s in GameManager.Instance.Species) {
					s.Button.interactable = true;

					buttons.Add(new TileActionButton(s.Button, () => SetSpieces(s)));
				}
			}
			else if (state == GrowableState.FullGrown) {
				// Show harvest button
				Button b = UIController.Instance.GetButton("HarvestButton");
				b.interactable = true;

				buttons.Add(new TileActionButton(b, Harvest));
			}
			else if (state == GrowableState.Harvested || state == GrowableState.Dead) {
				// Show retill/convert back to ground tile
				Button b = UIController.Instance.GetButton("HoeButton");
				b.interactable = true;

				buttons.Add(new TileActionButton(b, ConvertToGrowable));
			}
			else {
				return;
			}

			UIController.Instance.ShowDialog(this, buttons.ToArray());
		}

		private void SetSpieces(Species species) {
			this.species = species;

			// Reset growth stage to -1, set the current state and set tile mesh accordingly
			growthStage = -1;
			State = GrowableState.Seeded;

			StartCoroutine(WaitForGrowth());
		}

		private IEnumerator WaitForGrowth() {
			// TODO maybe add a random here in which a seedling fails
			yield return new WaitForSeconds(species.GerminationTime);

			StartCoroutine(GrowingLoop());
		}

		private IEnumerator GrowingLoop() {
			State = GrowableState.Growing;

			while (State == GrowableState.Growing) {
				growthStage++;
				UpdateGrowthStage();

				// TODO add a random possibility for the plant to die (but it is based on the tile wetness and the current growth stage)
				if (growthStage == species.StageCount - 1) {
					State = GrowableState.FullGrown;
				}

				// Time is defined by the growing speed of the species itself and the wetness of the tile.
				// The time will decrease to 55% of the original speed when at max wetness
				float time = species.GrowSpeed - (.45f * wetness * species.GrowSpeed);
				yield return new WaitForSeconds(species.GrowSpeed);
			}
		}

		private void Harvest() {
			Debug.Log("hrvest");
		}

		private void UpdateGrowthStage() {
			if (!mf) throw new InvalidOperationException("MeshFilter wasn't loaded");

			if (growthStage >= 0) {
				foreach (Transform t in spawnPoint.GetComponentsInChildren<Transform>()) {
					if (t.gameObject != spawnPoint.gameObject) Destroy(t.gameObject);
				}

				Instantiate(species.GetStage(growthStage), spawnPoint);
			}
		}

		private void SetGroundTexture(Texture tex) {
			const string matName = "Ground";

			// Change color using matPropBlock
			rend = GetComponent<MeshRenderer>();

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
			// Set texture
			matPropBlock.SetTexture("_Albedo", tex == null ? Texture2D.whiteTexture : tex);
			rend.SetPropertyBlock(matPropBlock, matIndex);
		}
	}
}