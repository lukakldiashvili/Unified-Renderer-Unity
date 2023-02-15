using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer.Editor {
    
    [CustomPropertyDrawer(typeof(Vector4))]
    public class Vector4Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            var newVectorValue = EditorGUI.Vector4Field(position, label, property.vector4Value);

            if (newVectorValue != property.vector4Value) {
                property.vector4Value = newVectorValue;
            }
        }
    }
}