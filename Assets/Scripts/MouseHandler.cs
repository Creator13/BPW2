using Tile;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class MouseHandler : MonoBehaviour {
	private Camera cam;
	[SerializeField] private UIController ui;

	[SerializeField, Range(0, 1)] private float hoverStrength = 0.7f;

	private BaseTile currentHover;
	private BaseTile CurrentHover {
		set {
			// Remove hover effect from previous object
			if (currentHover) currentHover.OnHoverExit();
			// Set new current object
			currentHover = value;
			// Apply hover effect to new object (if set to null, no object will get a hover effect)
			if (currentHover) currentHover.OnHoverEnter(hoverStrength);
		}
	}

	private void Awake() {
		cam = GetComponent<Camera>();
	}

	private void Update() {
		CheckClick();
		
		// Check for hover every frame
		CheckHover();
	}

	private void CheckHover() {
		// Raycast from mouse position
		if (!EventSystem.current.IsPointerOverGameObject()) {
			if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit hoverHitInfo)) {
				BaseTile hover = hoverHitInfo.collider.GetComponent<BaseTile>();

				// Set the current hover object to the object that was hit (if not tile was hit, the value will be null)
				CurrentHover = hover;
			}
			else CurrentHover = null;
		}
		// Set hover to null if raycast hit nothing
		else CurrentHover = null;
	}

	private void CheckClick() {
		// Check if player clicked
		if (Input.GetMouseButtonDown(0)) {
			// Raycast from mouse position
			if (!EventSystem.current.IsPointerOverGameObject()) {
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit clickHitInfo)) {
					BaseTile clicked = clickHitInfo.collider.GetComponent<BaseTile>();

					// Check if the object that was hit was a tile, if so handle click
					if (clicked) {
						ui.ShowDialog(clicked.gameObject);
						HandleClick(clicked);
					}
					else ui.HideDialog();
				}
				else ui.HideDialog();
			}
		}
	}

	private void HandleClick(BaseTile clicked) {
		clicked.OnClick();
	}
}