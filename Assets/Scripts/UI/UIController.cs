﻿using System;
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
		[SerializeField] private RectTransform pauseMenu;
		[SerializeField] private GoalPanel goalPanel;

		[Space(10)] [SerializeField] private TileGrid grid;

		[Serializable]
		private struct NameButton {
			// Binds a name to a standard unity button for lookup
			public string name;
			public Button button;
		}

		// 'Static' database of available buttons. TODO Might replace with scriptableObjects
		[Space(10)] [SerializeField] private List<NameButton> buttons;
		private Canvas canvas;

		private TileDialog dialog;

		private void Awake() {
			// Initialize singleton
			Instance = this;
		}

		private void Start() {
			// Load required components
			canvas = GetComponent<Canvas>();

			// Make pausemenu start out as disabled.
			pauseMenu.gameObject.SetActive(false);
			
			goalPanel.CreateList(GameManager.Instance.Goals);
		}

		private void Update() {
			// Check for pause to make game run slightly more efficient
			if (GameManager.Instance.Paused) return;

			// Update water availability text
			waterText.text = "Water left: " + grid.Source.AvailableRivers;
			
			goalPanel.UpdateList();
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

		public void ShowPauseMenu(bool show) {
			pauseMenu.gameObject.SetActive(show);
		}

		public Button GetButton(string name) {
			// Lookup button in the database/button list and return the actual Unity button associated with it.
			return buttons.Find(b => b.name == name).button;
		}
	}
}