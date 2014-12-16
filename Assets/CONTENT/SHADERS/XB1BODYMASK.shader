
Shader "Custom/XB1BODYMASK" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AlphaTex ("Culling Mask", 2D) = "white" {}
	}

	SubShader {
		Tags { "RenderType"="Transparent" }
		AlphaTest Greater 0
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always
		Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		
		Pass {
			CGPROGRAM
			
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			
			static const float4 g_yuy2Offset = { 0.501961, 0, 0.501961, 0 };
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			
			float4 frag (v2f_img i) : SV_Target
			{
				float4 yuy2 = tex2D( _MainTex, i.uv );
				float4 offset = yuy2 - g_yuy2Offset;
				
				int index = tex2D( _AlphaTex, i.uv ).a*255;
				float alphaColor = index <= 5 ? 1 : 0;
				//float alphaColor = 1;
				float rColor = index <= 5 ? clamp( offset.g + 1.568648 * offset.b, 0.0, 1.0 ) : 0;
				
		
				float4 rgba = float4
				(
					clamp( offset.g + 1.568648 * offset.b, 0.0, 1.0 ),
					//rColor,
					clamp( offset.g - 0.186593 * offset.r - 0.466296 * offset.b, 0.0, 1.0 ),
					clamp( offset.g + 1.848352 * offset.r, 0.0, 1.0 ),
					alphaColor
				);
			
				
				return rgba;
			}
			
			ENDCG
			}
	}
	FallBack "Diffuse"
}
