using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unify.UnifiedRenderer.Editor {
	[CustomPropertyDrawer(typeof(MaterialPropertyData))]
	public class MaterialPropertyDataDrawer : PropertyDrawer {
		private BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public |
		                                 BindingFlags.NonPublic;

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
			EditorGUI.BeginProperty(position, label, property);

			var rect = new Rect(position.x, position.y, position.width, position.height);
			
			var  target        = property.serializedObject.targetObject;
			var  propertyType  = property.serializedObject.targetObject.GetType();
			var  splitPath     = property.propertyPath.Split('.');
			bool isMultiTarget = property.serializedObject.targetObjects.Length > 1;
			
			
			var fieldName = splitPath.First();
			var fieldInfo = propertyType.GetField(fieldName, bindFlags);
			
			var isArray = fieldInfo.FieldType.IsArray;
			var isList  = typeof(IList).IsAssignableFrom(fieldInfo.FieldType);
			
			if (isArray || isList) {
				var listIndex = int.Parse(new Regex(@"(?<=\[)(.*?)(?=\])").Match(splitPath.Last()).Value);
				
				IList<MaterialPropertyData> dataList = null;
			
				if (isList) {
					dataList = (IList<MaterialPropertyData>) fieldInfo.GetValue(target);
				} else {
					dataList = (MaterialPropertyData[]) fieldInfo.GetValue(target);
				}

				if (dataList != null && dataList.Count > listIndex) {
					var thisProperty = dataList[listIndex];
			
					DrawMaterialProperty(rect, property, thisProperty, target, isMultiTarget);
				}
			}

			foreach (var objectTargetObject in property.serializedObject.targetObjects) {
				if (objectTargetObject is UnifiedRenderer uniRend) {
					uniRend.ApplyPropertiesToBlock();
				}
			}

			EditorGUI.EndProperty();
		}

		void DrawMaterialProperty(Rect rect, SerializedProperty property, MaterialPropertyData data, Object target, bool isMultiTarget) {
			string matIndex      = data.GetMaterialID == -1 ? "(Global)" : $"(matId: {data.GetMaterialID})";
			var    fieldNameCont = new GUIContent($"{data.GetInternalName} {matIndex}");

			rect.width -= 55;
			
			if (data.GetValueType == typeof(int))
				EditorGUI.PropertyField(rect, property.FindPropertyRelative("intValue"), fieldNameCont);
			else if (data.GetValueType == typeof(float))
				EditorGUI.PropertyField(rect, property.FindPropertyRelative("floatValue"), fieldNameCont);
			else if (data.GetValueType == typeof(Color))
				EditorGUI.PropertyField(rect, property.FindPropertyRelative("colorValue"), fieldNameCont);
			else if (data.GetValueType == typeof(bool))
				EditorGUI.PropertyField(rect, property.FindPropertyRelative("boolValue"), fieldNameCont);
			else if (data.GetValueType == typeof(Vector4))
				EditorGUI.PropertyField(rect, property.FindPropertyRelative("vectorValue"), fieldNameCont);
			else if (data.GetValueType == typeof(Texture) || data.GetValueType.IsSubclassOf(typeof(Texture))) {
				EditorGUI.PropertyField(rect, property.FindPropertyRelative("textureValue"), fieldNameCont);
			}

			
			
			if (target is UnifiedRenderer targetUniRend) {
				if (!isMultiTarget) {
					var backgroundRedRect = new Rect(rect);
					
					var pos = rect.position;
					pos.x += rect.width + 5;

					rect.width    = 25;
					rect.position = pos;
					
					DrawMiniButton(rect, "O",
						() => {
							PopupWindow.Show(new Rect(Event.current.mousePosition, Vector2.zero),
								new MaterialIndexChangePopup(targetUniRend, data, targetUniRend.GetRenderer.sharedMaterials));
						});

					pos.x         += 25;
					rect.position =  pos;
			
					DrawMiniButton(rect, "X", () => {
						targetUniRend.RemoveProperty(data);
					});
					
					if (!targetUniRend.IsPropertyApplicable(data)) {
						EditorGUI.DrawRect(backgroundRedRect, new Color(1f, 0, 0, 0.2f));
					}
				}
			} 
		}
		
		void DrawMiniButton(Rect rect, string character, Action onClick) {
			var st = EditorStyles.miniButton;
			st.wordWrap   = true;
			st.fixedWidth = 25;

			if (GUI.Button(rect, character, st)) {
				onClick?.Invoke();
			}
		}
	}
}