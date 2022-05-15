using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using PropType = UnityEditor.MaterialProperty.PropType;
#endif

namespace Unify.UnifiedRenderer {

	[Serializable]
	public class MaterialPropertyData {
		#if UNITY_EDITOR
		public static List<PropType> SupportedTypes = new List<PropType>() {
			PropType.Color, PropType.Float, PropType.Range, PropType.Vector, PropType.Texture
		};
		#endif

		[SerializeField] private string displayName;
		[SerializeField] private string internalName;
		[SerializeField] private string materialName;
		[SerializeField] private string typeName;
		[SerializeField] private int materialId;

		[SerializeField] public bool boolValue;
		[SerializeField] public float floatValue;
		
		[SerializeField] public Color colorValue;
		[ColorUsage(true, true)]
		[SerializeField] public Color hdrColorValue;
		[SerializeField] public Vector4 vectorValue;
		[SerializeField] public Texture textureValue;

		[SerializeField] private bool isHdr;
		
		
		//Accessors
		public string GetDisplayName => displayName;
		public string GetMaterialName => materialName;
		public string GetInternalName => internalName;
		public string GetTypeName => typeName;
		public int GetMaterialID => materialId;
		public bool GetIsColorHDR => isHdr;
		
		public string GetNameForDisplay => $"{(UnifiedRenderer.UseDisplayPropertyName ? displayName : internalName)}";

		public Type GetValueType =>
			_valueType = _valueType ?? (Type.GetType(typeName) ?? Type.GetType(typeName + ", UnityEngine.CoreModule", true));

		private Type _valueType = null;

		public object GetValue {
			get {
				if (GetValueType == typeof(float) || GetValueType == typeof(int)) return floatValue;
				if (GetValueType == typeof(Color)) return GetIsColorHDR ? hdrColorValue : colorValue;
				if (GetValueType == typeof(Vector4)) return vectorValue;
				if (GetValueType == typeof(Texture) || GetValueType.IsSubclassOf(typeof(Texture))) return textureValue;
				if (GetValueType == typeof(bool)) return boolValue;

				return null;
			}
		}

		public MaterialPropertyData(string mDisplayName, string mInternalName, string mMaterialName, int mMaterialId, object mValue) {
			displayName  = mDisplayName;
			internalName = mInternalName;
			materialName = mMaterialName;
			materialId   = mMaterialId;

			if (mValue != null) {
				UpdateValueBoxed(mValue);
			}
		}

		[Obsolete]
		private bool UpdateValueBoxed(object mValue, Type typeOverride = null) {
			return UpdateValue(mValue, typeOverride);
		}
		
		public bool UpdateValue<T>(T mValue, Type typeOverride = null) {
			//Manually converting int to float
			if (mValue is int) typeOverride = typeof(float);

			typeOverride = typeOverride ?? mValue.GetType(); 

			typeName = typeOverride.FullName;

			if (mValue is int intVal) floatValue                   = intVal;
			else if (mValue is float floatVal) floatValue          = floatVal;
			else if (mValue is Color colorVal) {
				if (isHdr) hdrColorValue = colorVal;
				else colorValue          = colorVal;
			}
			else if (mValue is Vector4 vectorVal) vectorValue      = vectorVal;
			else if (mValue is bool boolVal) boolValue             = boolVal;
			else if (typeOverride == typeof(Texture) || typeOverride.IsSubclassOf(typeof(Texture))) textureValue = mValue as Texture;
			else {
				Debug.LogError("Unified Renderer: Unsupported type detected!");
				return false;
			}

			return true;
		}

		public void UpdateMaterialID(int id) {
			materialId = id;
		}

		public void EnableHDR(bool enabled = true) {
			isHdr = enabled;

			if (isHdr) {
				hdrColorValue = colorValue;
			}
			else {
				colorValue = hdrColorValue;
			}
		}

		public bool Equals(MaterialPropertyData data) {
			if (data == null) return false;
			
			return (data.internalName == internalName && data.materialName == materialName) && data.materialId == materialId;
		}

		public static bool operator ==(MaterialPropertyData lhs, MaterialPropertyData rhs) {
			if (lhs is null) {
				if (rhs is null) {
					return true;
				}

				return false;
			}

			return lhs.Equals(rhs);
		}

		public static bool operator !=(MaterialPropertyData lhs, MaterialPropertyData rhs) => !(lhs == rhs);
	}
}