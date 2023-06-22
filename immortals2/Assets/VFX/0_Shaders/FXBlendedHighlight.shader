// Upgrade NOTE: upgraded instancing buffer 'Props' to new syntax.

Shader "BraveGames/VFX/FXBlendedHighlight" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Active Texture", 2D) = "white" {}
		_Brightness("Brightness", Float) = 1

	}
	SubShader {
		Tags{ "IgnoreProjector" = "True"
		"Queue" = "Transparent"
		"RenderType" = "Transparent"
	}
		LOD 200
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		Offset -1, 1
		
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

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color * IN.color;

			c.rgb = c.rgb * _Brightness;

			o.Albedo = c.rgb;
			o.Emission = c.rgb;

			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
