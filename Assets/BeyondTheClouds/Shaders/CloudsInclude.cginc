#include "UnityCG.cginc"

UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
float4     _MainTex_TexelSize;
float4     _MainTex_ST;
sampler2D  _NoiseTex;
UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
float4 _CameraDepthTexture_TexelSize;

float4x4 _ClipToWorld;

float4 _OptimizeDatas;
float4 _DensityParams;
float2 _HeightDatas;

float3 _WindSpeed;
float4 _QScales;

float _AmbientParam;

float3 _AmbientColor;
float3 _DiffuseColor;
float3 _HighDensityColor;
float3 _LowDensityColor;

float3 _SunGlowColor;
float  _SunGlowPower;
float  _SunGlowIntensity;

float noise( float3 x )
{    
    float3 p = floor(x);
    float3 f = frac(x);
	f = f*f*(3.0-2.0*f);
	float2 uv = (p.xy+float2(37.0,17.0)*p.z) + f.xy;
	float2 rg = tex2Dlod(_NoiseTex, float4((uv+0.5)/256.0,0.0,0.0)).yx;
	return -1.0+2.0*lerp( rg.x, rg.y, f.z );    
}

float clampDensity(float h,float f)
{
   	return clamp(_DensityParams.x + _DensityParams.y*h + _DensityParams.z*f, 0.0, 1.0);
}

float map5(in float3 p)
{
	float3 q = p - _WindSpeed*_Time.y;
	float f;
	f = 0.50000*noise(q);
	q = q*_QScales.x;
	f += 0.25000*noise(q); 
	q = q*_QScales.y;
	f += 0.12500*noise(q);
	q = q*_QScales.z;
	f += 0.06250*noise(q);
	q = q*_QScales.w;
	f += 0.03125*noise(q);
	return clampDensity(p.y,f);
}

float map4(in float3 p)
{
	float3 q = p - _WindSpeed*_Time.y;
	float f;
	f = 0.50000*noise(q); 
	q = q*_QScales.x;
	f += 0.25000*noise(q);
	q = q*_QScales.y;
	f += 0.12500*noise(q);
	q = q*_QScales.z;
	f += 0.06250*noise(q);
	return clampDensity(p.y,f);
}

float map3(in float3 p)
{
	float3 q = p - _WindSpeed*_Time.y;
	float f;
	f = 0.50000*noise(q); 
	q = q*_QScales.x;
	f += 0.25000*noise(q);
	q = q*_QScales.y;
	f += 0.12500*noise(q);
	return clampDensity(p.y,f);
}

float map2(in float3 p)
{
	float3 q = p - _WindSpeed*_Time.y;
	float f;
	f = 0.50000*noise(q);
    q = q*_QScales.x;
	f += 0.25000*noise(q);
	return clampDensity(p.y,f);
}

float4 integrate(in float4 sum, in float dif, in float den, in float3 bgcol, in float t,out float4 cloudCol)
{
	float3 lightColor = _AmbientColor*_AmbientParam + _DiffuseColor*dif;
	cloudCol = float4(lerp(_LowDensityColor, _HighDensityColor, den), den);
	cloudCol.xyz *= lightColor;
	cloudCol.xyz = lerp(cloudCol.xyz, bgcol, 1.0 - exp(-_DensityParams.w*t*t));
	cloudCol.a *= 0.4;
	cloudCol.rgb *= cloudCol.a;
	return sum + cloudCol*(1.0 - sum.a);
}

#define EXP_MARCH(STEPS,MAP) \
i = 0; \
for(;i<STEPS;i++,e_pow+=ray_step) \
{ \
	float t = e_pow; \
	foot = foot_start + t*dir.xyz; \
	float den = MAP(foot); \
	if(den>0.01) \
	{ \
		float diff = clamp((den - MAP(foot + 0.3*_WorldSpaceLightPos0)) / 0.6, 0.0, 1.0); \
		sum = integrate(sum, diff, den, bgcol, t, cloud_color); \
	} \
} \

