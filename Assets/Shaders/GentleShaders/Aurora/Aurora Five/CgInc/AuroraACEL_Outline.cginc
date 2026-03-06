#pragma vertex vert
#pragma fragment frag
// make fog work
//#pragma multi_compile_fog
#pragma shader_feature_local _ACEL	//2

#include "UnityCG.cginc"

struct appdata
{
	float4 vertex : POSITION;
	float3 normal : NORMAL;
};

struct v2f
{
	//UNITY_FOG_COORDS(1)
	float4 vertex : SV_POSITION;
};

uniform fixed _ACELOutlineWidth;

v2f vert (appdata v)
{
	v2f o;
	
	// This should result in being clipped by the near plane and the frag shader shouldn't run at all
	// Hoping to find a better solution for this at a later time
	o.vertex.xyz = _WorldSpaceCameraPos;
	o.vertex.w = 0;
	#ifdef _ACEL
		v.vertex.xyz += v.normal * _ACELOutlineWidth * 0.00005;
		o.vertex = UnityObjectToClipPos(v.vertex);
	#endif
	//UNITY_TRANSFER_FOG(o,o.vertex);
	return o;
}

fixed4 frag (v2f i) : SV_Target
{
	fixed4 col = half4(0,0,0,1);
	//UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}