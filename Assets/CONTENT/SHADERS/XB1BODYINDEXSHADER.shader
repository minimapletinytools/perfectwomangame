Shader "Custom/XB1GRAYSCALESHADER" {

	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
	
		Pass {
			CGPROGRAM
			
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 5.0
			
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			
			float4 frag (v2f_img i) : SV_Target
			{
				float4 pixelVal = tex2D( _MainTex, i.uv );
				int playerId = pixelVal.a * 255;
	 
				switch(playerId)
				{
					case 0:  return float4(1,0,0,1);     // player 1 = red
					case 1:  return float4(1,0.8,0,1);   // player 2 = orange
					case 2:  return float4(1,1,0,1);     // player 3 = yellow
					case 3:  return float4(0,1,0,1);     // player 4 = green
					case 4:  return float4(0,1,1,1);     // player 5 = cyan
					case 5:  return float4(0,0,1,1);     // player 6 = blue
					
					// The Xbox One's Kinect Sensor can only track 6 players at a time.
					default:  return float4(0.1,0,0.1,1);     // dark purple, background	
				}
			}
			
			ENDCG
			}
	}
	FallBack "Diffuse"
}
