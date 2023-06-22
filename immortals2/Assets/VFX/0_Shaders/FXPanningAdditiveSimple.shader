// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "BraveGames/VFX/FXPanningAdditiveSimple" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Active Texture", 2D) = "white" {}
		_Brightness("Brightness", Float) = 1
		_PanSpeed("Pan Speed", Float) = 1

	}
	SubShader {
		Tags{ "IgnoreProjector" = "True"
		"Queue" = "Transparent"
		"RenderType" = "Transparent"
	}
		LOD 200
		Blend One One
		Cull Off
		ZWrite Off
		Offset -1, -1
		
		CGPROGRAM
		#pragma surface surf Lambert alpha
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 color : COLOR;
		};

		fixed4 _Color;
		fixed _Brightness;
		fixed _PanSpeed;
		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {

			fixed2 UVs1 = IN.uv_MainTex;
			fixed2 UVs2 = IN.uv_MainTex;
			UVs2.y += 0.1;
			UVs2.x += 0.5;

			UVs1.x += _Time.x * _PanSpeed;
			UVs2.x -= _Time.x * (_PanSpeed + 0.2);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, UVs1);
			fixed4 d = tex2D(_MainTex, UVs2);

			c = (c * d) * 1.5;
			c = c * _Color;

			c.rgb = clamp(0, 1, c.rgb *_Brightness);

			o.Albedo = c.rgb;
			o.Emission = c.rgb * IN.color.rgb;

			o.Alpha = clamp(0,1, c.a * IN.color);
		}
		ENDCG
	}
	FallBack "Diffuse"
}
