﻿
// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/NormalMappedTile" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[PerRendererData] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MyNormalMap("My Normal map", 2D) = "white" {}
		_Metallic("Metalic",Range(0,1)) = 0
		_Smoothness("Smoothness",Range(0,1)) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MyNormalMap;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;
		half _Metallic,_Smoothness;
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(mul(unity_ObjectToWorld,tex2D(_MyNormalMap, IN.uv_MainTex)));
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			//o.Normal = UnpackNormal(tex2D(_MyNormalMap, IN.uv_MainTex));
			//o.Albedo = fixed3(o.Normal.x,o.Normal.y,0);
			// Hack for outline: Make black pixels always black
			if(length(c.rgb)<0.001)
			{
				o.Normal = fixed3(0,0,-1);
				o.Albedo = fixed3(0,0,0);
			}
		}
		ENDCG
	}
	FallBack "Diffuse"
}
