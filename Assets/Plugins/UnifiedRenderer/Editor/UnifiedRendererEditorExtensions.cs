using UnityEditor;

namespace Unify.UnifiedRenderer.Editor {
	public static class UnifiedRendererEditorExtensions {
		public static void AddNewProperty(this UnifiedRenderer rend, MaterialPropertyData newProp) {
			rend.GetMaterialProperties.Add(newProp);
			EditorUtility.SetDirty(rend.gameObject);
		}

		public static void RemoveProperty(this UnifiedRenderer rend, MaterialPropertyData newProp) {
			rend.GetMaterialProperties.Remove(newProp);
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
	}
}