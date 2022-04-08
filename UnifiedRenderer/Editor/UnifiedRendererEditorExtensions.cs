using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer.Editor {
	public static class UnifiedRendererEditorExtensions {
		public static void AddNewProperty(this UnifiedRenderer rend, MaterialPropertyData newProp) {
			rend.GetMaterialProperties.Add(newProp);
			EditorUtility.SetDirty(rend.gameObject);
		}

		public static void RemoveProperty(this UnifiedRenderer rend, MaterialPropertyData newProp) {
			rend.GetMaterialProperties.Remove(newProp);
			rend.ClearPropertyBlock();
			EditorUtility.SetDirty(rend.gameObject);
		}
		
		public static void ClearProperties(this UnifiedRenderer rend) {
			rend.GetMaterialProperties.Clear();
			EditorUtility.SetDirty(rend.gameObject);
		}
		
		public static bool ContainsProperty(this UnifiedRenderer rend, MaterialPropertyData newProp) {
			foreach (var propertyData in rend.GetMaterialProperties) {
				if (propertyData == newProp) {
					return true;
				}
			}

			return false;
		}

		public static bool IsPropertyApplicable(this UnifiedRenderer rend, MaterialPropertyData data) {
			foreach (var material in rend.GetRenderer.sharedMaterials) {
				foreach (var prop in material.GetProperties()) {
					if (prop.name == data.GetInternalName) {
						return true;
					}
				}
			}

			return false;
		}
		
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
	}
}