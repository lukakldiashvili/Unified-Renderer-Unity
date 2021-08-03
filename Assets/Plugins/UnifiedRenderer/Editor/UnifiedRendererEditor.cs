using System;
using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer.Editor {
	[CustomEditor(typeof(UnifiedRenderer))]
	public class UnifiedRendererEditor : UnityEditor.Editor {

		private bool showRendererAndFilter;

		public override void OnInspectorGUI() {
			// base.OnInspectorGUI();
			serializedObject.UpdateIfRequiredOrScript();
			EditorGUI.BeginChangeCheck();
			//----

			var uniRend              = (UnifiedRenderer) target;

			var serializedMeshRend   = new SerializedObject(uniRend.GetRenderer);

			showRendererAndFilter = EditorGUILayout.Foldout(showRendererAndFilter,
				new GUIContent("Default Renderer & Filter Settings",
					"Displays data that MeshRenderer and Mesh Filter would display"));

			if (showRendererAndFilter) {
				if (uniRend.GetMeshFilter != null) {
					var serializedMeshFilter = new SerializedObject(uniRend.GetMeshFilter);
					Unify.UnifiedRenderer.UnifiedRendererEditorExtensions.DrawSerializedObject(serializedMeshFilter);
				}
				
				Unify.UnifiedRenderer.UnifiedRendererEditorExtensions.DrawSerializedObject(serializedMeshRend);
			}

			DrawHead(serializedMeshRend);

			DrawMaterialProperties(uniRend);

			DrawPropertyButtons(uniRend);

			//----
			if (EditorGUI.EndChangeCheck()) {
				// Undo.RecordObject(target, "Property Apply");
				uniRend.ApplyPropertiesToBlock();
				EditorUtility.SetDirty(target);
			}
			
			serializedObject.ApplyModifiedProperties();
		}

		void DrawHead(SerializedObject serializedMeshRend) {
			EditorGUILayout.Space(10);
			EditorGUILayout.LabelField("Unified Renderer",
				new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
			EditorGUILayout.Space(5);
			Unify.UnifiedRenderer.UnifiedRendererEditorExtensions.DrawSerializedObjectProperty(serializedMeshRend,
				serializedMeshRend.FindProperty("m_Materials"));
		}

		void DrawMaterialProperties(UnifiedRenderer uniRend) {
			Undo.RecordObject(target, "Property Change");
			
			EditorGUILayout.Space(5);
			
			EditorGUILayout.BeginHorizontal();
			
			EditorGUILayout.LabelField(new GUIContent("Properties:", "Used when property has been removed from the override list"),
				new GUIStyle(GUI.skin.label) {fontStyle = FontStyle.Italic});
			
			if (GUILayout.Button("Clear Unused Data")) {
				uniRend.ClearPropertyBlock();
			}
			
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < uniRend.GetMaterialProperties.Count; i++) {
				var data = uniRend.GetMaterialProperties[i];

				EditorGUILayout.BeginHorizontal();

				if (!uniRend.IsPropertyApplicable(data)) {
					// DrawMiniButton(()=>{});
					GUI.backgroundColor = Color.red;
				}

				if (data.GetValueType == typeof(int))
					data.intValue = EditorGUILayout.IntField(data.GetNameWithType, data.intValue);
				if (data.GetValueType == typeof(float))
					data.floatValue = EditorGUILayout.FloatField(data.GetNameWithType, data.floatValue);
				if (data.GetValueType == typeof(Color))
					data.colorValue = EditorGUILayout.ColorField(data.GetNameWithType, data.colorValue);
				if (data.GetValueType == typeof(bool))
					data.boolValue = EditorGUILayout.Toggle(data.GetNameWithType, data.boolValue);
				if (data.GetValueType == typeof(Vector4))
					data.vectorValue = EditorGUILayout.Vector4Field(data.GetNameWithType, data.vectorValue);
				if (data.GetValueType == typeof(Texture2D) || data.GetValueType == typeof(Texture) || data.GetValueType == typeof(Texture3D))
					data.textureValue = (Texture) EditorGUILayout.ObjectField(data.GetNameWithType, data.textureValue, typeof(Texture));

				GUI.backgroundColor = Color.white;

				DrawMiniButton(() => { uniRend.RemoveProperty(data); });

				EditorGUILayout.EndHorizontal();
			}
		}

		void DrawPropertyButtons(UnifiedRenderer uniRend) {
			Undo.RecordObject(target, "Property Add");
			EditorGUILayout.Space(5);

			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Property")) {
				
				PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero),
					new MaterialPropertySelectPopup(uniRend, uniRend.GetRenderer.sharedMaterials));
			}

			if (GUILayout.Button("Clear Properties")) {
				uniRend.ClearProperties();
			}

			EditorGUILayout.EndHorizontal();
		}

		void DrawMiniButton(Action onClick) {
			var st = EditorStyles.miniButton;
			st.wordWrap   = true;
			st.fixedWidth = 25;

			if (GUILayout.Button("X", st)) {
				onClick?.Invoke();
			}
		}
	}
}