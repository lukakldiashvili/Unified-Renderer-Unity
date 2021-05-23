using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Unify.UnifiedRenderer.Serializables {
	[Serializable]
	public class SerializableColor {
		public float r;
		public float g;
		public float b;
		public float a;

		public Color GetColor => new Color(r, g, b, a);

		public Color SetColor {
			set {
				r = value.r;
				g = value.g;
				b = value.b;
				a = value.a;
			}
		}

		public SerializableColor(Color inColor) {
			SetColor = inColor;
		}
	}
	
	[Serializable]
	public class SerializableVector {
		public float x;
		public float y;
		public float z;
		public float w;

		public Vector4 GetVector => new Vector4(x, y, z, w);

		public Vector4 SetVector {
			set {
				x = value.x;
				y = value.y;
				z = value.z;
				w = value.w;
			}
		}

		public SerializableVector(Vector4 inVector) {
			SetVector = inVector;
		}
	}
	
	[Serializable]
	public class SerializableTexture {
		public SerializationData data;
		public Texture GetTexture => (Texture) Serialization.Deserialize(data);

		public Texture SetTexture {
			set {
				data = value.Serialize();
			}
		}

		public SerializableTexture(Texture inTexture) {
			SetTexture = inTexture;
		}
	}
}