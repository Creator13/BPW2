using System;
using System.Collections.Generic;
using Tile;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
	public class UIController : MonoBehaviour {
		public static UIController Instance { get; private set; }

		[SerializeField] private TileDialog dialogPrefab;
		[SerializeField] private TextMeshProUGUI waterText;
		[SerializeField] private TileGrid grid;

		[Serializable]
		private struct NameButton {
			public string name;
			public Button button;
		}
		
		[SerializeField] private List<NameButton> buttons;

		private Camera cam;
		private Canvas canvas;

		private TileDialog dialog;

		private void Awake() {
			Instance = this;
		}

		private void Start() {
			cam = Camera.main;
			canvas = GetComponent<Canvas>();
		}

		private void Update() {
			waterText.text = "Water: " + grid.Source.AvailableRivers;
		}

		public void ShowDialog(BaseTile obj, params TileActionButton[] buttons) {
			if (dialog) {
				HideDialog();
			}
			
			dialog = Instantiate(dialogPrefab, canvas.transform, false);

			if (obj) {
				dialog.ShowDialog(obj, buttons);
			}
		}

		public void HideDialog() {
			if (dialog) {
				Destroy(dialog.gameObject);
			}
		}

		public Button GetButton(string name) {
			return buttons.Find(b => b.name == name).button;
		}
	}
}