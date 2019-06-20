using Tile;
using UnityEditor;
using UnityEngine;

namespace Editor {
	[CustomEditor(typeof(TileGrid))]
	public class CustomTileGridInspector : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			TileGrid grid = (TileGrid) target;
			
			GUILayout.BeginHorizontal();
 			if (GUILayout.Button("Generate")) {
				grid.Generate();
			}

            if (GUILayout.Button("Destroy")) {
	            grid.DestroyGrid();
            }
            GUILayout.EndHorizontal();
		}
	}
}