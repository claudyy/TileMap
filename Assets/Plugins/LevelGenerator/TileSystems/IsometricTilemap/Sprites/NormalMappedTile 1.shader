// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Custom/NormalMappedTileCut" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[PerRendererData] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MyNormalMap("My Normal map", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		Cull Off

		CGPROGRAM
		#pragma surface surf Toon fullforwardshadows alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _MyNormalMap;

		struct Input {
			float2 uv_MainTex;
		};

		fixed4 _Color;

		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput  o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_MyNormalMap, IN.uv_MainTex));
			// Hack for outline: Make black pixels always black
			

		}
		 half4 LightingToon (SurfaceOutput s, half3 lightDir, half atten) {
			half NdotL = dot(s.Normal, lightDir); 
			NdotL = step(0.5,NdotL * atten * 2);
			
			fixed4 c;
			//c.rgb = s.Albedo * _LightColor0.rgb * NdotL;
			c.rgb = s.Albedo* NdotL;
			c.a = s.Alpha;
			return c;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
