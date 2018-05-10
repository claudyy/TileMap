Shader "Hidden/PixelDrawer"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_ChunckSize("chunck Size",Float) = 40
		_PosX("pos x",Float) = 40
		_PosY("pos y",Float) = 40
		_Color("color",Color) = (1,1,1,1)
		_Target("Target Texuter", 2D) = "white"{}
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
			
			sampler2D _MainTex;
			float _PosX,_PosY,_ChunckSize;
			fixed4 _Color;
			float square(fixed2 st,fixed2 centre,float xSize,float ySize){
				float square =  step(0,st.x-centre.x);
			    square *= 1. - step(xSize,st.x-centre.x);
				square *=  step(0,st.y-centre.y);
			    square *= 1. - step(ySize,st.y-centre.y);
			    return square;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				fixed2 pos = fixed2(_PosX / _ChunckSize,(_PosY)/ _ChunckSize);
				fixed l = square(i.uv,pos,1/_ChunckSize,1/_ChunckSize);
				col = lerp(col,_Color,l);
				return col;
			}
			ENDCG
		}
	}
}
