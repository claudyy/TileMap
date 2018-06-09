Shader "Sprites/Tilemap/Water"
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
		_Wave("wave", 2D) = "white" {}
		_WaveCut("WaveCut",Float)=.5
		_WaveHeight("wave height",Float)=.5
		_WaveSpeed("WaveSpeed",Float) = 1
		
		_WaterColor("Water Color",Color) = (0,0,1,1)
		_RefBlend("blend reflection with water color",Float) = .5

		_EdgeCut("edge color cut",Float) = .2
		_EdgeColor("blend reflection with water color",Color) = (0,0,1,1)

		_Normal("nomal map", 2D) = "bump" {}
		_Magnitude("Magnitude", Range(0,1)) = 0.05

		_WaveSmooth("WaveSmooth",Float)=.1
		_WaveSize("WaveSize",Vector)= (12,10,8,6)

	}

		SubShader
		{
			Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}

			Cull Off
			Lighting Off
			ZWrite Off
			Fog{ Mode Off }
			Blend One OneMinusSrcAlpha
			GrabPass{ "_GrabTexture" }
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
		sampler2D _Wave;
		float _WaveCut;
		float _WaveSmooth;
		fixed4 _WaveSize;
		fixed _WaveSpeed,_WaveHeight;

		sampler2D _GrabTexture,_Normal;
		fixed _RefBlend,_EdgeCut,_Magnitude;
		fixed3 _WaterColor,_EdgeColor;
		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 c = tex2D(_MainTex, IN.texcoord);

			float blend = GetBlendedY(IN.cPos);


			fixed cut =_WaveCut+(.5+sin(IN.cPos.x+_Time.z*_WaveSpeed)/2)*_WaveHeight;
			c.a = step(cut,blend);
			
			float edge = 1-smoothstep(cut-_EdgeCut,cut+_EdgeCut,blend);
			fixed3 waterCol = lerp(_WaterColor,_EdgeColor,edge);

			float4 refUV = UNITY_PROJ_COORD(IN.uvgrab);
			half4 bump = tex2D(_Normal, frac(IN.cPos / 4+_Time.x*2));
			half2 distortion = UnpackNormal(bump).rg;
			refUV.xy += distortion * _Magnitude;
			c.rgb =lerp(waterCol, tex2Dproj(_GrabTexture, refUV ).rgb, _RefBlend);


			c.rgb *= c.a;
			return c;
		}
			ENDCG
		}
		}
}
