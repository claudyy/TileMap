uniform fixed4 _SkyGroundColor,_SkyHorizonColor,_SkyColor,_FogColor;
		uniform fixed4 _SkyNightGroundColor,_SkyNightHorizonColor,_SkyNightColor,_FogNightColor;
		uniform float _SkyGroundPos,_SkyGroundBlend,_SkyHorizonPos,_SkyHorizonBlend,_SkyHorizonSize;
		uniform float _SkyTime,_Depth,_DayTime,_FogBlend;
		
		fixed4 GetDaySkyColor(fixed2 pos){
			fixed4 s =_SkyColor;

			float hblend = smoothstep(_SkyHorizonPos - _SkyHorizonBlend,_SkyHorizonPos + _SkyHorizonBlend, pos.y + _SkyHorizonSize);
			hblend *= 1-smoothstep(_SkyHorizonPos - _SkyHorizonBlend,_SkyHorizonPos + _SkyHorizonBlend, pos.y - _SkyHorizonSize);
			s=lerp(s,_SkyHorizonColor,hblend);

			float gblend = smoothstep(_SkyGroundPos,_SkyGroundPos + _SkyGroundBlend,pos.y);
			s=lerp(s,_SkyGroundColor,(1-gblend));

			return s;
		}
		fixed4 GetSkyNightColor(fixed2 pos){
			fixed4 s =_SkyNightColor;

			float hblend = smoothstep(_SkyHorizonPos - _SkyHorizonBlend,_SkyHorizonPos + _SkyHorizonBlend, pos.y + _SkyHorizonSize);
			hblend *= 1-smoothstep(_SkyHorizonPos - _SkyHorizonBlend,_SkyHorizonPos + _SkyHorizonBlend, pos.y - _SkyHorizonSize);
			s=lerp(s,_SkyNightHorizonColor,hblend);

			float gblend = smoothstep(_SkyGroundPos,_SkyGroundPos + _SkyGroundBlend,pos.y);
			s=lerp(s,_SkyNightGroundColor,1-gblend);

			return s;
		}
		fixed4 GetSkyColor(fixed2 pos){
			return GetDaySkyColor(pos);
		}
		fixed4 GetSkyColor(fixed2 pos,float fog){
			fixed4 c = GetDaySkyColor(pos);
			return lerp(c,_FogColor,fog);
		}
		half DepthBlend(fixed3 pos){
			return clamp(pos.z / _Depth, 0, 1);
		}