float4 raymarch(in float3 ro, in float3 rd, in float3 worldPos,in float3 bgcol)
{
    const half4 zeros = half4(0.0,0.0,0.0,0.0);  

    //cross check
    if((ro.y>_HeightDatas.y && worldPos.y>_HeightDatas.y) || (ro.y<-_HeightDatas.y && worldPos.y<-_HeightDatas.y))
    {
        return zeros;
    }

    float3 dir_cam_obj = worldPos-ro;
    float dist_cam_obj = length(dir_cam_obj);

    //near cut
    float delta = length(dir_cam_obj.xz);
    float2 dir_cam_obj_xz_nrmed = dir_cam_obj.xz/delta;
    delta /= dir_cam_obj.y;

    float h_near = clamp(ro.y,-_HeightDatas.y,_HeightDatas.y);
    float xh = delta*(ro.y-h_near);
    float2 xz = ro.xz-dir_cam_obj_xz_nrmed*xh;
    float3 nearCut = float3(xz.x,h_near,xz.y);

    //cloud start after object?
    float disToCloud = distance(nearCut,ro);
    if(disToCloud>=dist_cam_obj)
    {
        return zeros;
    }

    //far cut
	float h_far = _HeightDatas.y*sign(delta);

    xh = delta*(h_far-ro.y);
    float2 xzf = ro.xz+dir_cam_obj_xz_nrmed*xh;
    float3 farCut = float3(xzf.x,h_far,xzf.y);

    //cloud density
    float cloudThickness = distance(nearCut,farCut);
    cloudThickness = min(cloudThickness,dist_cam_obj-disToCloud);
    if(cloudThickness<=0)
    {
        return zeros;
    }

	cloudThickness = min(_OptimizeDatas.w, cloudThickness);

	float log_cloud_thickness = max(cloudThickness, 0);
	float ray_step = log_cloud_thickness*_OptimizeDatas.x;
    ray_step = max(ray_step,0.005);

    float3 dir = float3(dir_cam_obj/dist_cam_obj);
	float step_count = log_cloud_thickness/ray_step;	
    
	float3 foot = nearCut;

    //Ray-march
    half4 sum = zeros;
	half4 cloud_color = zeros;

	float3 foot_start = foot;
	float e_pow = 0;
	int i = 0;

	step_count *= 0.25;

	EXP_MARCH(step_count, map5);
	EXP_MARCH(step_count, map4);
	EXP_MARCH(step_count, map3);
	EXP_MARCH(step_count, map2);

	return clamp(sum, 0.0, 1.0);
}

struct appdata
{
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
	UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
	float4 pos : SV_POSITION;
	float2 uv  : TEXCOORD0;
	float3 cameraToFarPlane : TEXCOORD1;
   	float2 depthUV          : TEXCOORD2;
  	float2 depthUVNonStereo : TEXCOORD3;
	UNITY_VERTEX_INPUT_INSTANCE_ID
	UNITY_VERTEX_OUTPUT_STEREO
};

inline float getDepth(v2f i)
{
#if FOG_ORTHO
	float depth01 = UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.depthUV));
#if UNITY_REVERSED_Z
	depth01 = 1.0 - depth01;
#endif
#else
	float depth01 = Linear01Depth(UNITY_SAMPLE_DEPTH(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthTexture, i.depthUV)));
#endif
	return depth01;
}

float3 cs_camera_pos;     //Camera Pos In Cloud Space

// Misc functions
float3 getWorldPos(v2f i, float depth01)
{
	cs_camera_pos = float3(_WorldSpaceCameraPos.x, _WorldSpaceCameraPos.y-_HeightDatas.x, _WorldSpaceCameraPos.z);
	float3 worldPos = (i.cameraToFarPlane * depth01) + cs_camera_pos;
	worldPos.y += 0.00001;
	return worldPos;
}

// the Vertex Shader
v2f vert(appdata v)
{
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v);
	UNITY_TRANSFER_INSTANCE_ID(v, o);
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

	o.pos = UnityObjectToClipPos(v.vertex);
	o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord, _MainTex_ST);

   	o.depthUV = o.uv;
	o.depthUVNonStereo = v.texcoord;

#if UNITY_UV_STARTS_AT_TOP
	if (_MainTex_TexelSize.y < 0) 
    {
		o.depthUV.y = 1.0 - o.depthUV.y;
		o.depthUVNonStereo.y = 1.0 - o.depthUVNonStereo.y;
	}
#endif

	float2 clipXY = o.pos.xy / o.pos.w;
	float4 farPlaneClip = float4(clipXY, 1.0, 1.0);
	farPlaneClip.y *= _ProjectionParams.x;

#if UNITY_SINGLE_PASS_STEREO
	_ClipToWorld = mul(_ClipToWorld, unity_CameraInvProjection);
#endif
	float4 farPlaneWorld4 = mul(_ClipToWorld, farPlaneClip);
	float3 farPlaneWorld = farPlaneWorld4.xyz / farPlaneWorld4.w;

	o.cameraToFarPlane = farPlaneWorld - _WorldSpaceCameraPos;

	return o;
}

fixed4 frag(v2f i) : SV_Target
{
	float depthOpaque = getDepth(i);
	float3 worldPos = getWorldPos(i, depthOpaque);

	fixed3 col = tex2D(_MainTex,i.uv);

	float3 ro = cs_camera_pos;
	float3 rd = normalize(i.cameraToFarPlane);
	fixed4 res = raymarch(ro, rd, worldPos,col);
    col = col*saturate(1 - res.w) + res.xyz;

	float sun = saturate(dot(_WorldSpaceLightPos0, rd));
	col += _SunGlowIntensity*_SunGlowColor*pow(sun, _SunGlowPower);

	return fixed4(col, 1);
}