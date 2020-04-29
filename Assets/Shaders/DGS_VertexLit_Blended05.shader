Shader "DGS/VertexLit BlendedBG05" {
	Properties{
		_EmisColor("Color1", Color) = (.2,.2,.2,0)
		_MainTex("Texture", 2D) = "white" {}
		_GradTex("Grad Texture",2D) = "white" {}
		_EmisPower("Emissive Power", Range(0,5)) = 1
	}


	SubShader{
		Tags { "RenderType" = "Opaque"}

		CGPROGRAM
		#pragma surface surf Standard

		sampler2D _MainTex;
		sampler2D _GradTex;
		float4 _EmisColor;
		float _EmisPower;

		struct Input {
			float2 uv_MainTex;
			float2 uv_GradTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c1 = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c2 = tex2D(_GradTex, IN.uv_GradTex);
			o.Emission = c1.rgb * _EmisColor* _EmisPower*c2;
		}
		ENDCG
	}
	Fallback "Legarcy Shaders/Transparent/VertexLit"
}