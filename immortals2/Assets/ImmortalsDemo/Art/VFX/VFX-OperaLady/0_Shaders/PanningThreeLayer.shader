// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

///
/// This shader is for the "Slashing" style effects
/// It is to be used in conjunction with driving scripts or animation
/// It is a visual FX shader.
///
Shader "BraveGames/VFX/PanningThreeLayer" {
	Properties {
		_Color ("Tint Color", Color) = (1,1,1,1)
		_MainTex ("Effect Shape", 2D) = "white" {}
		_AlphaMaskTex("Alpha Mask", 2D) = "white" {}
		_Brightness("Brightness", Float) = 1
		_AlphaBoost("Alpha Brightness", Float) = 1
		_AlphaMaskBoost("Alpha Mask boost", Float) = 1
		_PanSpeeds("Pan Speeds", Vector) = (0,0,0,-1)

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
		#pragma surface surf Lambert alpha

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _AlphaMaskTex;

		fixed4 _Color;
		fixed4 _PanSpeeds;
		fixed _Brightness;
		fixed _AlphaBoost;
		fixed _AlphaMaskBoost;

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
			
			fixed2 UVs1 = IN.uv_MainTex;
			fixed2 UVs2 = IN.uv_MainTex;
			fixed2 UVs3 = IN.uv_MainTex;
			UVs2.y += 0.2; // Minor offsets to help it not be so identical 
			UVs3.y += 0.6;
			UVs2.x += 0.2;
			UVs3.x += 0.6;

			UVs1.x += _Time.x * _PanSpeeds.x;
			UVs2.x += _Time.x * _PanSpeeds.y;
			UVs3.x += _Time.x * _PanSpeeds.z;
			// Albedo comes from a texture tinted by color
			fixed4 t1 = tex2D(_MainTex, UVs1);
			fixed4 t2 = tex2D(_MainTex, UVs2);
			fixed4 t3 = tex2D(_MainTex, UVs3);

			// Composite layers
			fixed4 c = t1 + t2 + t3;
			c = clamp(0, 1, c);
			fixed4 alphaMask = tex2D(_AlphaMaskTex, IN.uv_AlphaMaskTex);

			// Composite Diffuse layers
			c = c * _Color * IN.color;

			// Amplify with brightness
			c.rgb = c.rgb * _Brightness;

			// Composite Alpha layers
			c.a *= alphaMask *_AlphaMaskBoost;
			c.a *= _AlphaBoost;
			
			c.a = clamp(0, 1, c.a);
	
			o.Albedo = c.rgb;
			o.Emission = c.rgb;

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
