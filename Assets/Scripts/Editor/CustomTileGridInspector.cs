using Tile;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

namespace Editor {
	[CustomEditor(typeof(TileGrid))]
	public class CustomTileGridInspector : UnityEditor.Editor {
		public override void OnInspectorGUI() {
			TileGrid grid = (TileGrid) target;

			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("groundTilePrefab"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("sourceTilePrefab"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("riverTilePrefab"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("growableTilePrefab"));
			
			GUILayout.Space(10);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("dryColor"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("wetColor"));
			
			GUILayout.Space(10);
			
			EditorGUILayout.PropertyField(serializedObject.FindProperty("sourceCapacity"));
			
			GUILayout.Space(10);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("mainCamera"));
			
			GUILayout.Space(10);

			EditorGUILayout.PropertyField(serializedObject.FindProperty("gridSizeX"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("gridSizeZ"));

			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(serializedObject.FindProperty("tileSize"));
			EditorGUILayout.PropertyField(serializedObject.FindProperty("yOffset"));
			if (EditorGUI.EndChangeCheck()) {
				serializedObject.ApplyModifiedProperties();
				grid.UpdateTilePositions();
			}

			serializedObject.ApplyModifiedProperties();

			GUILayout.Space(10);
			
			DrawButtons(grid);
		}

		private void DrawButtons(TileGrid grid) {
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