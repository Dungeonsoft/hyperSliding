Shader "DGS/Refraction01"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_UvSpeed("Uv Speed", Range(0.0, 20.0)) = 1
		_TexContrast("Contrast",float) = 1
	}
		SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
		LOD 200

		GrabPass{}

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _GrabTexture;
		sampler2D _MainTex;
		float _UvSpeed;
		float _TexContrast;

		struct Input {
			float2 uv_MainTex;
			float4 screenPos;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex + _Time.x* _UvSpeed) * _TexContrast;
			float2 screenUV = IN.screenPos.rgb / IN.screenPos.a;
			screenUV = float2(screenUV.r,1-screenUV.g);
			o.Emission = tex2D(_GrabTexture, screenUV + c.r * 0.1);
			o.Alpha = c.a;
		}
		ENDCG

	}
}
