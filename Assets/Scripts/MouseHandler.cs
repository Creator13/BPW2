using Tile;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class MouseHandler : MonoBehaviour {
	private Camera cam;
	[SerializeField] private UIController ui;

	[SerializeField, Range(0, 1)] private float hoverStrength = 0.4f;

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
		// Check for pause to make game run slightly more efficient
		if (!GameManager.Instance.Paused) {
			CheckClick();
		
			// Check for hover every frame
			CheckHover();
		}
	}

	private void CheckHover() {
		// Make UI (current dialogs) block raycast.
		if (!EventSystem.current.IsPointerOverGameObject()) {
			// Raycast from mouse position
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
			// Make UI (current dialogs) block raycast.
			if (!EventSystem.current.IsPointerOverGameObject()) {
				// Raycast from mouse position
				if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out RaycastHit clickHitInfo)) {
					BaseTile clicked = clickHitInfo.collider.GetComponent<BaseTile>();

					// Check if the object that was hit was a tile, if so handle click
					if (clicked) {
						HandleClick(clicked);
					}
					// If raycast didn't hit an instance of BaseTile, stop showing the dialog
					else ui.HideDialog();
				}
				// If raycast didn't hit anything, hide the dialog
				else ui.HideDialog();
			}
		}
	}

	private void HandleClick(BaseTile clicked) {
		clicked.OnClick();
	}
}