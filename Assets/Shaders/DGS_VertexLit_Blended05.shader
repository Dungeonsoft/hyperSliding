Shader "DGS/VertexLit BlendedBG05" {
	Properties{
		_EmisColor("Color1", Color) = (.2,.2,.2,0)
		_MainTex("Texture", 2D) = "white" {}
		_EmisPower("Emissive Power", Range(0,5)) = 1
	}


	SubShader{
		Tags { "RenderType" = "Opaque"}

		CGPROGRAM
		#pragma surface surf Standard

		sampler2D _MainTex;
		float4 _EmisColor;
		float _EmisPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Emission = c.rgb * _EmisColor* _EmisPower;
		}
		ENDCG
	}
	Fallback "Legarcy Shaders/Transparent/VertexLit"
}