using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unify.UnifiedRenderer {
	public static class UnifiedRendererEditorExtensions {

		public static MaterialPropertyNameType GetDefaultIfAuto(this MaterialPropertyNameType nameType) {
			return nameType == MaterialPropertyNameType.AUTO ? 
				(UnifiedRenderer.UseDisplayPropertyName ? MaterialPropertyNameType.DISPLAY : MaterialPropertyNameType.INTERNAL) : nameType;
		}

		#if UNITY_EDITOR
		public static MaterialProperty[] GetPropertiesFromMaterial(Material mat) {
			return MaterialEditor.GetMaterialProperties(new Object[] {mat});
		}

		public static MaterialProperty[] GetProperties(this Material mat) {
			return MaterialEditor.GetMaterialProperties(new Object[] {mat});
		}

		public static string GetNameWithType(this MaterialPropertyData property, MaterialPropertyNameType nameType) {
			nameType = nameType.GetDefaultIfAuto();
			
			if (nameType == MaterialPropertyNameType.INTERNAL) return property.GetInternalName;
			return property.GetDisplayName;
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