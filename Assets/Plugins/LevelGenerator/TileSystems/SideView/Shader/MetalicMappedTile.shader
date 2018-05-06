
// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "Sprite/Tilemap/Metalic" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		[PerRendererData] _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_MyNormalMap("My Normal map", 2D) = "white" {}
		_MyMetallicMap("My Metalic map", 2D) = "white" {}
		_MySmoothnessMap("My Smoothness map", 2D) = "white" {}
		_MyEmissionMap("My Emission map", 2D) = "black" {}
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
		sampler2D _MyNormalMap,_MyMetallicMap,_MySmoothnessMap,_MyEmissionMap;

		struct Input {
			float2 uv_MainTex;
			float2 screenuv;
			float4 screenPos;
		};
		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.screenuv = ((v.vertex.xy / v.vertex.w) + 1) * 0.5;

		}
		fixed4 _Color;
		half _Metallic,_Smoothness;
		UNITY_INSTANCING_BUFFER_START(Props)
		UNITY_INSTANCING_BUFFER_END(Props)

		uniform sampler2D _GlobalRefractionTex;
		uniform float _GlobalVisibility;
		uniform float _GlobalRefractionMag;

		float2 safemul(float4x4 M, float4 v)
		{
			float2 r;

			r.x = dot(M._m00_m01_m02, v);
			r.y = dot(M._m10_m11_m12, v);

			return r;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;

			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
			//screenUV *= float2(8,6);
			float2 offset = safemul(unity_ObjectToWorld, tex2D(_MyNormalMap, IN.uv_MainTex) * 2 - 1);
			float4 worldRefl = tex2D(_GlobalRefractionTex, screenUV + offset.xy * _GlobalRefractionMag);
			c.rgb+=worldRefl.rgb * worldRefl.a * _GlobalVisibility;

			o.Albedo = c.rgb;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(mul(unity_ObjectToWorld,tex2D(_MyNormalMap, IN.uv_MainTex)));
			o.Metallic = tex2D(_MyMetallicMap, IN.uv_MainTex);
			o.Smoothness =  tex2D(_MySmoothnessMap, IN.uv_MainTex);
			o.Emission = tex2D(_MyEmissionMap, IN.uv_MainTex);

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
