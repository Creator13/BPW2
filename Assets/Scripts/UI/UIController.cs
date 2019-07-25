using System;
using System.Collections.Generic;
using Tile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	[RequireComponent(typeof(Canvas))]
	public class UIController : MonoBehaviour {
		public static UIController Instance { get; private set; }

		[SerializeField] private TileDialog dialogPrefab;
		[SerializeField] private TextMeshProUGUI waterText;
		[SerializeField] private TileGrid grid;

		[Serializable]
		private struct NameButton {
			// Binds a name to a standard unity button for lookup
			public string name;
			public Button button;
		}
		
		// 'Static' database of available buttons. TODO Might replace with scriptableObjects
		[SerializeField] private List<NameButton> buttons;

		private Camera cam;
		private Canvas canvas;

		private TileDialog dialog;

		private void Awake() {
			// Initialize singleton
			Instance = this;
		}

		private void Start() {
			// Load required components
			cam = Camera.main;
			canvas = GetComponent<Canvas>();
		}

		private void Update() {
			// Update water availability text
			waterText.text = "Water: " + grid.Source.AvailableRivers;
		}

		public void ShowDialog(BaseTile obj, params TileActionButton[] buttons) {
			// Show the tile popup dialog with the specified options on the specified tile
			// Hide the old dialog first, if it exists.
			if (dialog) {
				HideDialog();
			}
			
			// Create the new dialog as a child of the main canvas
			dialog = Instantiate(dialogPrefab, canvas.transform, false);

			// If the tile was set, build the dialog up with this tile and with the buttons provided as parameters
			if (obj) {
				dialog.ShowDialog(obj, buttons);
			}
		}

		public void HideDialog() {
			// HideDialog might get called when there is no dialog present, so check for existence.
			if (dialog) {
				// Destroy the old dialog
				Destroy(dialog.gameObject);
				// Reset reference to null
				dialog = null;
			}
		}

		public Button GetButton(string name) {
			// Lookup button in the database/button list and return the actual Unity button associated with it.
			return buttons.Find(b => b.name == name).button;
		}
	}
}