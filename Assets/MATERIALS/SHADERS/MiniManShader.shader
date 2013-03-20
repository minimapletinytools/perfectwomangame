Shader "Custom/MiniManShader" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "Queue"="Transparent +100" "IgnoreProjector"="True" "RenderType"="Transparent" }
		AlphaTest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha
		//Blend One Zero
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
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
		fixed4 _Color;
		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			if(c.a != 0)
			{
				o.Alpha =  _Color.a;
				//o.Alpha = min(0.5,c.a);
				o.Albedo = half3(0.7,0.7,0.7);
				//o.Albedo = half3(0.4,0.4,0.4);
			}
			else
			{
				o.Albedo = c.rgb;
				o.Alpha = c.a * _Color.a;
			}
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
