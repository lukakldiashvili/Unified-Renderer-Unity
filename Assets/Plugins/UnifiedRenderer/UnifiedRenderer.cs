//Unified Renderer - is developed and maintained by Luka Kldiashvili
//Learn more at: https://github.com/lukakldiashvili

using System;
using System.Collections.Generic;
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

		private MaterialPropertyBlock _propBlock;

		private void OnEnable() {
			ApplyPropertiesToBlock();
		}

		private void OnDestroy() {
			#if UNITY_EDITOR
			EditorApplication.playModeStateChanged -= ApplyBlockEditor;
			#endif
		}

		public bool SetMaterialProperty(string identifier, object value,
		                                MaterialPropertyNameType nameType = MaterialPropertyNameType.AUTO,
		                                bool immediateApply = true) {
			if (nameType == MaterialPropertyNameType.AUTO) {
				nameType = UseDisplayPropertyName
					? MaterialPropertyNameType.DISPLAY : MaterialPropertyNameType.INTERNAL;
			}
			
			foreach (var propertyData in GetMaterialProperties) {
				if ((nameType == MaterialPropertyNameType.DISPLAY  && propertyData.GetDisplayName  == identifier) ||
				    (nameType == MaterialPropertyNameType.INTERNAL && propertyData.GetInternalName == identifier)) {
					try {
						var result = propertyData.UpdateValue(value);

						if (immediateApply) ApplyPropertiesToBlock();
						return result;
					}
					catch (Exception e) {
						return false;
					}
				}
			}

			return false;
		}

		#region Property Getters
		
		public Color? GetMaterialPropertyColor(string identifier,
		                                       MaterialPropertyNameType nameType = MaterialPropertyNameType.DISPLAY) {
			var propertyData = FindDataWithIdentifier(identifier, nameType);
			
			if (!(propertyData is null) && propertyData.GetValueType == typeof(Color)) return propertyData.colorValue;

			return null;
		}

		public float? GetMaterialPropertyFloat(string identifier,
		                                     MaterialPropertyNameType nameType = MaterialPropertyNameType.DISPLAY) {
			var propertyData = FindDataWithIdentifier(identifier, nameType);
			
			if (!(propertyData is null) && propertyData.GetValueType == typeof(float)) return propertyData.floatValue;

			return null;
		}

		public int? GetMaterialPropertyInt(string identifier,
		                                       MaterialPropertyNameType nameType = MaterialPropertyNameType.DISPLAY) {
			var propertyData = FindDataWithIdentifier(identifier, nameType);
			
			if (!(propertyData is null) && propertyData.GetValueType == typeof(int)) return propertyData.intValue;

			return null;
		}
		
		#endregion

		public void ClearPropertyBlock() {
			ApplyBlock(true);
		}

		public void ApplyPropertiesToBlock() {
			if (_propBlock == null) {
				_propBlock = new MaterialPropertyBlock();

				#if UNITY_EDITOR
				EditorApplication.playModeStateChanged -= ApplyBlockEditor;
				EditorApplication.playModeStateChanged += ApplyBlockEditor;
				#endif
			}

			ApplyBlock();
		}
		
		void ApplyBlock(bool clearBlock = false) {
			GetRenderer.GetPropertyBlock(_propBlock);

			if (clearBlock) {
				_propBlock.Clear();
				GetRenderer.SetPropertyBlock(_propBlock);
				
				return;
			}

			foreach (var propertyData in materialProperties) {
				var internalName = propertyData.GetInternalName;
				
				if (propertyData.GetValueType == typeof(int))
					_propBlock.SetColor(internalName, propertyData.colorValue);
				if (propertyData.GetValueType == typeof(float))
					_propBlock.SetFloat(internalName, propertyData.floatValue);
				if (propertyData.GetValueType == typeof(Color))
					_propBlock.SetColor(internalName, propertyData.colorValue);
				if (propertyData.GetValueType == typeof(Vector4))
					_propBlock.SetVector(internalName, propertyData.vectorValue);
				if (propertyData.GetValueType == typeof(Texture)) {
					if (propertyData.textureValue != null) {
						_propBlock.SetTexture(internalName, propertyData.textureValue);
					}
					else {
						if (_propBlock.HasTexture(internalName)) {
							_propBlock.Clear();
						}
					}
				}
				if (propertyData.GetValueType == typeof(bool))
					_propBlock.SetFloat(internalName, propertyData.boolValue ? 1 : 0);
			}

			GetRenderer.SetPropertyBlock(_propBlock);
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