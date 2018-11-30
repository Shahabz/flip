Shader "IQ/Sky"
{
	Properties
	{
	   _SkyColor("Sky Color", Color) = (0.6,0.71,0.75)
	   _GroundColor("Ground Color",Color) = (1.0,0.5,1.0)
	   _Base("Base",Range(0,0.2)) = 0.075
	   _SunColor("Sun Color",Color) = (1.0,0.6,0.1)
	   _SunPower("Sun Power",Range(2,64)) = 8
	   _SunIntensity("Sun Intensity",Range(0.1,2)) = 0.2
	}

	SubShader
	{
	   Tags
	   {
		  "Queue" = "Background"
		  "RenderType" = "Background"
		  "PreviewType" = "Skybox"
	   }

	   Pass
	   {
		  Cull Off
		  ZWrite Off
		  ZTest LEqual

		  CGPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag

		  #include "UnityCG.cginc"

		  fixed3 _SkyColor;
	      fixed3 _GroundColor;
		  fixed  _Base;
		  fixed3 _SunColor;
		  float _SunPower;
		  float _SunIntensity;


         struct a2v
         {
            half4 vertex : POSITION;
         };

         struct v2f
         {
            half4 position : POSITION;
            half3 rayDir   : TEXCOORD0;
         };

         v2f vert(a2v v)
         {
            v2f o;
            half3 normVert = normalize(v.vertex.xyz);
            o.rayDir = normVert;
            o.position = UnityObjectToClipPos(v.vertex);
            return o;
         }

         fixed4 frag(v2f i) : COLOR
         {
            float sun = saturate(dot(_WorldSpaceLightPos0,normalize(i.rayDir)));
			float fLerp = (i.rayDir.y + 1)*0.5;			
            float3 col = lerp(_GroundColor,_SkyColor,fLerp) + _Base;
            col += _SunIntensity*_SunColor*pow(sun, _SunPower);
            return fixed4(col,1);
         }
         ENDCG
      }
   }
   Fallback off
}