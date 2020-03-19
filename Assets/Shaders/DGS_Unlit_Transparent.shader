Shader "DGS/Unlit Transparent"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_MainColor ("Color",Color) = (1,1,1,1)
		_AlphaPower("Alpha Power", Range(0,1)) = 1
    }
    SubShader
    {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent"}

	    CGPROGRAM
		#pragma surface surf Lambert alpha:fade

        sampler2D _MainTex;
		float4 _MainColor;
		float _AlphaPower;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb* _MainColor;
			o.Alpha = c.a * _AlphaPower;
        }
        ENDCG
    }
}