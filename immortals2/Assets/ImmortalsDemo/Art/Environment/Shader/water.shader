// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'unity_World2Shadow' with 'unity_WorldToShadow'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

	
Shader "brave/PlatformShader"
{
	Properties
	{
		// we have removed support for texture tiling/offset,
		// so make them not be displayed in material inspector
		[NoScaleOffset] _MainTex("RenderTexture", 2D) = "white" {}
		_ContrastBoost ("Constrast Boost", Float) = 1
		_RipplesDistortion ("Ripples Distortion", 2D) = "gray" {}
		_RipplesStrength ("Ripples Strength", Float) = 0.2
		_RipplesSpeed ("Ripples UV Scroll", Float) = 0.2
		_Waves ("Waves", Float) = 10
		_WavesSpeed ("Waves Speed", Float) = 1
		_WavesColor ("Waves Color", Color) = (1,1,1,1)
		_WaterColor("Water Color", Color) = (0,0.3,0.9,1)
		_Water2Color("Water2 Color", Color) = (0,0,0,1)
		_FilTex("Filter (Alpha)", 2D) = "black"{}
	}

	SubShader
	{

		Pass
		{
			Tags{ "RenderType" = "Opaque" }
			Tags{ "LightMode" = "ForwardBase" }
			LOD 200
			CGPROGRAM
			// use "vert" function as the vertex shader
			#pragma vertex vert
			// use "frag" function as the pixel (fragment) shader
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			#pragma multi_compile_builtin
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest

			
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"
			// vertex shader inputs
			struct appdata
			{
				float4 vertex : POSITION; // vertex position
				float2 uv : TEXCOORD0; // texture coordinate
				float4 color    : COLOR;
			};

			// vertex shader outputs ("vertex to fragment")
			struct v2f
			{
				float2 uv : TEXCOORD0; // texture coordinate
				float2 uv_ripples : TEXCOORD1; // ripples texture coordinate
				float4 pos : SV_POSITION; // clip space position
				fixed4 color : COLOR;
				#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
					float4 _ShadowCoord : TEXCOORD2;
				#else
					LIGHTING_COORDS(2, 3)
				#endif
			};

			// texture we will sample
			sampler2D _MainTex;

			// Jean: used to offset the UV to add randomness to it, else the result feels very geometric
			sampler2D _RipplesDistortion;
			sampler2D _FilTex;
			float4 _RipplesDistortion_ST;
			float _RipplesStrength;
			float _RipplesSpeed;

			// Jean: boost the contrast of the blurred RT
			float _ContrastBoost;

			// Jean: controls the sin() used on the blurred RT, so that it creates more waves visually
			float _Waves;
			float _WavesSpeed;

			fixed4 _WaterColor;
			fixed4 _Water2Color;
			fixed4 _WavesColor;

			// vertex shader
			v2f vert(appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				//o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.uv_ripples = TRANSFORM_TEX(v.uv, _RipplesDistortion) + (_Time.yy * _RipplesSpeed.xx);
				o.color = v.color;

				TRANSFER_VERTEX_TO_FRAGMENT(o);

				return o;
			}

			// pixel shader; returns low precision ("fixed4" type)
			// color ("SV_Target" semantic)
			fixed4 frag(v2f i) : SV_Target
			{
				float attenuation = LIGHT_ATTENUATION(i);
				fixed ripples = (tex2D(_RipplesDistortion, i.uv_ripples).xy - 0.5) * _RipplesStrength;
				
				// Jean: we only want the red channel
				fixed rt = tex2D(_MainTex, i.uv.xy + ripples).r * _ContrastBoost;

				// Jean: animation based on the red channel
				rt = sin(rt * _Waves + _Time.y * _WavesSpeed) * rt;

				// Jean: add more contrast to the resulting waves, comment this to get more blur in the result
				rt = smoothstep(0.4, 0.6, rt);

				fixed4 _finalWaterColor = lerp(_WaterColor.rgba, _Water2Color.rgba, tex2D(_FilTex, i.uv.xy).r);

				return lerp(_finalWaterColor, _WavesColor, rt)* attenuation;
			}
			ENDCG
		}
		// Pass to render object as a shadow caster
				Pass{
				Name "ShadowCaster"
				Tags{ "LightMode" = "ShadowCaster" }

				Fog{ Mode Off }
				ZWrite On ZTest LEqual Cull Off
				Offset 1, 1

				CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"
			#include "AutoLight.cginc"

				struct v2f {
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
					return o;
			}

			float4 frag(v2f i) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
				ENDCG
			} //Pass
		// Pass to render object as a shadow collector
		Pass
		{
				Name "ShadowCollector"
				Tags{ "LightMode" = "ShadowCollector" }

				Fog{ Mode Off }
				ZWrite On ZTest Less

				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma fragmentoption ARB_precision_hint_fastest
				#pragma multi_compile_shadowcollector

				#define SHADOW_COLLECTOR_PASS
				#include "UnityCG.cginc"
				#include "AutoLight.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				V2F_SHADOW_COLLECTOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				TRANSFER_SHADOW_COLLECTOR(o)
					return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				SHADOW_COLLECTOR_FRAGMENT(i)
			}
				ENDCG

			}
	}
	
}