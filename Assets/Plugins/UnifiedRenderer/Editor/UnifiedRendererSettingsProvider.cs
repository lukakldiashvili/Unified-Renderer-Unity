using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using PrefSet = UnifiedRendererSettings;

namespace Unify.UnifiedRenderer.Editor {
	class UnifiedRendererSettingsProvider : SettingsProvider {
		class Styles {
			public static GUIContent DisableOriginalEditorsContent = new GUIContent("Disable original editors", "For Mesh Renderer & Filter");
		}

		public UnifiedRendererSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path,
			scope) { }

		public override void OnActivate(string searchContext, VisualElement rootElement) {
			// This function is called when the user clicks on the SmartNSSettings element in the Settings window.
		}

		public override void OnGUI(string searchContext) {
			EditorGUILayout.LabelField("Version: 0.1.1");

			var disableOriginalEditors = EditorGUILayout.Toggle(Styles.DisableOriginalEditorsContent, PrefSet.DisableOriginalEditors);
			
			if (GUI.changed) {
				PrefSet.DisableOriginalEditors = disableOriginalEditors;
			}
		}

		[SettingsProvider]
		public static SettingsProvider CreateUnifiedRendererSettingsProvider() {
			var provider = new UnifiedRendererSettingsProvider("Project/Unified Renderer", SettingsScope.Project) {
				keywords = GetSearchKeywordsFromGUIContentProperties<Styles>()
			};

			// Automatically extract all keywords from the Styles.
			return provider;
		}
	}
}