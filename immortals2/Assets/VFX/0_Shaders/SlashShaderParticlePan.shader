///
/// This shader is for the "Slashing" style effects
/// It is to be used in conjunction with driving scripts or animation
/// It is a visual FX shader.
/// This shader uses the RED vertex colour to drive panning the UVs on the particle mesh.
///
Shader "BraveGames/VFX/SlashShaderParticlePan" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Effect Shape", 2D) = "white" {}
		_NoiseMaskTex("Noise Mask", 2D) = "black" {}
		_AlphaMaskTex("Alpha Mask", 2D) = "white" {}
		_Brightness("Brightness", Float) = 1
		_AlphaBoost("Alpha Brightness", Float) = 1
		_AlphaMaskBoost("Alpha Mask boost", Float) = 1

		_RedPanSpeed("Pan Speed Multiplier", Float) = 1

		//[HideInInspector]_Cutoff("Alpha cutoff", Range(0,1)) = 0.1
	}
	SubShader {
		Tags {	"IgnoreProjector" = "True"
				"Queue" = "Transparent"
				"RenderType" = "Transparent" 
			}
		LOD 200
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			//Offset 1, 1

		CGPROGRAM
		#pragma surface surf Lambert alpha addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _NoiseMaskTex;
		sampler2D _AlphaMaskTex;

		fixed4 _Color;
		fixed _Brightness;
		fixed _AlphaBoost;
		fixed _AlphaMaskBoost;
		fixed _RedPanSpeed;

		struct Input {
			float2 uv_MainTex;
			float2 uv_NoiseMaskTex;
			float2 uv_AlphaMaskTex;

			float4 color : COLOR;
		};

		void surf (Input IN, inout SurfaceOutput  o) {

			float2 pannedUVs = IN.uv_MainTex;
			fixed PanPos = lerp(1, -1, IN.color.r * _RedPanSpeed);
			/// This shader uses the RED vertex colour to drive panning the UVs on the particle mesh.
			pannedUVs.x += PanPos;

			fixed4 baseTex = tex2D(_MainTex, pannedUVs);
			fixed4 noiseMask = tex2D(_NoiseMaskTex, IN.uv_NoiseMaskTex);
			fixed4 alphaMask = tex2D(_AlphaMaskTex, pannedUVs);
			//fixed4 alphaMaskDynamic = tex2D(_AlphaMaskTex, IN.uv_MainTex); // For slight extra gradient

			// Composite Diffuse layers
			fixed4 c = baseTex * _Color;// *IN.color;

			// Amplify with brightness
			c.rgb = c.rgb * _Brightness;

			// Composite Alpha layers
			c.a *= alphaMask *_AlphaMaskBoost;
			c.a *= noiseMask;// *alphaMaskDynamic;
			c.a *= _AlphaBoost;
			
			c.a = clamp(0, 1, c.a);
			

			o.Albedo = c.rgb;
			o.Emission = c.rgb;

			// Useful for stylizing the effect
			//c.a = saturate(c.a - (_TrimThreshold - c.a));

			o.Alpha = c.a * IN.color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
