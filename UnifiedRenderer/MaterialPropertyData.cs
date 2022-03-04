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
			#if UNITY_2021
			MaterialProperty.PropType.Int,
			#endif
			MaterialProperty.PropType.Color, MaterialProperty.PropType.Float,
			MaterialProperty.PropType.Range, MaterialProperty.PropType.Vector, MaterialProperty.PropType.Texture
		};
		#endif

		[SerializeField] private string displayName;
		[SerializeField] private string internalName;
		[SerializeField] private string materialName;
		[SerializeField] private string typeName;
		[SerializeField] private int materialId;

		[SerializeField] public int intValue;
		[SerializeField] public bool boolValue;
		[SerializeField] public float floatValue;
		
		[SerializeField] private SerializableColor _colorValue;
		[SerializeField] private SerializableVector _vectorValue;
		[SerializeField] private SerializableTexture _textureValue;
		
		//Accessors
		
		public string GetDisplayName => displayName;
		public string GetMaterialName => materialName;
		public string GetInternalName => internalName;
		public string GetTypeName => typeName;
		public int GetMaterialID => materialId;


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
		
		public string GetNameForDisplay => $"{(UnifiedRenderer.UseDisplayPropertyName ? displayName : internalName)}";

		public Type GetValueType =>
			_valueType ??= Type.GetType(typeName) ?? Type.GetType(typeName + ", UnityEngine.CoreModule", true);

		private Type _valueType = null;

		public object GetValue {
			get {
				if (GetValueType == typeof(int)) return intValue;
				if (GetValueType == typeof(float)) return floatValue;
				if (GetValueType == typeof(Color)) return colorValue;
				if (GetValueType == typeof(Vector4)) return vectorValue;
				if (GetValueType == typeof(Texture) || GetValueType.IsSubclassOf(typeof(Texture))) return textureValue;
				if (GetValueType == typeof(bool)) return boolValue;

				return null;
			}
		}

		// public bool HasEmptyValue => hasEmptyValue || typeName == String.Empty;

		public MaterialPropertyData(string mDisplayName, string mInternalName, string mMaterialName, int mMaterialId, object mValue) {
			displayName  = mDisplayName;
			internalName = mInternalName;
			materialName = mMaterialName;
			materialId   = mMaterialId;

			if (mValue != null) {
				UpdateValue(mValue);
			}
		}

		public bool UpdateValue(object mValue, Type typeOverride = null) {
			if (typeOverride == null) typeOverride = mValue.GetType();

			typeName = typeOverride.FullName;

			if (mValue is int intVal) intValue                     = intVal;
			else if (mValue is float floatVal) floatValue          = floatVal;
			else if (mValue is Color colorVal) colorValue          = colorVal;
			else if (mValue is Vector4 vectorVal) vectorValue      = vectorVal;
			else if (mValue is bool boolVal) boolValue             = boolVal;
			else if (typeOverride == typeof(Texture) || typeOverride.IsSubclassOf(typeof(Texture))) textureValue = (Texture) mValue;
			else {
				Debug.LogError("Unified Renderer: Unsupported type detected!");
				return false;
			}

			return true;
		}

		public void UpdateMaterialID(int id) {
			materialId = id;
		}

		public bool Equals(MaterialPropertyData data) {
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