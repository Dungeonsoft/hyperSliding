Shader "DGS/VertexLit BlendedBG04" {
	Properties{
		_EmisColor("Color1", Color) = (.2,.2,.2,0)
		_EmisColor2("Color2", Color) = (.2,.2,.2,0)
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
		float4 _EmisColor2;
		float _EmisPower;
		float _AlphaPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			//o.Emission = (((c * _EmisColor) + ((-1 * (c - 0.5) + 0.5) * _EmisColor2)) * c * _EmisPower);
			//o.Emission = (-1 * (c - 0.5) + 0.5) * _EmisColor2;
			//o.Emission = c * _EmisColor;
			o.Emission = ((c * _EmisColor + ((-1 * c + 1) * _EmisColor2)) - 0.4 )* c.r * c.r* _EmisPower+(c.r * c.r * c.r * _EmisPower);
			//o.Emission = c * _EmisColor * _EmisPower;
			o.Alpha = _AlphaPower * c.r;
			//o.Alpha = _AlphaPower;
		}
		ENDCG
	}
	Fallback "Legarcy Shaders/Transparent/VertexLit"
}