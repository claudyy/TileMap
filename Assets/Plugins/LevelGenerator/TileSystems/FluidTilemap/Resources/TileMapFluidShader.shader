Shader "Unlit/TileMapFluidShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SmokeColor("Smoke Color", Color) = (1,1,1,1)
		_SmokeFadeColor("Smoke Fade Color", Color) = (1,1,1,1)
		_DetailTex ("Texture", 2D) = "white" {}
		_DetailBlend("Detail Blend",Range(0,1)) = 1
		_AddDetailTex ("Add Texture", 2D) = "white" {}
		_AddDetailBlend("addDetail Blend",Range(0,1)) = 1
		_DisplayDetailTex ("Add Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 worldPos :TEXCOORD1;
			};

			sampler2D _MainTex,_DetailTex,_AddDetailTex,_DisplayDetailTex;
			float4 _MainTex_ST;
			fixed4 _SmokeColor,_SmokeFadeColor;
			fixed _DetailBlend,_AddDetailBlend;
			uniform sampler2D _FluidCenterTex;
			uniform sampler2D _FluidLeftTex;
			uniform sampler2D _FluidTopTex;
			uniform sampler2D _FluidRightTex;
			uniform sampler2D _FluidBottomTex;
			uniform sampler2D _FluidTopLeftTex;
			uniform sampler2D _FluidTopRightTex;
			uniform sampler2D _FluidBottomRightTex;
			uniform sampler2D _FluidBottomLeftTex;
			uniform fixed _FluidPixelPerChunk,_FluidPixelPerTile;
			uniform float4 _ChunckCenterPos;
			uniform float _ChunkSize;
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul (unity_ObjectToWorld, v.vertex);
				return o;
			}
			float InChunk(float2 pos,float2 chunk){
				float v = step(chunk.x,floor(pos).x/_ChunkSize);
				v *= step(chunk.y,floor(pos).y/_ChunkSize);
				v*= 1-step(chunk.x+1,floor(pos).x/_ChunkSize);
				v *= 1-step(chunk.y+1,floor(pos).y/_ChunkSize);
				return v;
			}
			fixed4 ChunkColor(float2 pos,in fixed4 OrignalCol,float2 chunkPos,sampler2D chunkTex){
				float2 inChunkPos = frac((pos)/_ChunkSize);
				float inChunk = InChunk(pos,chunkPos);
				fixed4 color = tex2D(chunkTex, inChunkPos);
				return lerp(OrignalCol,color,inChunk);
			}
			fixed4 TilesColor(float2 pos){
				fixed4 OrignalCol = fixed4(0,0,0,0);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(0,0),  _FluidCenterTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(-1,0), _FluidLeftTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(0,1),  _FluidTopTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(1,0),  _FluidRightTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(0,-1), _FluidBottomTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(-1,1), _FluidTopLeftTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(1,1),  _FluidTopRightTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(1,-1), _FluidBottomRightTex);
				OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(-1,-1),_FluidBottomLeftTex);
				return OrignalCol;
			}
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = TilesColor(i.worldPos);
				fixed inner = smoothstep(0,.5,col.a);
				fixed outer = (1-smoothstep(.2,.7,col.a));
				fixed edge = smoothstep(0,.5,col.a)*(1-smoothstep(.2,.7,col.a));
				edge= 1-pow(1-edge,4);
				fixed2 displace = (tex2D(_DisplayDetailTex,frac(i.worldPos/32)+_Time.x).xy -.5)*2;
				displace*=0.1;

				fixed detailSize = .4;
				fixed detailBlend = _DetailBlend * edge;
				
				fixed detail = tex2D(_DetailTex,frac(i.worldPos*detailSize)+displace).r;
				detail *=tex2D(_DetailTex,frac(fixed2(i.worldPos.x*-1,i.worldPos.y)*detailSize/2+_Time.x*2) -displace).b;

				col.a=lerp(col.a,col.a*detail,detailBlend);

				fixed addDetail = tex2D(_AddDetailTex,frac(i.worldPos/8+_Time.x*displace/12)+displace).r;
				fixed addDetailBlend = _AddDetailBlend;
				col.a+=addDetail*inner*addDetailBlend;

				col.rgb =lerp(_SmokeFadeColor,_SmokeColor,col.a);
				return col;
			}
			ENDCG
		}
	}
}
