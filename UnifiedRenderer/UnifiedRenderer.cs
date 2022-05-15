//Unified Renderer - is developed and maintained by Luka Kldiashvili
//Learn more at: https://github.com/lukakldiashvili

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unify.UnifiedRenderer {
	[RequireComponent(typeof(Renderer))]
	[DisallowMultipleComponent]
	[ExecuteAlways]
	public class UnifiedRenderer : MonoBehaviour {
		public static bool UseDisplayPropertyName = false;

		public Renderer GetRenderer {
			get {
				if (_renderer == null) {
					_renderer = GetComponent<Renderer>();
				}

				return _renderer;
			}
		}

		public MeshFilter GetMeshFilter {
			get {
				if (_meshFilter == null) _meshFilter = GetComponent<MeshFilter>();
				return _meshFilter;
			}
		}

		private Renderer _renderer = null;
		private MeshFilter _meshFilter = null;


		[SerializeField]
		private List<MaterialPropertyData> materialProperties = new();

		private List<MaterialPropertyBlock> propertyBlocks = new();

		
		public List<MaterialPropertyData> GetMaterialProperties => materialProperties;
		private int GetMaterialCount => GetRenderer.sharedMaterials.Length;
		
		
		private void OnEnable() {
			ApplyPropertiesToBlock();
		}

		private void OnDestroy() {
			DiscardAllProperties();
		}
		
		public MaterialPropertyData AddProperty<T>(string identifier, int materialIndex = -1) {
			if (GetPropertyData(identifier, materialIndex) != null) return null;
			
			var newProperty = new MaterialPropertyData(identifier,identifier, "unknown", materialIndex, ReadBaseValue<T>(identifier, materialIndex));
			
			materialProperties.Add(newProperty);

			return newProperty;
		}

		public MaterialPropertyData AddProperty(string identifier, object value, int materialIndex = -1) {
			if (GetPropertyData(identifier, materialIndex) != null) return null;
			
			var newProperty = new MaterialPropertyData(identifier,identifier, "unknown", materialIndex, value);
			
			materialProperties.Add(newProperty);

			return newProperty;
		}

		public void RemoveProperty(string identifier, int materialIndex = -1) {
			MaterialPropertyData dataFound = GetPropertyData(identifier, materialIndex);
			if (dataFound != null) RemoveProperty(dataFound);
		}

		public void RemoveProperty(MaterialPropertyData dataToRemove) {
			GetMaterialProperties.Remove(dataToRemove);

			ClearPropertyBlocks();
			ApplyPropertiesToBlock();
		}
		
		public void DiscardAllProperties() {
			GetMaterialProperties.Clear();

			ClearPropertyBlocks();
			ApplyPropertiesToBlock();
		}

		public bool HasProperty(string identifier, int materialIndex = -1) =>
			GetPropertyData(identifier, materialIndex) != null;
		
		public bool TrySetPropertyValue<T>(string identifier, T value, int materialIndex = -1, bool autoGenerate = true) {
			try {
				var selectedProp = GetPropertyData(identifier, materialIndex);
				
				if (selectedProp == null && autoGenerate) { 
					Debug.LogWarning($"Unified Renderer: Property with name '{identifier}' was not found on object '{gameObject}'. <b>Adding it automatically</b>", this);

					selectedProp = AddProperty<T>(identifier, materialIndex);
				}

				var result = selectedProp.UpdateValue(value);

				ApplyPropertiesToBlock();
				return result;
			}
        	catch (Exception e) {
	            Debug.LogError(e, gameObject);
        	}
			
			return false;
		}
		
		/// <summary>
		/// Gets property by identifier and returns its value. It is recommended to use TryGetPropertyValue instead
		/// to avoid NullReferenceException.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="matIndex">Pass -1 to get global property.</param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetPropertyValue<T> (string identifier, int matIndex = -1) {
			return TryGetPropertyValue<T>(identifier, out var value, matIndex) ? value : default;
		}

		/// <summary>
		/// Tries to return global property by default.
		/// </summary>
		/// <param name="identifier"></param>
		/// <param name="matIndex">Pass -1 to get global property.</param>
		/// <param name="value"></param>
		/// <param name="autoGenerate"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public bool TryGetPropertyValue<T> (string identifier, out T value, int matIndex = -1, bool autoGenerate = true) {
			var propertyData = GetPropertyData(identifier, matIndex);

			if (propertyData is null && autoGenerate) {
				propertyData = AddProperty<T>(identifier, matIndex);
			}

			if (propertyData is not null && propertyData.GetValueType == typeof(T)) {
				value = (T) propertyData.GetValue;

				return true;
			}

			Debug.LogWarning($"Unified Renderer: Property with name '{identifier} and type {typeof(T)}", this);
			
			value = default;
			return false;
		}

		public void EnableColorHDR(string identifier, int matIndex = -1, bool isEnabled = true) {
			var propertyData = GetPropertyData(identifier, matIndex);

			propertyData?.EnableHDR(isEnabled);
		}

		public bool ContainsIdenticalData(MaterialPropertyData newData) {
			return materialProperties.Count(data => data == newData) > 1;
		}

		public void ApplyPropertiesToBlock() {
			UpdateListMembers();
			ApplyBlock();
		}

		public void UpdatePropertyMaterialIndex(MaterialPropertyData data, int index) {
			data.UpdateMaterialID(index);
			
			ClearPropertyBlocks();
			ApplyPropertiesToBlock();
		}
		
		void UpdateListMembers() {
			propertyBlocks.Clear();

			for (int i = 0; i < GetMaterialCount; i++) {
				propertyBlocks.Add(new MaterialPropertyBlock());
			}
		}
		
		void ApplyBlock(bool getBlocksFirst = false) {
			if (getBlocksFirst) {
				for (int i = 0; i < GetMaterialCount; i++) {
					GetRenderer.GetPropertyBlock(propertyBlocks[i], i);
				}
			}

			foreach (var propertyData in materialProperties) {
				var internalName = propertyData.GetInternalName;
				var valueType    = propertyData.GetValueType;
				var matIndex     = propertyData.GetMaterialID;

				if (matIndex == -1) {
					for (int i = 0; i < GetMaterialCount; i++) {
						SetBlockValue(internalName, valueType, i, propertyData);
					}
				}
				else {
					SetBlockValue(internalName, valueType, matIndex, propertyData);
				}
			}
			
			SetAllBlocks();
		}
		
		private void SetBlockValue(string internalName, Type valueType, int matIndex, MaterialPropertyData propertyData) {
			if (valueType == typeof(int))
				propertyBlocks[matIndex].SetColor(internalName, propertyData.colorValue);
			if (valueType == typeof(float))
				propertyBlocks[matIndex].SetFloat(internalName, propertyData.floatValue);
			if (valueType == typeof(Color))
				propertyBlocks[matIndex].SetColor(internalName, propertyData.GetIsColorHDR ? propertyData.hdrColorValue : propertyData.colorValue);
			if (valueType == typeof(Vector4))
				propertyBlocks[matIndex].SetVector(internalName, propertyData.vectorValue);
			if (valueType == typeof(Texture) || valueType.IsSubclassOf(typeof(Texture))) {
				if (propertyData.textureValue != null) {
					propertyBlocks[matIndex].SetTexture(internalName, propertyData.textureValue);
				}
				else {
					propertyBlocks[matIndex].Clear();
				}
			}
			if (valueType == typeof(bool))
				propertyBlocks[matIndex].SetFloat(internalName, propertyData.boolValue ? 1 : 0);
		}
		

		private void SetAllBlocks() {
			if (GetMaterialCount != propertyBlocks.Count) return;
			
			for (int matIndex = 0; matIndex < GetMaterialCount; matIndex++) {
				GetRenderer.SetPropertyBlock(propertyBlocks[matIndex], matIndex);
			}
		}

		private T ReadBaseValue<T>(string identifier, int matIndex = -1) {
			var mValue = typeof(T);

			object ret = default;

			matIndex = Mathf.Clamp(matIndex, 0, GetMaterialCount - 1);
			Material targetMat = GetRenderer.sharedMaterials[matIndex];
			
			if (mValue      == typeof(int)) ret     = targetMat.GetInteger(identifier);
			else if (mValue == typeof(float)) ret   = targetMat.GetFloat(identifier);
			else if (mValue == typeof(Color)) ret   = targetMat.GetColor(identifier);
			else if (mValue == typeof(Vector4)) ret = targetMat.GetVector(identifier);
			else if (mValue == typeof(bool)) ret    = targetMat.GetInteger(identifier);
			else if (mValue == typeof(Texture)) ret = targetMat.GetTexture(identifier);

			return (T) ret;
		}

		private MaterialPropertyData GetPropertyData(string identifier, int matIndex) {
			foreach (var propertyData in GetMaterialProperties) {
				if (propertyData.GetInternalName == identifier && propertyData.GetMaterialID == matIndex) {
					return propertyData;
				}
			}

			return null;
		}
		
		[ContextMenu("Clear Unused Data")]
		private void ClearPropertyBlocks() {
			UpdateListMembers();

			foreach (var block in propertyBlocks) {
				block.Clear();
			}
		}
	}
}