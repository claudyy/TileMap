Shader "Hidden/TileMapFluidCollisionDrawer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_CollisionTex ("Texture", 2D) = "white" {}
		_CollisionTilePosX("Pos x",int) = 0
		_CollisionTilePosY("Pos x",int) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex,_CollisionTex;
			uniform float _ChunkSize;
			int _CollisionTilePosX,_CollisionTilePosY;
			uniform fixed _FluidPixelPerChunk,_FluidPixelPerTile;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed tileSize = 1/_ChunkSize;
				fixed2 tilePos = fixed2(_CollisionTilePosX,_CollisionTilePosY)*tileSize;
				float inTile = step(tilePos.x,i.uv.x);
				inTile *= step(tilePos.y,i.uv.y);
				inTile *= 1-step(tilePos.x+tileSize,i.uv.x);
				inTile *= 1-step(tilePos.y+tileSize,i.uv.y);

				fixed tileCol = tex2D(_CollisionTex,frac(i.uv*_ChunkSize)).a;

				col = lerp(col,fixed4(0,0,tileCol,0),inTile);

				return col;
			}
			ENDCG
		}
	}
}
