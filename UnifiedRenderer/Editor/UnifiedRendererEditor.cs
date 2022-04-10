using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer.Editor {
	[CustomEditor(typeof(UnifiedRenderer), true)]
	[InitializeOnLoad]
	[CanEditMultipleObjects]
	public class UnifiedRendererEditor : UnityEditor.Editor {
		private bool showRendererAndFilter;

		static UnifiedRendererEditor() {
			EditorApplication.playModeStateChanged += _ => ApplyAllUnifiedRenderers();
			Undo.undoRedoPerformed                 += ApplyAllUnifiedRenderers;
		}

		SerializedProperty materialPropertiesProp;

		void OnEnable() {
			materialPropertiesProp = serializedObject.FindProperty("materialProperties");
		}

		static void ApplyAllUnifiedRenderers() {
			foreach (var unifiedRenderer in FindObjectsOfType<UnifiedRenderer>()) {
				unifiedRenderer.ApplyPropertiesToBlock();
			}
		}

		public override void OnInspectorGUI() {
			// base.OnInspectorGUI();
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			//----

			var uniRend = (UnifiedRenderer) target;

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
			EditorGUILayout.LabelField("Properties",
				new GUIStyle(GUI.skin.label) {alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
		}

		void DrawMaterialProperties(UnifiedRenderer uniRend) {
			Undo.RecordObject(target, "Property Change");

			EditorGUILayout.Space(10);

			for (int i = 0; i < materialPropertiesProp.arraySize; i++) {
				EditorGUILayout.PropertyField(materialPropertiesProp.GetArrayElementAtIndex(i));
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

		//autohide gizmo
		[UnityEditor.Callbacks.DidReloadScripts]
		private static void OnScriptsReloaded() {
			var Annotation  = Type.GetType("UnityEditor.Annotation, UnityEditor");
			var ClassId     = Annotation.GetField("classID");
			var ScriptClass = Annotation.GetField("scriptClass");
			var Flags       = Annotation.GetField("flags");
			var IconEnabled = Annotation.GetField("iconEnabled");

			Type AnnotationUtility = Type.GetType("UnityEditor.AnnotationUtility, UnityEditor");
			var GetAnnotations = AnnotationUtility.GetMethod("GetAnnotations",
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
			var SetIconEnabled = AnnotationUtility.GetMethod("SetIconEnabled",
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);

			Array annotations = (Array) GetAnnotations.Invoke(null, null);
			foreach (var a in annotations) {
				int    classId     = (int) ClassId.GetValue(a);
				string scriptClass = (string) ScriptClass.GetValue(a);
				int    flags       = (int) Flags.GetValue(a);
				int    iconEnabled = (int) IconEnabled.GetValue(a);

				// this is done to ignore any built in types
				if (string.IsNullOrEmpty(scriptClass)) {
					continue;
				}

				// load a json or text file with class names

				const int HasIcon     = 1;
				bool      hasIconFlag = (flags & HasIcon) == HasIcon;
				if (hasIconFlag && (iconEnabled != 0)) {
					SetIconEnabled?.Invoke(null, new object[] {classId, scriptClass, 0});
				}
			}
		}
	}
}