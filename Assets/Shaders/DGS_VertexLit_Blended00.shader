Shader "DGS/VertexLit BlendedBG00" {
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
		//#pragma multi_compile FEATURE_OFF FEATURE_ON

		sampler2D _MainTex;
		float4 _EmisColor;
		float _EmisPower;
		float _AlphaPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Emission = c.rgb * _EmisColor * _EmisPower;
			float g = (o.Emission.r * o.Emission.g * o.Emission.b) / 3;
			o.Alpha = _AlphaPower * (-1*(g-0.5)+0.5);
			//o.Emission = o.Alpha;
			//0.Alpha = 0;
		}
		ENDCG
	}
	Fallback "Legarcy Shaders/Transparent/VertexLit"
}