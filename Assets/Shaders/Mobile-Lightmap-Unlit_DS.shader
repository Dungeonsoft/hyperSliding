// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible textured shader.
// - SUPPORTS lightmap
// - no lighting
// - no per-material color

Shader "Mobile/DS/Unlit (Supports Lightmap)" {
	Properties{
		_Color("Tint", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 100

		// Non-lightmapped
		Pass {
			Tags { "LightMode" = "Vertex" }
			Lighting Off

			SetTexture[_MainTex] {
				constantColor(1,1,1,1)
				combine texture, constant // UNITY_OPAQUE_ALPHA_FFP
			}
		}
	}
}
