Shader "DGS/VertexLit BlendedBG02" {
	Properties{
		_EmisColor("Color", Color) = (.2,.2,.2,0)
		_MainTex("Texture", 2D) = "white" {}
		_EmisPower("Emissive Power", Range(0,20)) = 1
		_AlphaPower("Alpha Power", Range(0,5)) = 1
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
		float _AlphaPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Emission = c.rgb* _EmisColor* _EmisPower;
			//o.Alpha = c.a * c.a * _EmisPower;
			o.Alpha = _AlphaPower*c.r;
		}
		ENDCG
	}
	Fallback "Legarcy Shaders/Transparent/VertexLit"
}