// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'unity_World2Shadow' with 'unity_WorldToShadow'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'unity_World2Shadow' with 'unity_WorldToShadow'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

	
Shader "brave/WaterShader"
{
	
	Properties
	{
		// we have removed support for texture tiling/offset,
		// so make them not be displayed in material inspector
		[NoScaleOffset] _MainTex("RenderTexture", 2D) = "white" {}
		_SkyTex("Sky Texture", 2D) = "white" {}
		_ContrastBoost("Constrast Boost", Float) = 1
		_OffSet("OffSet ", Float) = 1
		_WavesDistortion("Waves Distortion", 2D) = "gray" {}
		_RipplesDistortion("Ripples Distortion", 2D) = "gray" {}
		_RipplesStrength("Ripples Strength", Float) = 0.2
		_RipplesSpeed("Ripples UV Scroll", Float) = 0.2
		_Waves("Waves", Float) = 10
		_WaveIntensity("WaveIntensity", Float) = 1.0
		_WavesSpeed("Waves Speed", Float) = 1
		_WavesColor("Waves Color", Color) = (1,1,1,1)
		_WaterColor("Water Color", Color) = (0,0.3,0.9,1)
		_Water2Color("Water2 Color", Color) = (0,0,0,1)
		_FilTex("Filter (Alpha)", 2D) = "black"{}
		//	[NoScaleOffset] _CubeTex("Cubemap (HDR)", Cube) = "grey" {}
		[Gamma] _Exposure("Exposure", Range(0, 8)) = 1.0
		_RotationSpeed("Rotation Speed", Float) = 2.0
		_ScrollSpeeds("Scroll Speeds", vector) = (-5, -20, 0, 0)
	}

		SubShader
	{
		Pass
	{
		Tags{ "RenderType" = "Opaque" }
		Tags{ "LightMode" = "ForwardBase" }
		LOD 200
		CGPROGRAM
		#pragma target 3.0
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
		float3 normal : NORMAL;
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
		float3 normalDir : TEXCOORD4;
		float2 uv_sky : TEXCOORD5;
		float2 uv_waves : TEXCOORD6;
	};
	//samplerCUBE _CubeTex;
	uniform float4x4 _Rotation;

	// texture we will sample
	sampler2D _MainTex;
	sampler2D _SkyTex;
	sampler2D _WavesDistortion;

	// Jean: used to offset the UV to add randomness to it, else the result feels very geometric
	sampler2D _RipplesDistortion;
	sampler2D _FilTex;
	float4 _RipplesDistortion_ST;
	float4 _WavesDistortion_ST;
	float4 _SkyTex_ST;
	float _RipplesStrength;
	float _RipplesSpeed;
	half _Exposure;

	float _RotationSpeed;

	// Jean: boost the contrast of the blurred RT
	float _ContrastBoost;
	float _OffSet;

	// Jean: controls the sin() used on the blurred RT, so that it creates more waves visually
	float _Waves;
	float _WaveIntensity;
	float _WavesSpeed;

	half4 _WaterColor;
	half4 _Water2Color;
	half4 _WavesColor;
	float4 _ScrollSpeeds;

	// vertex shader
	v2f vert(appdata v)
	{
		v2f OUT;
		OUT = (v2f)0;
		OUT.pos = UnityObjectToClipPos(v.vertex);
		OUT.uv = v.uv;

		float s = sin(_RotationSpeed * _Time);
		float c = cos(_RotationSpeed * _Time);
		float2x2 rotationMatrix = float2x2(c, -s, s, c);
		rotationMatrix *= 0.5;
		rotationMatrix += 0.5;
		rotationMatrix = rotationMatrix * 2 - 1;
		OUT.uv_sky = TRANSFORM_TEX(v.uv, _SkyTex);
		OUT.uv_sky += _ScrollSpeeds * _Time.xx;

		OUT.uv_waves.xy = mul(TRANSFORM_TEX(v.uv, _WavesDistortion), rotationMatrix);
		OUT.uv_ripples = (TRANSFORM_TEX(v.uv, _RipplesDistortion) + (_Time.yy * _RipplesSpeed.xx));
		OUT.color = v.color;

		#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
			OUT._ShadowCoord = mul(unity_WorldToShadow[0], mul(unity_ObjectToWorld, v.vertex));
		#else
			TRANSFER_VERTEX_TO_FRAGMENT(OUT);
		#endif   

		return OUT;
	}

	// pixel shader; returns low precision ("fixed4" type)
	// color ("SV_Target" semantic)
	fixed4 frag(v2f i) : SV_Target
	{		
		half4 env = tex2D(_SkyTex, i.uv_sky);
		env *= _Exposure;

		float attenuation = LIGHT_ATTENUATION(i);

		fixed ripples = (tex2D(_RipplesDistortion, i.uv_ripples).xy - 0.5) * _RipplesStrength;

		// Jean: animation based on the red channel
		fixed rt = tex2D(_MainTex, i.uv.xy + ripples).r * _ContrastBoost;

		fixed bt = tex2D(_MainTex, i.uv.xy + ripples).r * _OffSet;

		rt = sin(rt * _Waves + _Time.y * _WavesSpeed) * rt;

		rt = lerp(rt, fixed4(0, 0, 0, 0), tex2D(_WavesDistortion, i.uv_waves).g);
		// Jean: add more contrast to the resulting waves, comment this to get more blur in the result
		rt = smoothstep(0.4, 0.6, rt);
		bt = smoothstep(0.50, 0.55, bt);

		fixed _finalRipples = clamp(bt + rt, 0, _WaveIntensity);

		half4 _outputWaterColor = lerp(_WaterColor.rgba, _Water2Color.rgba, tex2D(_FilTex, i.uv.xy).g);

		half4 _finalWaterOutput = lerp(_outputWaterColor, env, tex2D(_FilTex, i.uv.xy).r);

		half4 _finalWaterWithRipples = lerp(_finalWaterOutput, _WavesColor, _finalRipples)* attenuation;

		return _finalWaterWithRipples;
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