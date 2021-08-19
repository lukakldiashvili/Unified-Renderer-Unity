using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer.Editor {
	public class MaterialPropertySelectPopup : PopupWindowContent {
		private readonly UnifiedRenderer _targetRend;

		private readonly Material[] _materials;
		private string[] _materialOptions;
		private int _selectedMaterial = -1;

		private MaterialProperty[] _properties;
		private string[] _propertyOptions;
		private int _selectedProperty = -1;

		private int SelectedMaterial {
			set {
				if (value != -1) {
					_properties =
						(from prop in _materials[value].GetProperties()
						 where MaterialPropertyData.SupportedTypes.Contains(prop.type)
						 select prop).ToArray();
				}

				_selectedMaterial = value;
			}

			get => _selectedMaterial;
		}

		private bool _setupComplete = false;

		public MaterialPropertySelectPopup(UnifiedRenderer targetRend, Material[] mats) {
			_targetRend      = targetRend;
			_materials       = mats;

			int matCount = 0;
			
			_materialOptions = (from mat in _materials select $"{mat.name} ({matCount++})").ToArray();
		}

		public override Vector2 GetWindowSize() {
			return new Vector2(350, 80);
		}

		public override void OnGUI(Rect rect) {
			GUILayout.Label("Add Reference", EditorStyles.boldLabel);

			var materialInd = EditorGUILayout.Popup("Select Material", SelectedMaterial, _materialOptions);

			if (materialInd != SelectedMaterial) {
				SelectedMaterial = materialInd;

				if (materialInd != -1) {
					_propertyOptions =
						(from prop in _properties
						 select UnifiedRenderer.UseDisplayPropertyName ? prop.displayName : prop.name).ToArray();
				}
			}

			if (SelectedMaterial == -1) return;

			var propertyInd = EditorGUILayout.Popup("Select Property", _selectedProperty, _propertyOptions);

			_selectedProperty = propertyInd;

			if (_selectedProperty != -1) {
				if (GUILayout.Button("Add Reference")) {
					FinishReference();
				}
			}
		}

		public override void OnOpen() {
			// Popup opened
		}

		public override void OnClose() {
			FinishReference();
		}

		void FinishReference() {
			if (_setupComplete) {
				return;
			}

			if (_selectedMaterial != -1 && _selectedProperty != -1) {
				// EditorUtility.SetDirty(_targetProp.GetGameObject);
				_setupComplete = true;

				var prop = _properties[_selectedProperty];

				var newData = new MaterialPropertyData(prop.displayName, prop.name, _materials[_selectedMaterial].name, _selectedMaterial,
					null);

				var type = prop.type;

				#if UNITY_2021
				if (type == MaterialProperty.PropType.Int)     newData.UpdateValue(prop.intValue);
				#endif
				if (type == MaterialProperty.PropType.Color)   newData.UpdateValue(prop.colorValue);
				if (type == MaterialProperty.PropType.Vector)  newData.UpdateValue(prop.vectorValue);
				if (type == MaterialProperty.PropType.Texture) newData.UpdateValue(prop.textureValue, typeof(Texture));
				if (type == MaterialProperty.PropType.Float ||
				    type == MaterialProperty.PropType.Range)   newData.UpdateValue(prop.floatValue);
				
				
				if (!_targetRend.ContainsProperty(newData)) {
					_targetRend.AddNewProperty(newData);
				}
				else {
					EditorUtility.DisplayDialog("Error adding property",
						"Selected property is already added to the list", "Got It!");
				}

				try {
					editorWindow.Close();
				}
				catch {
					// ignored
				}
			}
		}
	}
	
	public class MaterialIndexChangePopup : PopupWindowContent {
		private readonly MaterialPropertyData _targetData;
		
		private readonly Material[] _materials;
		private string[] _materialOptions;
		private int _selectedMaterial = -1;
		

		private bool _setupComplete = false;

		public MaterialIndexChangePopup(MaterialPropertyData data, Material[] mats) {
			_targetData = data;
			_materials  = mats;
			
			int matCount = 0;
			
			_materialOptions = (from mat in _materials select $"{mat.name} ({matCount++})").ToArray();
		}

		public override Vector2 GetWindowSize() {
			return new Vector2(350, 60);
		}

		public override void OnGUI(Rect rect) {
			GUILayout.Label("Update Material Index", EditorStyles.boldLabel);

			_selectedMaterial = EditorGUILayout.Popup("Select Material", _selectedMaterial, _materialOptions);

			if (_selectedMaterial != -1) {
				if (GUILayout.Button("Update Material Index")) {
					FinishReference();
				}
			}
		}

		public override void OnOpen() {
			// Popup opened
		}

		public override void OnClose() {
			FinishReference(true);
		}

		void FinishReference(bool isOnClose = false) {
			if (_setupComplete) {
				return;
			}

			if (_selectedMaterial != -1 && !isOnClose) {
				// EditorUtility.SetDirty(_targetProp.GetGameObject);
				_setupComplete = true;
				
				_targetData.UpdateMaterialID(_selectedMaterial);
				
				editorWindow.Close();
			}
		}
	}
}