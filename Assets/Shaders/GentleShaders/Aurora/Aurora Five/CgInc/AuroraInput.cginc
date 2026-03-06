	struct Input
	{
		float2 uv_MainTex;
		float2 uv2_Decals;
		float2 uv_RaveMask;
		float3 viewDir;
		float3 worldRefl; INTERNAL_DATA
		//float3 worldNormal; INTERNAL_DATA
	};
	
	uniform uint _cutoffDisable;

	SamplerState sampler_linear_repeat;

    UNITY_DECLARE_TEX2D(_MainTex);
	UNITY_DECLARE_TEX2D(_BumpMap);
	UNITY_DECLARE_TEX2D(_CC);
	UNITY_DECLARE_TEX2D(_Aurora);

	#ifdef _DECALS
		uniform sampler2D _Decals;
		uniform sampler2D _DecalNormal;
	#endif

	#ifdef _RAVE
		UNITY_DECLARE_TEX2D(_RaveCC);
		uniform sampler2D _RaveMask;
		uniform half4 _RaveColor;
		uniform half4 _RaveSecondaryColor;
		uniform half4 _RaveTertiaryColor;
		uniform half4 _RaveQuaternaryColor;
		uniform half4 _RaveRG;
		uniform half4 _RaveBA;
	#endif

	#ifdef _U1
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex1);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap1);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CC1);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Aurora1);
		#ifdef _RAVE
			UNITY_DECLARE_TEX2D_NOSAMPLER(_RaveCC1);
		#endif
    #endif

    #ifdef _U2
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex2);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap2);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CC2);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Aurora2);
		#ifdef _RAVE
			UNITY_DECLARE_TEX2D_NOSAMPLER(_RaveCC2);
		#endif
    #endif

    #ifdef _U3
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex3);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap3);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CC3);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Aurora3);
		#ifdef _RAVE
			UNITY_DECLARE_TEX2D_NOSAMPLER(_RaveCC3);
		#endif
    #endif

    #ifdef _U4
		UNITY_DECLARE_TEX2D_NOSAMPLER(_MainTex4);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_BumpMap4);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_CC4);
		UNITY_DECLARE_TEX2D_NOSAMPLER(_Aurora4);
		#ifdef _RAVE
			UNITY_DECLARE_TEX2D_NOSAMPLER(_RaveCC4);
		#endif
    #endif

	uniform half4 _Color;
	uniform half4 _SecondaryColor;
	uniform half4 _TertiaryColor;
	uniform half4 _IllumColor;
	uniform half _Roughness;
	uniform half _Deepness;
	uniform half _DetailStrength;

	uniform fixed _trueMetallic;
	uniform fixed _alphaCutoff;
	uniform fixed _ColorTexture;
	uniform fixed _BlendMode;
	uniform fixed _occlusionStrength;

	uniform uint _useALThemeColor0;
	uniform uint _useALThemeColor1;
	uniform uint _useALThemeColor2;
	uniform uint _useALThemeColor3;

	uniform int _audioLinkExclusive0 = 1;
	uniform int _audioLinkExclusive1;
	uniform int _audioLinkExclusive2;
	uniform int _audioLinkExclusive3;

	uniform uint _chronotensityScroll0;
	uniform uint _chronotensityScroll1;
	uniform uint _chronotensityScroll2;
	uniform uint _chronotensityScroll3;
		
	uniform uint _audioLinkAdd0;
	uniform uint _audioLinkAdd1;
	uniform uint _audioLinkAdd2;
	uniform uint _audioLinkAdd3;

	uniform fixed _RSStrength;
	uniform fixed _RSGain;

	uint _TileCount;