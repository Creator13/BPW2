using UnityEditor;
using UnityEngine;
using UnityEngine.WSA;

namespace Editor {
	[CustomEditor(typeof(TileGrid))]
	public class CustomGridInspector : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			DrawDefaultInspector();

			TileGrid grid = (TileGrid) target;
			
			GUILayout.BeginHorizontal();
 			if (GUILayout.Button("Generate")) {
				grid.Generate();
			}

            if (GUILayout.Button("Destroy")) {
	            grid.DestroyGrid(true);
            }
            GUILayout.EndHorizontal();
		}
	}
}