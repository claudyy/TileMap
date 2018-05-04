// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/CellShadingTileCut" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[PerRendererData] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MyNormalMap("My Normal map", 2D) = "white" {}
		_Base    ("Base", Range(0,1)) = 0
        _Boost   ("Boost", Range(1,8)) = 4
        _Edge    ("Edge", Range(1,96)) = 32
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf Toon fullforwardshadows alpha:fade
		//#pragma surface surf Lambert finalcolor:tooncolor alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MyNormalMap;

		struct Input {
			float2 uv_MainTex;
			float3 worldPos;

		};

		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)
		 //float4 unity_4LightPosX0;
		void surf (Input IN, inout SurfaceOutput  o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_MyNormalMap, IN.uv_MainTex));
			float3 vertexLighting = fixed3(0,0,0);
			// Hack for outline: Make black pixels always black

		}

		 half4 LightingToon (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir); 
			NdotL = step(0.5,NdotL * atten * 2);
			//NdotL = clamp(NdotL+NdotL+NdotL+NdotL,0,1);
			fixed4 c;
			c.rgb = s.Albedo * _LightColor0.rgb * NdotL;
			c.rgb = s.Albedo * _LightColor0.rgb * NdotL;
			//c.rgb = s.Albedo;
			//c.rgb = lerp(fixed3(0,0,0),s.Albedo,1);
			c.a = s.Alpha;
			return c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
