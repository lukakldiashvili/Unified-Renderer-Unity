using System;
using System.Collections.Generic;
using Unify.UnifiedRenderer.Serializables;
using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer {
	public enum MaterialPropertyNameType {
		AUTO,
		DISPLAY,
		INTERNAL
	}

	[Serializable]
	public class MaterialPropertyData {
		#if UNITY_EDITOR
		public static List<MaterialProperty.PropType> SupportedTypes = new List<MaterialProperty.PropType>() {
			MaterialProperty.PropType.Color, MaterialProperty.PropType.Int, MaterialProperty.PropType.Float,
			MaterialProperty.PropType.Range, MaterialProperty.PropType.Vector, MaterialProperty.PropType.Texture
		};
		#endif

		public string displayName;
		public string internalName;
		public string materialName;
		public string typeName;

		public int intValue;
		public bool boolValue;
		public float floatValue;
		public SerializableColor _colorValue;
		public SerializableVector _vectorValue;
		public SerializableTexture _textureValue;

		public Color colorValue {
			get => _colorValue.GetColor;
			set => _colorValue = new SerializableColor(value);
		}

		public Vector4 vectorValue {
			get => _vectorValue.GetVector;
			set => _vectorValue = new SerializableVector(value);
		}

		public Texture textureValue {
			get => _textureValue.GetTexture;
			set => _textureValue = new SerializableTexture(value);
		}

		public string GetNameWithType => $"{GetNameForDisplay} ({GetValueType.Name})";
		public string GetNameForDisplay => $"{(UnifiedRenderer.UseDisplayPropertyName ? displayName : internalName)}";

		public Type GetValueType => Type.GetType(typeName) ?? Type.GetType(typeName + ", UnityEngine.CoreModule", true);

		public object GetValue {
			get {
				if (GetValueType == typeof(int)) return intValue;
				if (GetValueType == typeof(float)) return floatValue;
				if (GetValueType == typeof(Color)) return colorValue;
				if (GetValueType == typeof(Vector4)) return vectorValue;
				if (GetValueType == typeof(Texture2D)) return textureValue;
				if (GetValueType == typeof(bool)) return boolValue;

				return null;
			}
		}

		// public bool HasEmptyValue => hasEmptyValue || typeName == String.Empty;

		public MaterialPropertyData(string mDisplayName, string mInternalName, string mMaterialName, object mValue) {
			displayName   = mDisplayName;
			internalName  = mInternalName;
			materialName  = mMaterialName;

			if (mValue != null) {
				UpdateValue(mValue);
			}
		}

		public bool UpdateValue(object mValue) {
			var valueType = mValue.GetType();
			typeName = valueType.FullName;

			if (mValue is int intVal) intValue                  = intVal;
			else if (mValue is float floatVal) floatValue       = floatVal;
			else if (mValue is Color colorVal) colorValue       = colorVal;
			else if (mValue is Vector4 vectorVal) vectorValue   = vectorVal;
			else if (mValue is bool boolVal) boolValue          = boolVal;
			else if (mValue is Texture textureVal && textureVal != null) textureValue = textureVal;
			else {
				Debug.LogError("Unified Renderer: Unsupported type detected!");
				return false;
			}

			return true;
		}

		public bool Equals(MaterialPropertyData data) {
			return data.internalName == internalName && data.materialName == materialName;
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