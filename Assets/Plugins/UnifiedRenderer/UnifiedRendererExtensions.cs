using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unify.UnifiedRenderer {
	public static class UnifiedRendererEditorExtensions {
		#if UNITY_EDITOR
		public static MaterialProperty[] GetPropertiesFromMaterial(Material mat) {
			return MaterialEditor.GetMaterialProperties(new Object[] {mat});
		}

		public static MaterialProperty[] GetProperties(this Material mat) {
			return MaterialEditor.GetMaterialProperties(new Object[] {mat});
		}

		public static void DrawSerializedObject(SerializedObject obj) {
			obj.UpdateIfRequiredOrScript();
			SerializedProperty iterator = obj.GetIterator();
			for (bool enterChildren = true; iterator.NextVisible(enterChildren); enterChildren = false) {
				using (new EditorGUI.DisabledScope("m_Script" == iterator.propertyPath))
					EditorGUILayout.PropertyField(iterator, true);
			}

			obj.ApplyModifiedProperties();
		}

		public static void DrawSerializedObjectProperty(SerializedObject obj, SerializedProperty property) {
			obj.UpdateIfRequiredOrScript();

			using (new EditorGUI.DisabledScope("m_Script" == property.propertyPath))
				EditorGUILayout.PropertyField(property, true);

			obj.ApplyModifiedProperties();
		}
		#endif
	}
}