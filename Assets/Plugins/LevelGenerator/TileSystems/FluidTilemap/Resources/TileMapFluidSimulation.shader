Shader "Hidden/TileMapFluidSimulation"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ChunkX("ChunkX",int ) = 0
		_ChunkY("ChunkY",int ) = 0
		_ChunkDirX("ChunkX",int ) = 0
		_ChunkDirY("ChunkY",int ) = 0
		_Dissipation("_dissipation", Range(0,3)) = .5
		_DirDissipation("dir dissipation", Range(0,1)) = .5
		_Minimum("Minimum flow",Range(0,.1)) = 0

		_DirTex ("Texture", 2D) = "white" {}
		_DirScale("dir scale",float) = 1
		_DirFactor("dir factor",Range(0,5)) = 1
		_DirSpeed("dir speed",float) = 1
		_DirOffsetYFactor("_PixelPerUnit",Range(0,1)) = 1
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
			const static int MAX_EMITER = 4;
			sampler2D _MainTex,_DirTex;
			float _Dissipation,_Minimum,_DirDissipation;
			float _DirScale,_DirFactor,_DirSpeed,_DirOffsetYFactor;
			int _ChunkX,_ChunkY;
			int _ChunkDirX,_ChunkDirY;
			uniform float4 _Emitter;
			uniform int _EmitterCount;
			uniform fixed4 _EmittersPos[MAX_EMITER];
			uniform fixed _EmittersSize[MAX_EMITER];
			uniform fixed _EmittersStrength[MAX_EMITER];
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

			float outBoundBlend(fixed2 uv,fixed2 offset){
				half s = 1 / _FluidPixelPerChunk;
				float po = s;//pick offset
				float sm= 1;//smooth
				float uvr =(smoothstep(1-po*sm,1,uv.x + offset.x));
				float uvl =(1 - smoothstep(0,po*sm,uv.x + offset.x));
				float uvt =(smoothstep(1-po*sm,1,uv.y + offset.y));
				float uvb =(1 - smoothstep(0,po*sm,uv.y + offset.y));
				return uvr+uvl+uvt+uvb;
			}
			float isChunk(fixed x,fixed y){
				float v = step(_ChunkDirX,x);
				v *= step(_ChunkDirY,y);
				v*= 1-step(_ChunkDirX+1,x);
				v *= 1-step(_ChunkDirY+1,y);
				return v;
			}
			float4 OutOfBoundColor(fixed2 uv,fixed2 offset){
				float right =isChunk(1,0);
				float rightTop =isChunk(1,1);
				float rightBottom =isChunk(1,-1);
				float left =isChunk(-1,0);
				float leftTop =isChunk(-1,1);
				float leftBottom =isChunk(-1,-1);
				float center =isChunk(0,0);
				float top =isChunk(0,1);
				float bottom =isChunk(0,-1);

				float4 col = float4(1,1,0,0);
				half s = 1 / _FluidPixelPerChunk;
				
				float po = s;//pick offset

				float uvr =(1-step(uv.x + offset.x,1-po));
				float uvl =(1 - step(po,uv.x + offset.x));
				float uvt =(1-step(uv.y + offset.y,1-po));
				float uvb =(1 - step(po,uv.y + offset.y));

				//right Chunk
				col = lerp(col,tex2D(_FluidCenterTex, float2(1-po,uv.y + offset.y)),right*uvl);
				col = lerp(col,tex2D(_FluidTopRightTex, float2(uv.x + offset.x,po)),right*uvt);
				col = lerp(col,tex2D(_FluidBottomRightTex, float2(uv.x + offset.x,1-po)),right*uvb);
				//right Top
				col = lerp(col,tex2D(_FluidTopTex, float2(1-po,uv.y + offset.y)),rightTop*uvl);
				col = lerp(col,tex2D(_FluidRightTex, float2(uv.x + offset.x,1-po)),rightTop*uvb);
				//left Chunk
				col = lerp(col,tex2D(_FluidCenterTex, float2(po,uv.y + offset.y)),left*uvr);
				col = lerp(col,tex2D(_FluidTopLeftTex, float2(uv.x + offset.x,po)),left*uvt);
				col = lerp(col,tex2D(_FluidBottomLeftTex, float2(uv.x + offset.x,1-po)),left*uvb);
				//left Top
				col = lerp(col,tex2D(_FluidTopTex, float2(po,uv.y + offset.y)),leftTop*uvr);
				col = lerp(col,tex2D(_FluidLeftTex, float2(uv.x + offset.x,1-po)),leftTop*uvb);
				//top Chunk
				col = lerp(col,tex2D(_FluidCenterTex, float2(uv.x + offset.x,1-po)),top*uvb);
				col = lerp(col,tex2D(_FluidTopRightTex, float2(po,uv.y + offset.y)),top*uvr);
				col = lerp(col,tex2D(_FluidTopLeftTex, float2(1-po,uv.y + offset.y)),top*uvl);
				//left bottom
				col = lerp(col,tex2D(_FluidBottomTex, float2(po,uv.y + offset.y)),leftBottom*uvr);
				col = lerp(col,tex2D(_FluidLeftTex, float2(uv.x + offset.x,po)),leftBottom*uvt);
				//right Bottom
				col = lerp(col,tex2D(_FluidBottomTex, float2(1-po,uv.y + offset.y)),rightBottom*uvl);
				col = lerp(col,tex2D(_FluidRightTex, float2(uv.x + offset.x,po)),rightBottom*uvt);
				//bottom Chunk
				col = lerp(col,tex2D(_FluidCenterTex, float2(uv.x + offset.x,po)),bottom*uvt);
				col = lerp(col,tex2D(_FluidBottomRightTex, float2(po,uv.y + offset.y)),bottom*uvr);
				col = lerp(col,tex2D(_FluidBottomLeftTex, float2(1-po,uv.y + offset.y)),bottom*uvl);

				//Center Chunke
				col = lerp(col,tex2D(_FluidRightTex, float2(po ,uv.y + offset.y)),center*uvr);
				col = lerp(col,tex2D(_FluidLeftTex, float2(1-po ,uv.y + offset.y)),center*uvl);
				col = lerp(col,tex2D(_FluidTopTex, float2(uv.x + offset.x,po)),center*uvt);
				col = lerp(col,tex2D(_FluidBottomTex, float2(uv.x+ offset.x,1-po)),center*uvb);
				

				return  col;
			}
			float4 GetColor(fixed2 uv,fixed2 offset){
				
				//return float4(0,0,0,0);
				return lerp(tex2D(_MainTex, uv + offset),OutOfBoundColor(uv,offset),outBoundBlend(uv,offset));
			}
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 uv = i.uv;
			
				half s = 1 / _FluidPixelPerChunk;


				// Neighbour cells
				float4 cl = GetColor(uv,fixed2(-s, 0));	// Centre Left
				cl.rg=(cl.rg - float2(.5,.5))*2;
				float4 tc = GetColor(uv,fixed2(0, s));	// Top Centre
				tc.rg=(tc.rg - float2(.5,.5))*2;
				float4 cc = GetColor(uv,fixed2(0, 0));	// Centre Centre
				cc.rg=(cc.rg - float2(.5,.5))*2;
				float4 bc = GetColor(uv,fixed2(0, -s));	// Bottom Centre
				bc.rg=(bc.rg - float2(.5,.5))*2;
				float4 cr = GetColor(uv,fixed2(+s, 0));	// Centre Right
				cr.rg=(cr.rg - float2(.5,.5))*2;

				//Obstalce
				float obsC = step(.9,cc.b);	
				float obsL = step(.9,cl.b);	
				float obsT = step(.9,tc.b);	
				float obsB = step(.9,bc.b);	
				float obsR = step(.9,cr.b);	

				float SmoothobsL = cl.b;
				float SmoothobsT = tc.b;
				float SmoothobsB = bc.b;
				float SmoothobsR = cr.b;

				//cc.b = 1;

				//dir
				float dirl =step(tc.a,cl.a)*step(bc.a,cl.a)*step(cr.a,cl.a) * step(obsC,.1);
				float dirt =step(cl.a,tc.a)*step(bc.a,tc.a)*step(cr.a,tc.a) * step(obsC,.1);
				float dirb =step(cl.a,bc.a)*step(tc.a,bc.a)*step(cr.a,bc.a) * step(obsC,.1);
				float dirr =step(cl.a,cr.a)*step(bc.a,cr.a)*step(tc.a,cr.a) * step(obsC,.1);

				//cl.rg = lerp(cl.rg,float2(1,0),obsL);
				//tc.rg = lerp(tc.rg,float2(0,-1),obsT);
				//bc.rg = lerp(bc.rg,float2(0,1),obsB);
				//cr.rg = lerp(cr.rg,float2(-1,0),obsR);

				float2 dirFactor = normalize(cl.rg * dirl+ tc.rg*dirt  + bc.rg*dirb + cr.rg*dirr);

				//Collision Dir
				dirFactor = lerp(dirFactor,float2(0,1), SmoothobsL * SmoothobsL* SmoothobsL);
				dirFactor = lerp(dirFactor,float2(1,0), SmoothobsT * SmoothobsT* SmoothobsT);
				dirFactor = lerp(dirFactor,float2(-1,0),SmoothobsB * SmoothobsB* SmoothobsB);
				dirFactor = lerp(dirFactor,float2(0,-1),SmoothobsR * SmoothobsR* SmoothobsR);

				cc.rg = dirFactor;

				//cc.b = 0;

				float2 dir = float2(cc.x,cc.y);

				float2 addDir = tex2D(_DirTex,frac(uv *_DirScale)+float2(frac(_Time.x),frac(_Time.x))*_DirSpeed);
				addDir=(addDir -.5)*2;
				dir=normalize(dir+addDir*_DirFactor*(1+cc.a)/2);

				float l =0.01 + 0.24 * smoothstep(.2,1,dot(dir,float2(-1, 0)));
				float t =0.01 + 0.24 * smoothstep(.2,1,dot(dir,float2( 0,-1)));
				float b =0.01 + 0.24 * smoothstep(.2,1,dot(dir,float2( 0, 1)));
				float r =0.01 + 0.24 * smoothstep(.2,1,dot(dir,float2( 1, 0)));


				l = lerp(l,0,obsL);
				t = lerp(t,0,obsT);
				b = lerp(b,0,obsB);
				r = lerp(r,0,obsR);

				// Diffusion step
				float factor = 
					(_Dissipation) *
					(
						(cl.a * l + tc.a * t + bc.a * b + cr.a * r)
						- cc.a *(l+t+b+r)
					);
				// Minimum flow
				if(factor >= -_Minimum && factor < 0.0)
					factor = -_Minimum;
				cc.a += factor;
				

				for(int i= 0; i < _EmitterCount;i++){
					float radius = _EmittersSize[i] * _FluidPixelPerTile;
					fixed2 emitPos = _EmittersPos[i].xy / _ChunkSize;
					float e = (1-smoothstep(0,s*radius,distance(emitPos,uv + fixed2(_ChunkX,_ChunkY)))) * (1-obsC);
					fixed2 emitDir = _EmittersPos[i].zw*fixed2(-1,1);
					cc = lerp(cc,fixed4(emitDir.x,emitDir.y,cc.b,cc.a+_EmittersStrength[i]),e);
				}
				
				cc = lerp(cc,fixed4(0,0,cc.b,0),obsC);
				cc.rg = (cc.rg + 1)/2;

				//float uvb =(1-step(0, uv.y -s));
				//float top =isChunk(0,1);
				//cc.a = outBoundBlend(uv,fixed2(0,0));
				return cc;	

			}
			ENDCG
		}
	}
}
