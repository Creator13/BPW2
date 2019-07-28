using System;
using System.Collections;
using System.Collections.Generic;
using Growable;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Tile {
	[RequireComponent(typeof(MeshRenderer)), RequireComponent(typeof(MeshFilter))]
	public class GrowableTile : GroundTile {
		public enum GrowableState {
			Tilled,
			Seeded,
			Growing,
			FullGrown,
			Dead,
			Harvested
		}

		[SerializeField] private Texture tilledGround;
		[SerializeField] private Texture seededGround;
		[SerializeField] private Texture harvestedGround;

		[SerializeField] private Transform spawnPoint;

		// Species may be null when in state Tilled or Harvested
		[SerializeField] private Species species;
		public Species Species => species;

		// TODO implement different yield values
		public int ExpectedYield {
			get {
				// Return 0 if the tile is not either growing, seeded, or full grown
				if (State == GrowableState.Seeded ||
				    State == GrowableState.Growing ||
				    State == GrowableState.FullGrown) {
					return species.MinYield;
				}

				return 0;
			}
		}

		[SerializeField] private GrowableState state;
		public GrowableState State {
			get => state;
			private set {
				if (value == GrowableState.Tilled) {
					RemoveExistingChildren();

					SetGroundTexture(tilledGround);
				}
				else if (value == GrowableState.Seeded) {
					SetGroundTexture(seededGround);
				}
				else if (value == GrowableState.Harvested) {
					// Remove stages
					RemoveExistingChildren();

					// If the species has a defined model for being harvested, set it
					if (species.HarvestedStage) {
						SetChildObject(species.HarvestedStage);
					}
					else {
						SetGroundTexture(harvestedGround);
					}
				}
				else if (value == GrowableState.Dead) {
					// Replace current stage model with dead mode
					RemoveExistingChildren();

					if (species.DeadStage) {
						SetChildObject(species.DeadStage);
					}
					else {
						// TODO add a default texture for dead species
						SetGroundTexture(harvestedGround);
					}
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
			string title = null;

			// Show the convert to river button when in tilled, harvested or dead states
			if (State == GrowableState.Tilled || State == GrowableState.Harvested || State == GrowableState.Dead) {
				Button b = UIController.Instance.GetButton("RiverButton");
				b.interactable = true;

				buttons.Add(new TileActionButton(b, ConvertToRiver));
			}

			if (State == GrowableState.Tilled) {
				// Show species selection buttons
				foreach (Species s in GameManager.Instance.Species) {
					// Button is interactable if there are seeds available for this species
					s.Button.interactable = GameManager.Instance.HasSeed(s);

					buttons.Add(new TileActionButton(s.Button, () => SetSpieces(s)));
				}

				title = "Plant species...";
			}
			else if (State == GrowableState.FullGrown || State == GrowableState.Growing) {
				// Show harvest button
				Button b = UIController.Instance.GetButton("HarvestButton");
				// Only interactable if fullgrown
				b.interactable = State == GrowableState.FullGrown;

				buttons.Add(new TileActionButton(b, Harvest));

				title = State == GrowableState.FullGrown ? "Harvest!" : "Growing...";
			}
			else if (State == GrowableState.Harvested || State == GrowableState.Dead) {
				// Show retill/convert back to ground tile
				Button b = UIController.Instance.GetButton("HoeButton");
				b.interactable = true;

				buttons.Add(new TileActionButton(b, ConvertToGrowable));

				title = "Unusable terrain";
			}
			else {
				return;
			}

			UIController.Instance.ShowDialog(this, title, buttons.ToArray());
		}

		private void SetSpieces(Species species) {
			// Use one seed to plant on this tile (and throw a little safeguard exception to avoid this function from
			// being used in a wrong manner)
			if (!GameManager.Instance.UseSeed(species))
				throw new InvalidOperationException("Shouldn't be setting a species when it has no seeds");

			this.species = species;

			// Reset growth stage to -1, set the current state and set tile mesh accordingly
			growthStage = -1;
			State = GrowableState.Seeded;

			StartCoroutine(WaitForGrowth());
		}

		private IEnumerator WaitForGrowth() {
			yield return new WaitForSeconds(species.GerminationTime);

			// Germination chance: multiply base chance by wetness. Increase wetness by taking square root and add base
			// value (always higher than .25). Random value should be within the germination chance value
			float chance = species.GerminationChance * Mathf.Sqrt(Mathf.Clamp(wetness + .25f, 0, 1));
			Debug.Log("Germinating... chance: " + chance + " wetness: " + wetness);

			if (Random.value <= chance) {
				StartCoroutine(GrowingLoop());
			}
			else {
				State = GrowableState.Dead;
			}
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
				yield return new WaitForSeconds(time);
			}
		}

		private void Harvest() {
			// Only allow harvesting when state is fullgrown
			// TODO allow harvesting for less yield while growing
			if (State != GrowableState.FullGrown)
				throw new InvalidOperationException("Cannot harvest while the crop is not fully grown");

			GameManager.Instance.RegisterHarvest(species, ExpectedYield);

			State = GrowableState.Harvested;
		}

		public override void OnHoverEnter(float hoverStrength) {
			base.OnHoverEnter(hoverStrength);

			// Also apply hover on child objects of the growable spawn point
			foreach (Transform t in spawnPoint.GetComponentsInChildren<Transform>()) {
				// Exclude parent object
				if (t.gameObject != spawnPoint.gameObject) {
					// Set hover for each child object
					Renderer rend = t.GetComponent<Renderer>();
					if (rend) {
						SetHover(hoverStrength, rend);
					}
				}
			}
		}

		public override void OnHoverExit() {
			base.OnHoverExit();

			// Remove hover on spawnpoint child objects
			foreach (Transform t in spawnPoint.GetComponentsInChildren<Transform>()) {
				// Exclude parent object
				if (t.gameObject != spawnPoint.gameObject) {
					// Set hover for each child object
					Renderer rend = t.GetComponent<Renderer>();
					if (rend) {
						SetHover(0, rend);
					}
				}
			}
		}

		private void UpdateGrowthStage() {
			if (!mf) throw new InvalidOperationException("MeshFilter wasn't loaded");

			if (growthStage >= 0) {
				SetChildObject(species.GetStage(growthStage));
			}
		}

		private void SetChildObject(GameObject obj) {
			RemoveExistingChildren();

			// Instantiate the gameobject for the next growth stage
			Instantiate(obj, spawnPoint);
		}

		private void RemoveExistingChildren() {
			// Destroy all current stage gameobjects in the spawnpoint first, if there are any
			foreach (Transform t in spawnPoint.GetComponentsInChildren<Transform>()) {
				// GetCompnonentsInChildren will also return the components found in parents apparently, so check to
				// not destroy the spawnpoint
				if (t.gameObject != spawnPoint.gameObject) Destroy(t.gameObject);
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