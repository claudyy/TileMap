// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/FogSprite"
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
		_WaveSmooth("WaveSmooth",Float)=.1
		_WaveSize("WaveSize",Vector)= (12,10,8,6)
		_WaveSpeed("WaveSpeed",Vector) = (1,1,1,1)
		_PulletColor("PullotedColor",Color) = (1,1,1,1)
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

		sampler2D _MainTex;
		sampler2D _Wave;
		float _WaveCut;
		float _WaveSmooth;
		fixed4 _WaveSize;
		fixed4 _WaveSpeed;
		fixed4 _PulletColor;
		fixed4 frag(v2f IN) : SV_Target
		{
			fixed4 c = tex2D(_MainTex, IN.texcoord);
			if(IN.color.r != 1 || IN.color.g != 1 || IN.color.b != 1 ||IN.color.a != 1){
				float min = .01;
				c.a = clamp(c.a,min,1);
				
				float originalA = c.a;
				Float cutL = IN.color.r + min;
				Float cutR = IN.color.g + min;
				Float cutH = IN.color.b + min;
				Float cutO = IN.color.a;
				Float cutX = lerp(cutL ,cutR,frac(IN.cPos.x));
				Float cutY = cutH;
				c.a = lerp(c.a,c.a*cutX*cutY,0);
				//c.a = 1-step(c.a,1-cutX);
				c.a = smoothstep(1-cutX,1-cutX+.06,c.a);
				c.a = clamp(c.a,0,1);

				
				float outline = originalA;
				float smooth =  .16;
				//outline = 1-smoothstep(1-cutX-smooth,1-cutX+smooth,outline);
				//c.rgb+=clamp(outline,0,.1);

				//float full = smoothstep(0.0,1,cutO);


				//c.rgb = full;
				//c.rgb = cutX;
				outline = originalA;
				smooth =  1.16;
				outline = 1-smoothstep(1-cutX-smooth,1-cutX+smooth,outline);
				float wave = tex2D(_Wave, fixed2(IN.cPos.x+_Time.x*_WaveSpeed.x,IN.cPos.y+_Time.x*_WaveSpeed.y)*_WaveSize.x);
				wave=1-lerp(0,wave,outline);
				wave=smoothstep(_WaveCut-_WaveSmooth,_WaveCut+_WaveSmooth,wave);
				float wave2 = tex2D(_Wave, fixed2(IN.cPos.x-_Time.x*_WaveSpeed.z,IN.cPos.y-_Time.x*_WaveSpeed.w)*_WaveSize.y);
				wave2=1-lerp(0,wave2,outline);
				wave2=smoothstep(_WaveCut-_WaveSmooth,_WaveCut+_WaveSmooth,wave2)*2;
				c.a *= clamp(wave2*wave,0,1);

				float pulloted = step(frac(IN.cPos.y),cutO);
				c.rgb = lerp(c.rgb,_PulletColor,pulloted);
			}
			c.rgb *= c.a;
			return c;
		}
			ENDCG
		}
		}
}