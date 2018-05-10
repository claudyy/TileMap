
uniform sampler2D _ChunckCenterTex;
uniform sampler2D _ChunckLeftTex;
uniform sampler2D _ChunckTopTex;
uniform sampler2D _ChunckRightTex;
uniform sampler2D _ChunckBottomTex;
uniform sampler2D _ChunckTopLeftTex;
uniform sampler2D _ChunckTopRightTex;
uniform sampler2D _ChunckBottomRightTex;
uniform sampler2D _ChunckBottomLeftTex;
uniform float4 _ChunckCenterPos;
uniform float _ChunkSize;
float blendPoints(fixed2 st,float4 points) {
	fixed2 f = frac(st/2);
	float a = points.x;//Left
	float b = points.y;//Right
	float c = points.z;//TopLeft
	float d = points.w;//TopRight


	fixed2 u = smoothstep(0.,1.,f);
	return lerp(a, b, u.x) +
	        (c - a)* u.y * (1.0 - u.x) +
	        (d - b) * u.x * u.y;
}
float InChunk(float2 pos,float2 chunk){
	float v = step(chunk.x,floor(pos).x/_ChunkSize);
	v *= step(chunk.y,floor(pos).y/_ChunkSize);
	v*= 1-step(chunk.x+1,floor(pos).x/_ChunkSize);
	v *= 1-step(chunk.y+1,floor(pos).y/_ChunkSize);
	return v;
}
fixed4 ChunkColor(float2 pos,in out fixed4 OrignalCol,float2 chunkPos,sampler2D chunkTex){
	float2 inChunkPos = frac(floor(pos+fixed2(0,0))/_ChunkSize);
	float inChunk = InChunk(pos,chunkPos);
	fixed4 color = tex2D(chunkTex, inChunkPos);
	return lerp(OrignalCol,color,inChunk);
}
fixed4 TilesColor(float2 pos){
	fixed4 OrignalCol = fixed4(0,0,0,0);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(0,0),_ChunckCenterTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(-1,0),_ChunckLeftTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(0,1),_ChunckTopTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(1,0),_ChunckRightTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(0,-1),_ChunckBottomTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(-1,1),_ChunckTopLeftTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(1,1),_ChunckTopRightTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(1,-1),_ChunckBottomRightTex);
	OrignalCol = ChunkColor(pos,OrignalCol,_ChunckCenterPos.xy+float2(-1,-1),_ChunckBottomLeftTex);
	return OrignalCol;
}
float blendTile(float2 pos,float ce,float4 s,float4 c){
	float final=0;
	float finaltl = blendPoints(pos*4,float4(s.x,ce,c.x,s.y));
	float mtl = 1-step(frac(pos.x),.5)*step(1-frac(pos.y),.5);
	final = lerp(finaltl,final,mtl);

	float finaltr = blendPoints(pos*4,float4(ce,s.z,s.y,c.y));
	float mtr = 1-step(1-frac(pos.x),.5)*step(1-frac(pos.y),.5);
	final = lerp(finaltr,final,mtr);

	float finalbl = blendPoints(pos*4,float4(c.w,s.w,s.x,ce));
	float mbl = 1-step(frac(pos.x),.5)*step(frac(pos.y),.5);
	final = lerp(finalbl,final,mbl);

	float finalbr = blendPoints(pos*4,float4(s.w,c.z,ce,s.z));
	float mbr = 1-step(1-frac(pos.x),.5)*step(frac(pos.y),.5);
	final = lerp(finalbr, final, mbr);
	return final;
}
float GetBlendedX(fixed2 pos){
	float ce = TilesColor(pos+float2(0,0)).x;
	float ol = TilesColor(pos+float2(-1,0)).x;
	float or = TilesColor(pos+float2(1,0)).x;
	float ot = TilesColor(pos+float2(0,1)).x;
	float ob = TilesColor(pos+float2(0,-1)).x;

	float otl = TilesColor(pos+float2(-1,1)).x;
	float otr = TilesColor(pos+float2(1,1)).x;
	float obl = TilesColor(pos+float2(-1,-1)).x;
	float obr = TilesColor(pos+float2(1,-1)).x;
	

	float l=(ol+ce)/2;
	float r=(or+ce)/2;
	float t=(ot+ce)/2;
	float b=(ob+ce)/2;
	
	float tl=(otl+ol+ot+ce)/4;
	float tr=(otr+ot+or+ce)/4;
	float bl=(obl+ob+ol+ce)/4;
	float br=(obr+ob+or+ce)/4;

	return blendTile(pos,ce,float4(l,t,r,b),float4(tl,tr,br,bl));
}
float GetBlendedY(fixed2 pos){
	float ce = TilesColor(pos+float2(0,0)).y;
	float ol = TilesColor(pos+float2(-1,0)).y;
	float or = TilesColor(pos+float2(1,0)).y;
	float ot = TilesColor(pos+float2(0,1)).y;
	float ob = TilesColor(pos+float2(0,-1)).y;

	float otl = TilesColor(pos+float2(-1,1)).y;
	float otr = TilesColor(pos+float2(1,1)).y;
	float obl = TilesColor(pos+float2(-1,-1)).y;
	float obr = TilesColor(pos+float2(1,-1)).y;
	

	float l=(ol+ce)/2;
	float r=(or+ce)/2;
	float t=(ot+ce)/2;
	float b=(ob+ce)/2;
	
	float tl=(otl+ol+ot+ce)/4;
	float tr=(otr+ot+or+ce)/4;
	float bl=(obl+ob+ol+ce)/4;
	float br=(obr+ob+or+ce)/4;

	return blendTile(pos,ce,float4(l,t,r,b),float4(tl,tr,br,bl));
}
float GetBlendedZ(fixed2 pos){
	float ce = TilesColor(pos+float2(0,0)).z;
	float ol = TilesColor(pos+float2(-1,0)).z;
	float or = TilesColor(pos+float2(1,0)).z;
	float ot = TilesColor(pos+float2(0,1)).z;
	float ob = TilesColor(pos+float2(0,-1)).z;

	float otl = TilesColor(pos+float2(-1,1)).z;
	float otr = TilesColor(pos+float2(1,1)).z;
	float obl = TilesColor(pos+float2(-1,-1)).z;
	float obr = TilesColor(pos+float2(1,-1)).z;
	

	float l=(ol+ce)/2;
	float r=(or+ce)/2;
	float t=(ot+ce)/2;
	float b=(ob+ce)/2;
	
	float tl=(otl+ol+ot+ce)/4;
	float tr=(otr+ot+or+ce)/4;
	float bl=(obl+ob+ol+ce)/4;
	float br=(obr+ob+or+ce)/4;

	return blendTile(pos,ce,float4(l,t,r,b),float4(tl,tr,br,bl));
}
float GetBlendedW(fixed2 pos){
	float ce = TilesColor(pos+float2(0,0)).w;
	float ol = TilesColor(pos+float2(-1,0)).w;
	float or = TilesColor(pos+float2(1,0)).w;
	float ot = TilesColor(pos+float2(0,1)).w;
	float ob = TilesColor(pos+float2(0,-1)).w;

	float otl = TilesColor(pos+float2(-1,1)).w;
	float otr = TilesColor(pos+float2(1,1)).w;
	float obl = TilesColor(pos+float2(-1,-1)).w;
	float obr = TilesColor(pos+float2(1,-1)).w;
	

	float l=(ol+ce)/2;
	float r=(or+ce)/2;
	float t=(ot+ce)/2;
	float b=(ob+ce)/2;
	
	float tl=(otl+ol+ot+ce)/4;
	float tr=(otr+ot+or+ce)/4;
	float bl=(obl+ob+ol+ce)/4;
	float br=(obr+ob+or+ce)/4;

	return blendTile(pos,ce,float4(l,t,r,b),float4(tl,tr,br,bl));
}