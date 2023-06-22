
Shader  "ImmortalsApp/Character" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    [NoScaleOffset] _BumpMap ("Normalmap", 2D) = "bump" {}
	_TintColor("Tint Color", Color) = (1,1,1,1)
	_TintStrength("Tint", Range(0.0, 1.0)) = 1.0
	_Transparency("Transparency", Range(0.0,1.0)) = 1.0
}

SubShader {
    Tags { "Queue" = "Transparent" "RenderType"="Opaque" }

	Pass{
		ZWrite On
		ColorMask 0
	}

	CGPROGRAM
	#pragma surface surf Lambert alpha:fade

	sampler2D _MainTex;
	sampler2D _BumpMap;
	float4 _TintColor;
	float4 _FinalTint;
	float _TintStrength;
	float _Transparency;

	struct Input {
		float2 uv_MainTex;
	};

	void surf (Input IN, inout SurfaceOutput o) {

		fixed4 c = lerp(tex2D(_MainTex, IN.uv_MainTex)*_TintColor, tex2D(_MainTex, IN.uv_MainTex), 1 - _TintStrength);
		o.Albedo = c.rgb;
		o.Alpha = c.a * _Transparency;
		o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
	}
	ENDCG
}
FallBack "Mobile/Diffuse"
}
