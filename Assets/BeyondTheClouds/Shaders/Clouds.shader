Shader "AUXSummber/Clouds" 
{
	Properties 
    {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader 
	{
       	ZTest Always Cull Off ZWrite Off
       	Fog { Mode Off }       	
		Pass
		{
	        CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "CloudsInclude.cginc"
			ENDCG
        }
	}
	FallBack Off
}