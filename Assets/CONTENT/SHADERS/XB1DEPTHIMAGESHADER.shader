Shader "Custom/XB1DEPTHIMAGESHADER"{

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Cull Off Lighting Off ZWrite Off 
		Pass {
			CGPROGRAM
			
			#pragma vertex vert_img
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			uniform sampler2D _MainTex;
			
			float4 frag (v2f_img i) : SV_Target
			{
				float4 depth = tex2D( _MainTex, i.uv );
				//float shading = max(depth.r,1);
				return float4(depth.r,depth.r,depth.r, 1);
			}
			
			ENDCG
			}
	}
	FallBack "Diffuse"
}
