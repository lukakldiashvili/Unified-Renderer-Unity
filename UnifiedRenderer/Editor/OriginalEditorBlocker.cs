using UnityEditor;
using UnityEngine;

namespace Unify.UnifiedRenderer.Editor {
	[CustomEditor(typeof(MeshRenderer))]
	public class OriginalEditorBlocker_MeshRenderer : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			if (!UnifiedRendererSettings.DisableOriginalEditors) {
				base.OnInspectorGUI();
			}
		}
	}

	[CustomEditor(typeof(MeshFilter))]
	public class OriginalEditorBlocker_MeshFilter : UnityEditor.Editor {

		public override void OnInspectorGUI() {
			if (!UnifiedRendererSettings.DisableOriginalEditors) {
				base.OnInspectorGUI();
			}
		}
	}
}