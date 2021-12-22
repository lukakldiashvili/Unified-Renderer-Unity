using UnityEditor;

public static class UnifiedRendererSettings {
	
	public static class PrefTags {
		public const string DisableOriginalEditorsBoolTag = "DisableOriginalEditorsTag";
	}

	public static bool DisableOriginalEditors {
		get => EditorPrefs.GetBool(PrefTags.DisableOriginalEditorsBoolTag);
		set => EditorPrefs.SetBool(PrefTags.DisableOriginalEditorsBoolTag, value);
	}
	
}