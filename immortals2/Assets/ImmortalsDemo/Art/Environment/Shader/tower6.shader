	
Shader "brave/tower" {
Properties{
	 _Diff_Map("Diff_Map", 2D) = "white" {}
	 _Light_size("Light_size", Range(0, 1)) = 1
	 _Specular_Map("Specular_Map", 2D) = "white" {}
	 _Specular_size("Specular_size", Range(0, 20)) = 0.70
	 _Gloss_size("Gloss_size", Range(0, 1)) = 0.50
	 _Rim_Color("Rim_Color", Color) = (1,1,1,1)
	 _Rim_size("Rim_size", Range(0, 8)) = 1.7
	 _Sharpening_size("Sharpening_size", Range(0, 30)) = 16.0
	 _Emission_Map("Emission_Map", 2D) = "white" {}
	 _Emission_size("Emission_size", Range(0, 1)) = 0.30
	 _Normal_Map("Normal_Map", 2D) = "bump" {}
	 _ShadowColor("Shadow Color",Color) = (1,1,0,1)
	}
	SubShader{
	Tags{"IgnoreProjector" = "True""Queue" = "Geometry+200""RenderType" = "Opaque"}
		LOD 200
	Pass
	{
		Name "FORWARD" Tags{"LightMode" = "ForwardBase"}
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
//#define UNITY_PASS_FORWARDBASE
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "autolight.cginc"
#pragma multi_compile_fwdbase
//#pragma only_renderers d3d9 d3d11 glcore gles 
//#pragma target 2.0
	uniform sampler2D _Normal_Map; uniform float4 _Normal_Map_ST;
	uniform sampler2D _Diff_Map; uniform float4 _Diff_Map_ST;
	uniform sampler2D _Specular_Map; uniform float4 _Specular_Map_ST;
	uniform float4 _Rim_Color;
	uniform fixed _Rim_size;
	uniform sampler2D _Emission_Map; uniform float4 _Emission_Map_ST;
	uniform fixed _Emission_size;
	uniform fixed _Light_size;
	
	uniform fixed _Sharpening_size;
	uniform fixed _Gloss_size;
	uniform fixed _Specular_size;
	uniform fixed4 _ShadowColor;
	struct VertexInput {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
		float4 tangent : TANGENT;
		float2 texcoord0 : TEXCOORD0;
	};
	struct VertexOutput {
		float4 pos : SV_POSITION;
		float2 uv0 : TEXCOORD0;
		float4 posWorld : TEXCOORD1;
		float3 normalDir : TEXCOORD2;
		float3 tangentDir : TEXCOORD3;
		float3 bitangentDir : TEXCOORD4;
		float4 posWorld2 : TEXCOORD5;
		LIGHTING_COORDS(5, 6)
	};
	VertexOutput vert(VertexInput v) {
		VertexOutput o = (VertexOutput)0;
		o.uv0 = v.texcoord0;
		o.normalDir = UnityObjectToWorldNormal(v.normal);
		o.tangentDir = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0.0)).xyz);
		o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		float3 lightColor = _LightColor0.rgb;
		o.pos = UnityObjectToClipPos(v.vertex);

		TRANSFER_VERTEX_TO_FRAGMENT(o);
		TRANSFER_SHADOW(o)
			return o;
	};
	float4 frag(VertexOutput i) : COLOR{
		i.normalDir = normalize(i.normalDir);
	float3x3 tangentTransform = float3x3(i.tangentDir, i.bitangentDir, i.normalDir);
	float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
	float3 _Normal_Map_var = UnpackNormal(tex2D(_Normal_Map,TRANSFORM_TEX(i.uv0, _Normal_Map)));
	float3 normalLocal = _Normal_Map_var.rgb;
	float3 normalDirection = normalize(mul(normalLocal, tangentTransform)); // Perturbed normals
	float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
	float3 lightColor = _LightColor0.rgb;
	float3 halfDirection = normalize(viewDirection + lightDirection);
	////// Lighting:
	float attenuation = LIGHT_ATTENUATION(i);
	float3 attenColor = attenuation * _LightColor0.xyz;
	///////// Gloss:
	float gloss = _Gloss_size;
	float specPow = exp2(gloss * 10.0 + 1.0);
	////// Specular:
	float NdotL = max(0, dot(normalDirection, lightDirection));
	float4 _Specular_Map_var = tex2D(_Specular_Map,TRANSFORM_TEX(i.uv0, _Specular_Map));
	float3 specularColor = (_Specular_Map_var.rgb*_Specular_size);
	float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularColor;
	float3 specular = directSpecular;
	/////// Diffuse:
	NdotL = max(0.0,dot(normalDirection, lightDirection));
	float3 directDiffuse = max(0.0, NdotL) * attenColor;
	float3 indirectDiffuse = float3(0,0,0);
	float4 _Diff_Map_var = tex2D(_Diff_Map,TRANSFORM_TEX(i.uv0, _Diff_Map));
	float3 diffuseColor = _Diff_Map_var.rgb*_Light_size;
	indirectDiffuse += diffuseColor; // Diffuse Ambient Light
	float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
	////// Emissive:
	float4 _Emission_Map_var = tex2D(_Emission_Map,TRANSFORM_TEX(i.uv0, _Emission_Map));
	float node_6218 = i.normalDir.r;
	float3 node_4577 = (_Diff_Map_var.rgb + ((_Rim_Color.rgb*(pow(1.0 - max(0,dot(i.normalDir, viewDirection)),_Sharpening_size)*(node_6218 + ((1.0 - node_6218)*0.48))))*_Rim_size));
	float3 emissive = ((_Emission_size*_Emission_Map_var.rgb) + node_4577);

		float3 finalColor = diffuse + specular + emissive;

	float shadow_attenuation = SHADOW_ATTENUATION(i);
	finalColor = lerp((finalColor*_ShadowColor),finalColor, shadow_attenuation);
	return fixed4(finalColor,1);
	}
		ENDCG
	}
		}
		 FallBack "Mobile/Diffuse"
}
