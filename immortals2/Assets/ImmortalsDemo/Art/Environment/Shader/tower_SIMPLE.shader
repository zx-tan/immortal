	
Shader "brave/towerSIMPLE" 
	{
		Properties
		{
			_MainTex("Main Texture", 2D) = "white" {}
			_ShadowColor("Shadow Color",Color) = (1,1,0,1)

		[Header(Ambient)]
		_Ambient("Intensity", Range(0., 1.)) = 0.1
			_AmbColor("Color", color) = (1., 1., 1., 1.)

			[Header(Diffuse)]
		_Diffuse("Val", Range(0., 1.)) = 1.
			_DifColor("Color", color) = (1., 1., 1., 1.)

			[Header(Specular)]
		[Toggle] _Spec("Enabled?", Float) = 0.
			_Shininess("Shininess", Range(0.1, 10)) = 1.
			_SpecColor("Specular color", color) = (1., 1., 1., 1.)

			[Header(Emission)]
		_EmissionTex("Emission texture", 2D) = "gray" {}
		_EmiVal("Intensity", float) = 0.
			[HDR]_EmiColor("Color", color) = (1., 1., 1., 1.)
		}

		SubShader
		{
			Tags{ "Queue" = "Geometry" "RenderType" = "Opaque" }
		Pass
		{
			Tags{ "LightMode" = "ForwardBase" }
			LOD 200
			CGPROGRAM

			#pragma target 3.0
			// use "vert" function as the vertex shader
			#pragma vertex vert
						// use "frag" function as the pixel (fragment) shader
			#pragma fragment frag
			#pragma multi_compile_fwdbase                       // This line tells Unity to compile this pass for forward base.
			#pragma fragmentoption ARB_fog_exp2
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			// Change "shader_feature" with "pragma_compile" if you want set this keyword from c# code
			#pragma shader_feature __ _SPEC_ON

			#include "UnityCG.cginc"
#			include "AutoLight.cginc"

		struct v2f
		 {
			float4 pos : SV_POSITION;
			float2 uv			: TEXCOORD0;
			fixed4 light : COLOR0;

			#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
				float4 _ShadowCoord : TEXCOORD1;
			#else
				LIGHTING_COORDS(1, 2)
			#endif
		};

		fixed4 _LightColor0;

		// Diffuse
		fixed _Diffuse;
		fixed4 _DifColor;

		//Specular
		fixed _Shininess;
		fixed4 _SpecColor;

		//Ambient
		fixed _Ambient;
		fixed4 _AmbColor;

		uniform fixed4 _ShadowColor;

		v2f vert(appdata_base v)
		{
			v2f o;
			// World position
			float4 worldPos = mul(unity_ObjectToWorld, v.vertex);

			// Clip position
			o.pos = mul(UNITY_MATRIX_VP, worldPos);

			// Light direction
			float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

			// Normal in WorldSpace
			float3 worldNormal = normalize(mul(v.normal, (float3x3)unity_WorldToObject));

			// Camera direction
			float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos.xyz);

			// Compute ambient lighting
			fixed4 amb = _Ambient * _AmbColor;

			// Compute the diffuse lighting
			fixed4 NdotL = max(0., dot(worldNormal, lightDir) * _LightColor0);
			fixed4 dif = NdotL * _Diffuse * _LightColor0 * _DifColor;

			o.light = dif + amb;

			// Compute the specular lighting
			#if _SPEC_ON
			float3 refl = reflect(-lightDir, worldNormal);
			float RdotV = max(0., dot(refl, viewDir));
			fixed4 spec = pow(RdotV, _Shininess) * _LightColor0 * ceil(NdotL) * _SpecColor;

			o.light += spec;
			#endif

			o.uv = v.texcoord;

			TRANSFER_VERTEX_TO_FRAGMENT(o);

			return o;
		}

		sampler2D _MainTex;

		// Emission
		sampler2D _EmissionTex;
		fixed4 _EmiColor;
		fixed _EmiVal;

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 c = tex2D(_MainTex, i.uv);
			c.rgb *= i.light;

			float shadow_attenuation = SHADOW_ATTENUATION(i);

			// Compute emission
			fixed4 emi = tex2D(_EmissionTex, i.uv).r * _EmiColor * _EmiVal;
			c.rgb += emi.rgb;


			half4 output = lerp(( c * _ShadowColor), c, shadow_attenuation);

			return output;
		}

			ENDCG
		}

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
			Pass{
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

