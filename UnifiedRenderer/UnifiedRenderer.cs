//Unified Renderer - is developed and maintained by Luka Kldiashvili
//Learn more at: https://github.com/lukakldiashvili

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer {
	[RequireComponent(typeof(Renderer))]
	[ExecuteAlways]
	public class UnifiedRenderer : MonoBehaviour {
		public static bool UseDisplayPropertyName = false;

		public Renderer GetRenderer {
			get {
				if (_renderer == null) {
					_renderer = GetComponent<Renderer>();
				}

				#if UNITY_EDITOR
				if (_renderer == null) {
					EditorApplication.playModeStateChanged -= ApplyBlockEditor;
				}
				#endif

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

		public List<MaterialPropertyData> GetMaterialProperties => materialProperties;

		[SerializeField]
		private List<MaterialPropertyData> materialProperties = new List<MaterialPropertyData>();

		private List<MaterialPropertyBlock> propertyBlocks = new List<MaterialPropertyBlock>();

		private int GetMaterialCount => GetRenderer.sharedMaterials.Length;


		private void OnEnable() {
			ApplyPropertiesToBlock();
		}

		private void OnDestroy() {
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged -= ApplyBlockEditor;
			#endif
		}

		public bool SetMaterialProperty(string identifier, object value, int materialIndex = 0,
        		                                MaterialPropertyNameType nameType = MaterialPropertyNameType.AUTO,
        		                                bool immediateApply = true) {
        	try {
        		var selectedProp = GetMaterialProperties.First(prop =>
        			prop.GetNameWithType(nameType) == identifier && prop.GetMaterialID == materialIndex);
        		
        		var result = selectedProp.UpdateValue(value);

        		if (immediateApply) ApplyPropertiesToBlock();
        		return result;
        	}
        	catch (Exception e) {
        		return false;
        	}

        	return false;
        }

		#region Property Getters
		
		public Color? GetMaterialPropertyColor(string identifier,
		                                       MaterialPropertyNameType nameType = MaterialPropertyNameType.AUTO) {
			var propertyData = FindDataWithIdentifier(identifier, nameType.GetDefaultIfAuto());
			
			if (!(propertyData is null) && propertyData.GetValueType == typeof(Color)) return propertyData.colorValue;

			return null;
		}

		public float? GetMaterialPropertyFloat(string identifier,
		                                     MaterialPropertyNameType nameType = MaterialPropertyNameType.AUTO) {
			var propertyData = FindDataWithIdentifier(identifier, nameType.GetDefaultIfAuto());
			
			if (!(propertyData is null) && propertyData.GetValueType == typeof(float)) return propertyData.floatValue;

			return null;
		}

		public int? GetMaterialPropertyInt(string identifier,
		                                       MaterialPropertyNameType nameType = MaterialPropertyNameType.AUTO) {
			var propertyData = FindDataWithIdentifier(identifier, nameType.GetDefaultIfAuto());
			
			if (!(propertyData is null) && propertyData.GetValueType == typeof(int)) return propertyData.intValue;

			return null;
		}
		
		#endregion

		void UpdateListMembers() {
			if (propertyBlocks.Count == 0 || GetMaterialCount != propertyBlocks.Count) {
				propertyBlocks.Clear();

				for (int i = 0; i < GetMaterialCount; i++) {
					propertyBlocks.Add(new MaterialPropertyBlock());
				}

				#if UNITY_EDITOR
				EditorApplication.playModeStateChanged -= ApplyBlockEditor;
				EditorApplication.playModeStateChanged += ApplyBlockEditor;
				#endif
			}
		}
		
		public void ClearPropertyBlock() {
			UpdateListMembers();

			foreach (var block in propertyBlocks) {
				block.Clear();
			}
		}

		public void ApplyPropertiesToBlock() {
			UpdateListMembers();
			ApplyBlock();
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
				
				if (valueType == typeof(int))
					propertyBlocks[matIndex].SetColor(internalName, propertyData.colorValue);
				if (valueType == typeof(float))
					propertyBlocks[matIndex].SetFloat(internalName, propertyData.floatValue);
				if (valueType == typeof(Color))
					propertyBlocks[matIndex].SetColor(internalName, propertyData.colorValue);
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
			
			SetAllBlocks();
		}

		public bool ContainsSameIdentifierAndIndex(MaterialPropertyData newData) {
			int matchCount = 0;
			
			foreach (var data in materialProperties) {
				if (data == newData) matchCount++;
			}

			return matchCount > 1;
		}

		private void SetAllBlocks() {
			if (GetMaterialCount != propertyBlocks.Count) return;
			
			for (int i = 0; i < GetMaterialCount; i++) {
				GetRenderer.SetPropertyBlock(propertyBlocks[i], i);
			}
		}

		private MaterialPropertyData FindDataWithIdentifier(string identifier,
		                                                    MaterialPropertyNameType nameType) {
			foreach (var propertyData in GetMaterialProperties) {
				if ((nameType == MaterialPropertyNameType.DISPLAY  && propertyData.GetDisplayName  == identifier) ||
				    (nameType == MaterialPropertyNameType.INTERNAL && propertyData.GetInternalName == identifier)) {
					return propertyData;
				}
			}

			return null;
		}
		
		
		#region Editor
		#if UNITY_EDITOR
		
		void ApplyBlockEditor(PlayModeStateChange change) {
			ApplyBlock();
		}
		
		#endif
		#endregion
	}
}