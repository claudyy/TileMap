// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Cg per-pixel lighting with vertex lights" {
   Properties {
      _Color ("Diffuse Material Color", Color) = (1,1,1,1) 
      _SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
      _Shininess ("Shininess", Float) = 10
      _LightStrength ("Light strength", Range(0,1)) = .5
      _SunStrength ("Sun Strength", Range(0,1)) = .5
      _AmbientStrength ("Ambient", Range(0,1)) = .5
	  _MainTex ("Albedo (RGB)", 2D) = "white" {}
	  _MyNormalMap("My Normal map", 2D) = "normal" {}
	  _MySpecMap("Spec", 2D) = "black" {}

   }
   SubShader {
      Pass {      
         Tags { "LightMode" = "ForwardBase" } // pass for 
            // 4 vertex lights, ambient light & first pixel light
 
         CGPROGRAM
         #pragma multi_compile_fwdbase 
         #pragma vertex vert
         #pragma fragment frag
 
         #include "UnityCG.cginc" 
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _SpecColor; 
         uniform float _Shininess;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
			float2 uv : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 posWorld : TEXCOORD0;
            float3 normalDir : TEXCOORD1;
            float3 vertexLighting : TEXCOORD2;
            float2 uv : TEXCOORD3;
			float2 screenuv : TEXCOORD4;

         };
 
         vertexOutput vert(vertexInput input)
         {          
            vertexOutput output;
			output.uv = input.uv;
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.pos = UnityObjectToClipPos(input.vertex);
 
            // Diffuse reflection by four "vertex lights"            
            output.vertexLighting = float3(0.0, 0.0, 0.0);
            #ifdef VERTEXLIGHT_ON
            for (int index = 0; index < 4; index++)
            {    
               float4 lightPosition = float4(unity_4LightPosX0[index], 
                  unity_4LightPosY0[index], 
                  unity_4LightPosZ0[index], 1.0);
 
               float3 vertexToLightSource = 
                  lightPosition.xyz - output.posWorld.xyz;        
               float3 lightDirection = normalize(vertexToLightSource);
               float squaredDistance = 
                  dot(vertexToLightSource, vertexToLightSource);
               float attenuation = 1.0 / (1.0 + 
                  unity_4LightAtten0[index] * squaredDistance);
               float3 diffuseReflection = attenuation 
                  * unity_LightColor[index].rgb * _Color.rgb 
                  * max(0.0, dot(output.normalDir, lightDirection));         
 
               output.vertexLighting = 
                  output.vertexLighting + diffuseReflection;
            }
            #endif
			output.screenuv = ((output.pos.xy / output.pos.w) + 1) * 0.5;

            return output;
         }
		 float2 safemul(float4x4 M, float4 v)
			{
				float2 r;

				r.x = dot(M._m00_m01_m02, v);
				r.y = dot(M._m10_m11_m12, v);

				return r;
			}
			sampler2D _MainTex;
			sampler2D _MyNormalMap,_MySpecMap;
			float _SunStrength,_AmbientStrength;
		uniform sampler2D _GlobalRefractionTex;
		uniform float _GlobalVisibility;
		uniform float _GlobalRefractionMag;
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
			float2 n = UnpackNormal(tex2D(_MyNormalMap, input.uv));
			float4 c = tex2D(_MainTex, input.uv)*_Color;
			normalDirection = float3(n.x,n.y,-1)*c.a;


            float3 viewDirection = normalize(
               _WorldSpaceCameraPos - input.posWorld.xyz);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = 
                  normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float3 ambientLighting = 
                UNITY_LIGHTMODEL_AMBIENT.rgb * _Color.rgb;
 
            float3 diffuseReflection = 
               attenuation * _LightColor0.rgb * _Color.rgb 
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else // light source on the right side
            {
               specularReflection = attenuation * _LightColor0.rgb 
                  * _SpecColor.rgb * pow(max(0.0, dot(
                  reflect(-lightDirection, normalDirection), 
                  viewDirection)), _Shininess);
            }

			float spec = attenuation * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
			float cutSpec =step(.1,spec);
			float4 s = tex2D(_MySpecMap, input.uv);
			c = lerp(c,c+_SpecColor*_LightColor0.rgba ,cutSpec*s);


			float cut =step(.1,max(0.0, dot(normalDirection, lightDirection))*attenuation*_LightColor0.rgb );
            //return lerp(c*_AmbientStrength*UNITY_LIGHTMODEL_AMBIENT,c*_SunStrength*_LightColor0,cut);
			c = lerp(c*_AmbientStrength*UNITY_LIGHTMODEL_AMBIENT,c*_SunStrength*_LightColor0,cut);
			float2 offset = safemul(unity_ObjectToWorld, tex2D(_MyNormalMap, input.uv) * 2 - 1);
			float4 worldRefl = tex2D(_GlobalRefractionTex, input.screenuv + offset.xy * _GlobalRefractionMag)* _GlobalVisibility;
			return lerp(c,worldRefl,s);


			return float4(input.vertexLighting + ambientLighting 
               + diffuseReflection + specularReflection, 1.0);
         }
         ENDCG
      }
 
      Pass {    
         Tags { "LightMode" = "ForwardAdd" } 
            // pass for additional light sources
		BlendOp Max
         Blend One One // additive blending 
 
          CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc" 
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
         // User-specified properties
         uniform float4 _Color; 
         uniform float4 _SpecColor; 
         uniform float _Shininess;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;

         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
            float4 posWorld : TEXCOORD1;
            float3 normalDir : TEXCOORD2;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
 
            output.posWorld = mul(modelMatrix, input.vertex);
            output.normalDir = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.pos = UnityObjectToClipPos(input.vertex);
			output.uv= input.texcoord;
            return output;
         }
			sampler2D _MainTex;
			sampler2D _MyNormalMap,_MySpecMap;
			float _LightStrength;
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normalDir);
			float2 n = UnpackNormal(tex2D(_MyNormalMap, input.uv));
			float4 c = tex2D(_MainTex, input.uv);
			normalDirection = float3(n.x,n.y,-1)*c.a;
            float3 viewDirection = normalize(
               _WorldSpaceCameraPos.xyz - input.posWorld.xyz);
            float3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = 
                  normalize(_WorldSpaceLightPos0.xyz);
            } 
            else // point or spot light
            {
               float3 vertexToLightSource = 
                  _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            float3 diffuseReflection = 
               attenuation * _LightColor0.rgb * _Color.rgb
               * max(0.0, dot(normalDirection, lightDirection));
 
            float3 specularReflection;
            if (dot(normalDirection, lightDirection) < 0.0) 
               // light source on the wrong side?
            {
               specularReflection = float3(0.0, 0.0, 0.0); 
                  // no specular reflection
            }
            else // light source on the right side
            {
               specularReflection = attenuation * _LightColor0.rgb 
                  * _SpecColor.rgb * pow(max(0.0, dot(
                  reflect(-lightDirection, normalDirection), 
                  viewDirection)), _Shininess);
            }
			float cut =step(.08,max(0.0, dot(normalDirection, lightDirection))*attenuation* _LightColor0.a*2);
			float spec = attenuation * pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess);
			float cutSpec =step(.1,spec);
			float4 s = tex2D(_MySpecMap, input.uv);
			c = lerp(c,c+_SpecColor*_LightColor0.rgba ,cutSpec*s);
            return lerp(fixed4(0,0,0,c.a),c*_LightStrength* _LightColor0.rgba ,cut);
            return float4(diffuseReflection , 1.0);
               // no ambient lighting in this pass
         }
 
         ENDCG
      }
 
   } 
   Fallback "Specular"
}