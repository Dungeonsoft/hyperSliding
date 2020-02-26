// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified VertexLit Blended Particle shader. Differences from regular VertexLit Blended Particle one:
// - no AlphaTest
// - no ColorMask

Shader "DGS/VertexLit Blended" {
	Properties{
		_EmisColor("Color", Color) = (.2,.2,.2,0)
		_MainTex("Texture", 2D) = "white" {}
		_EmisPower("Emissive Power", Range(0,10)) = 1
	}


	SubShader{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}
		//blend SrcAlpha OneMinusSrcAlpha
		blend SrcAlpha One

		CGPROGRAM
		#pragma surface surf Standard alpha:fade

		sampler2D _MainTex;
		float4 _EmisColor;
		float _EmisPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			fixed3 c2 = c.rgb * _EmisColor.rgb * _EmisPower;
			o.Emission = c2+ c.rgb*(_EmisPower/4);
			//o.Alpha = c.a * c.a * _EmisPower;
			o.Alpha = c2.r* c2.r;
		}
		ENDCG
	}
	Fallback "Legarcy Shaders/Transparent/VertexLit"
}