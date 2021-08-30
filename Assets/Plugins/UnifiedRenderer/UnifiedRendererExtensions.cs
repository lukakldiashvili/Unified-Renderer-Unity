using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Unify.UnifiedRenderer {

	public static class UnifiedRendererExtensions {
		public static MaterialPropertyNameType GetDefaultIfAuto(this MaterialPropertyNameType nameType) {
			return nameType == MaterialPropertyNameType.AUTO ? 
				(UnifiedRenderer.UseDisplayPropertyName ? MaterialPropertyNameType.DISPLAY : MaterialPropertyNameType.INTERNAL) : nameType;
		}
		
		public static string GetNameWithType(this MaterialPropertyData property, MaterialPropertyNameType nameType) {
			nameType = nameType.GetDefaultIfAuto();
			
			if (nameType == MaterialPropertyNameType.INTERNAL) return property.GetInternalName;
			return property.GetDisplayName;
		}
	}
}