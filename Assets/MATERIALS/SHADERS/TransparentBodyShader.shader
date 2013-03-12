Shader "Custom/TransparentBodyShader" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		AlphaTest Greater 0
		//Blend SrcAlpha OneMinusSrcAlpha
		Blend One Zero
		CGPROGRAM
		#pragma surface surf SimpleLambert
		//this is not lambert at all
		half4 LightingSimpleLambert (SurfaceOutput s, half3 lightDir, half atten) {
		  half4 c;
		  c.rgb = s.Albedo;
		  c.a = s.Alpha;
		  return c;
		}
		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			if(c.a != 0)
			{
				o.Alpha = min(0.5,c.a);
				//o.Albedo = half3(0,0,0);
				o.Albedo = half3(0.2,0.2,0.2);
			}
			else
			{
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
