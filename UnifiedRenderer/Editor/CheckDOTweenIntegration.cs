using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Unify.UnifiedRenderer.DOTween.Editor {
	[InitializeOnLoad]
	public class CheckDOTweenIntegration : UnityEditor.Editor {
		public const string UNIFY_DOTWEEN_SYMBOL = "UNIFY_DOTWEEN";

		static bool DOTweenExists => (from assembly in AppDomain.CurrentDomain.GetAssemblies()
		                              from type in assembly.GetTypes()
		                              where type.FullName != null && type.FullName.Contains("DG.Tweening.DOTween")
		                              select type).FirstOrDefault() != null;

		static CheckDOTweenIntegration() {
			if (!EditorApplication.isCompiling) {
				WriteDefines();
			}
		}

		private static void WriteDefines(bool forceRemove = false) {
			string defines =
				PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
			List<string> allDefines = defines.Split(';').ToList();

			bool defineExists  = defines.Contains(UNIFY_DOTWEEN_SYMBOL);
			bool dotweenExists = DOTweenExists;

			if (!defineExists && dotweenExists) {
				allDefines.Add(UNIFY_DOTWEEN_SYMBOL);
				
				Debug.Log("Unified Renderer: Automatically enabled DOTween Integration");
			}
			else if ((!dotweenExists || forceRemove) && defineExists) {
				allDefines.Remove(UNIFY_DOTWEEN_SYMBOL);
				
				Debug.Log("Unified Renderer: Automatically removed DOTween Integration");
			}

			var updatedDefines = string.Join(";", allDefines.ToArray());

			if (updatedDefines != defines) {
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
					updatedDefines);
			}
		}

		private class DotweenModificationProcessor : AssetModificationProcessor {
			private static AssetDeleteResult OnWillDeleteAsset(string asset, RemoveAssetOptions options) {
				string fullPath = Application.dataPath.Replace("Assets", string.Empty) + asset;
				
				if (!Directory.Exists(fullPath))
					return 0;
				
				string[] deletedFiles  = Directory.GetFiles(fullPath, "DOTween.dll", SearchOption.AllDirectories);
				
				bool     foundDotweenDLL   = false;
				
				foreach (var filePath in deletedFiles) {
					if (filePath.EndsWith("DOTween.dll")) {
						foundDotweenDLL = true;
						break;
					}
				}
				if (!foundDotweenDLL)
					return 0;
				
				WriteDefines(true);
				
				return 0;
			}
		}
	}
}