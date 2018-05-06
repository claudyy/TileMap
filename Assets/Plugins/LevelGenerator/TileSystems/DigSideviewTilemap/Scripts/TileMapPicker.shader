// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/Tilemap/Picker"
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

		_1PickColor("1 Picker",Color) = (1,1,1,1)
		_1Size("1 size",Float) = 5
		_1Tex("1 Texture", 2D) = "white" {}
		_2PickColor("2 Picker",Color) = (1,1,1,1)
		_2Size("2 size",Float) = 5
		_2Tex("2 Texture", 2D) = "white" {}
		_3PickColor("3 Picker",Color) = (1,1,1,1)
		_3Size("3 size",Float) = 5
		_3Tex("3 Texture", 2D) = "white" {}
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

			Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile DUMMY PIXELSNAP_ON
#include "UnityCG.cginc"

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

			return OUT;
		}

		sampler2D _MainTex,_1Tex,_2Tex,_3Tex;
		float _1Size,_2Size,_3Size;
		fixed3 _1PickColor,_2PickColor,_3PickColor;
		float isColor(fixed3 c1 ,fixed3 c2){
			float is =(1-step(c1.r,c2.r-.1)) *(1-step(c1.g,c2.g-.1)) * (1-step(c1.b,c2.b-.1)); 
			return is;
		}
		fixed3 ColorPicker(fixed4 c,fixed4 oc,fixed2 pos,fixed3 pickCol,sampler2D tex,float size){
			float picker = isColor(oc,pickCol);
			fixed4 pickerTex = tex2D(tex, frac(pos/size))*_Color;
			return lerp(c.rgb,pickerTex.rgb,picker);
		}
		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 c = tex2D(_MainTex, IN.texcoord) * _Color;
			fixed4 oc = tex2D(_MainTex, IN.texcoord);

			c.rgb = ColorPicker(c,oc,IN.cPos.xy,_1PickColor,_1Tex,_1Size);
			c.rgb = ColorPicker(c,oc,IN.cPos.xy,_2PickColor,_2Tex,_2Size);
			c.rgb = ColorPicker(c,oc,IN.cPos.xy,_3PickColor,_3Tex,_3Size);



			c.rgb *= c.a;
			return c;
		}
			ENDCG
		}
		}
}