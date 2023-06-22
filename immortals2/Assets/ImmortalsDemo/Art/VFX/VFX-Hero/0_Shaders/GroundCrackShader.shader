// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

///
/// This shader is for the "Ground Crack" style effects
/// It is to be used in conjunction with driving scripts or animation
/// It is a visual FX shader.
///
Shader "BraveGames/VFX/GroundCrackShader" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Effect Shape", 2D) = "white" {}
		_AlphaMaskTex("Alpha Mask", 2D) = "white" {}
		//_TrimThreshold("Trim Threshold", Float) = 1

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
			Offset 1, 1

		CGPROGRAM
		#pragma surface surf Lambert alpha addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _AlphaMaskTex;

		fixed4 _Color;
		fixed _TrimThreshold;

		struct Input {
			float2 uv_MainTex;
			float2 uv_AlphaMaskTex;

			float4 color : COLOR;
		};


		// This can likely be removed - T
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			
			fixed4 baseTex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 alphaMask = tex2D(_AlphaMaskTex, IN.uv_AlphaMaskTex);

			// Composite Diffuse layers
			fixed4 c = baseTex * _Color * IN.color;

			// Composite Alpha layers
			c.a *= alphaMask;
			
			// Useful for stylizing the effect
			//c.a = saturate(c.a - (_TrimThreshold - c.a));
			//clip()

			c.a = clamp(0, 1, c.a);

			o.Albedo = c.rgb;
			o.Emission = c.rgb;

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
