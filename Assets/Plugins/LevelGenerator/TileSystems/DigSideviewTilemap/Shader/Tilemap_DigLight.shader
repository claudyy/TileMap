Shader "Sprites/Tilemap/DigLight"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
		[HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
		[HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
		[PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
		[PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0
		_SunLight("sun light blend",Range(0,1))=1


	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile DUMMY PIXELSNAP_ON
#include "UnityCG.cginc"
#include "/Assets/Plugins/LevelGenerator/Scripts/Shader/TilemapLib.cginc"
			struct appdata_t
		{
			float4 vertex : POSITION;
			float4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f
		{
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			half2 texcoord : TEXCOORD0;
			float3 cPos : TEXCOORD1;
			float4 uvgrab : TEXCOORD2;

		};

		fixed4 _Color;
		float _SunLight;
		v2f vert(appdata_t IN)
		{
			v2f OUT;
			OUT.vertex = UnityObjectToClipPos(IN.vertex);
			OUT.texcoord = IN.texcoord;
			OUT.color = IN.color;
#ifdef PIXELSNAP_ON
			OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
			OUT.cPos = mul(unity_ObjectToWorld, IN.vertex);
			OUT.uvgrab = ComputeGrabScreenPos(OUT.vertex);
			return OUT;
		}

		sampler2D _MainTex;
		
		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 c = tex2D(_MainTex, IN.texcoord);
			float blendSunLight = GetBlendedZ(IN.cPos);
			float blendLight = GetBlendedW(IN.cPos);

			blendLight=lerp(blendLight,1,blendSunLight*_SunLight);

			c.a = blendLight;
			c.rgb *= c.a;
			return c;
		}
			ENDCG
		}
		}
}
