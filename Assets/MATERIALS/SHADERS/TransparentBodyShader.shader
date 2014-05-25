Shader "Custom/TransparentBodyShader" {
	Properties {
		_Color ("Main Color", Color) = (0,0.317,.898,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Transparent" }
		AlphaTest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha
		//ZTest Less
		Cull Off Lighting Off ZWrite On Fog { Color (0,0,0,0) }
		
		//Blend One Zero
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
			float4 screenPos;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = _Color.rgb;
			o.Alpha = 0;
			if(c.a != 0)
			{
				
				
				
				float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
				
				//if(((int)(screenUV.x*100 + screenUV.y*100) % 2 == 0))
				{
					o.Alpha = min(c.a,_Color.a);
					//o.Alpha = _Color.a;
				
				}
			}
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
