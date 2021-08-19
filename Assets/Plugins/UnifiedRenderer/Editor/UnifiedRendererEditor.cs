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

			var uniRend = (UnifiedRenderer)target;

			DrawHead(uniRend);

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

		void DrawHead(UnifiedRenderer unifiedRend) {
			EditorGUILayout.LabelField("Unified Renderer",
				new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
			EditorGUILayout.Space(5);
			
			showRendererAndFilter = EditorGUILayout.Foldout(showRendererAndFilter,
				new GUIContent("Materials & Mesh"));

			if (showRendererAndFilter) {
				if (unifiedRend.GetMeshFilter != null) {
					var serializedMeshFilter = new SerializedObject(unifiedRend.GetMeshFilter);
					Unify.UnifiedRenderer.UnifiedRendererEditorExtensions.DrawSerializedObject(serializedMeshFilter);
					
					EditorGUILayout.Space(5);
				}

				var mats = unifiedRend.GetRenderer.sharedMaterials;

				for (int i = 0; i < mats.Length; i++) {
					mats[i] = (Material)EditorGUILayout.ObjectField($"Material {i}:", mats[i], typeof(Material), false);
				}
			}
		}

		void DrawMaterialProperties(UnifiedRenderer uniRend) {
			Undo.RecordObject(target, "Property Change");

			EditorGUILayout.Space(5);

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.LabelField(
				new GUIContent("Properties:", "Used when property has been removed from the override list"),
				new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Italic });
			
			if (GUILayout.Button("Clear Unused Data")) {
				uniRend.ClearPropertyBlock();
			}

			EditorGUILayout.EndHorizontal();
			
			EditorGUILayout.Space(10);

			for (int i = 0; i < uniRend.GetMaterialProperties.Count; i++) {
				var data = uniRend.GetMaterialProperties[i];

				var horRect = EditorGUILayout.BeginHorizontal();

				var fieldName = $"{data.GetNameForDisplay} (Mat ID: {data.GetMaterialID})";

				if (data.GetValueType == typeof(int))
					data.intValue = EditorGUILayout.IntField(fieldName, data.intValue);
				if (data.GetValueType == typeof(float))
					data.floatValue = EditorGUILayout.FloatField(fieldName, data.floatValue);
				if (data.GetValueType == typeof(Color))
					data.colorValue = EditorGUILayout.ColorField(fieldName, data.colorValue);
				if (data.GetValueType == typeof(bool))
					data.boolValue = EditorGUILayout.Toggle(fieldName, data.boolValue);
				if (data.GetValueType == typeof(Vector4))
					data.vectorValue = EditorGUILayout.Vector4Field(fieldName, data.vectorValue);
				if (data.GetValueType == typeof(Texture)) {
					var valueAssigned = EditorGUILayout.ObjectField(fieldName, data.textureValue, typeof(Texture),
						false);

					if (valueAssigned != null)
						data.textureValue = (Texture)valueAssigned;
					else
						data.textureValue = null;
				}

				DrawMiniButton("O", () => {
					PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero),
						new MaterialIndexChangePopup(data, uniRend.GetRenderer.sharedMaterials));
				});
				
				DrawMiniButton("X", () => {
					uniRend.RemoveProperty(data);
					uniRend.ClearPropertyBlock();
				});

				EditorGUILayout.EndHorizontal();

				var backgroundRedRect = new Rect(horRect.x - 13, horRect.y, EditorGUIUtility.currentViewWidth,
					horRect.height);

				if (!uniRend.IsPropertyApplicable(data)) {
					// DrawMiniButton(()=>{});
					EditorGUI.DrawRect(backgroundRedRect, new Color(1f, 0, 0, 0.2f));
				}
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

		void DrawMiniButton(string character, Action onClick) {
			var st = EditorStyles.miniButton;
			st.wordWrap   = true;
			st.fixedWidth = 25;

			if (GUILayout.Button(character, st)) {
				onClick?.Invoke();
			}
		}
	}
}