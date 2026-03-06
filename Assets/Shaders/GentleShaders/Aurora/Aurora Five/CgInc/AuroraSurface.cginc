void surface( Input i, inout AuroraOutput o )
{
	_TileCount = 1;

    //UV Setup
    float2 uv_d = float2(i.uv_MainTex.x * _TileCount, i.uv_MainTex.y);
    float2 uv0 = GetUV(_TileCount, 0, uv_d);
    #ifdef _U1
        float2 uv1 = GetUV(_TileCount, 1, uv_d);
    #endif
    #ifdef _U2
        float2 uv2 = GetUV(_TileCount, 2, uv_d);
    #endif
    #ifdef _U3
        float2 uv3 = GetUV(_TileCount, 3, uv_d);
    #endif
    #ifdef _U4
        float2 uv4 = GetUV(_TileCount, 4, uv_d);
    #endif

	fixed4 tempNormal = half4(0,0,0,1);
	#ifdef _U1
        fixed4 c1 = UNITY_SAMPLE_TEX2D_SAMPLER(_MainTex1, _MainTex, uv1);
        fixed4 cc1 = UNITY_SAMPLE_TEX2D_SAMPLER(_CC1, _CC, uv1);
        fixed4 a1 = UNITY_SAMPLE_TEX2D_SAMPLER(_Aurora1, _Aurora, uv1);
        tempNormal = UNITY_SAMPLE_TEX2D_SAMPLER(_BumpMap1, _BumpMap, uv1);
        tempNormal.g = 1.0 - tempNormal.g;
        fixed3 n1 = UnpackNormal(tempNormal);
		#ifdef _RAVE
			fixed4 rave1 = UNITY_SAMPLE_TEX2D_SAMPLER(_RaveCC1, _RaveCC, uv1);
		#endif
    #endif
    #ifdef _U2
        fixed4 c2 = UNITY_SAMPLE_TEX2D_SAMPLER(_MainTex2, _MainTex, uv2);
        fixed4 cc2 = UNITY_SAMPLE_TEX2D_SAMPLER(_CC2, _CC, uv2);
        fixed4 a2 = UNITY_SAMPLE_TEX2D_SAMPLER(_Aurora2, _Aurora, uv2);
        tempNormal = UNITY_SAMPLE_TEX2D_SAMPLER(_BumpMap2, _BumpMap, uv2);
        tempNormal.g = 1.0 - tempNormal.g;
        fixed3 n2 = UnpackNormal(tempNormal);
		#ifdef _RAVE
			fixed4 rave2 = UNITY_SAMPLE_TEX2D_SAMPLER(_RaveCC2, _RaveCC, uv2);
		#endif
    #endif
    #ifdef _U3
        fixed4 c3 = UNITY_SAMPLE_TEX2D_SAMPLER(_MainTex3, _MainTex, uv3);
        fixed4 cc3 = UNITY_SAMPLE_TEX2D_SAMPLER(_CC3, _CC, uv3);
        fixed4 a3 = UNITY_SAMPLE_TEX2D_SAMPLER(_Aurora3, _Aurora, uv3);
        tempNormal = UNITY_SAMPLE_TEX2D_SAMPLER(_BumpMap3, _BumpMap, uv3);
        tempNormal.g = 1.0 - tempNormal.g;
        fixed3 n3 = UnpackNormal(tempNormal);
		#ifdef _RAVE
			fixed4 rave3 = UNITY_SAMPLE_TEX2D_SAMPLER(_RaveCC3, _RaveCC, uv3);
		#endif
    #endif
    #ifdef _U4
        fixed4 c4 = UNITY_SAMPLE_TEX2D_SAMPLER(_MainTex4, _MainTex, uv4);
        fixed4 cc4 = UNITY_SAMPLE_TEX2D_SAMPLER(_CC4, _CC, uv4);
        fixed4 a4 = UNITY_SAMPLE_TEX2D_SAMPLER(_Aurora4, _Aurora, uv4);
        tempNormal = UNITY_SAMPLE_TEX2D_SAMPLER(_BumpMap4, _BumpMap, uv4);
        tempNormal.g = 1.0 - tempNormal.g;
        fixed3 n4 = UnpackNormal(tempNormal);
		#ifdef _RAVE
			fixed4 rave4 = UNITY_SAMPLE_TEX2D_SAMPLER(_RaveCC4, _RaveCC, uv4);
		#endif
    #endif

	fixed4 diffuse = UNITY_SAMPLE_TEX2D(_MainTex, uv0);
    fixed4 aurora = UNITY_SAMPLE_TEX2D(_Aurora, uv0);
    fixed4 cc = UNITY_SAMPLE_TEX2D(_CC, uv0);
	#ifdef _RAVE
		fixed4 raveCC = UNITY_SAMPLE_TEX2D(_RaveCC, uv0);
	#endif

	float4 normal = UNITY_SAMPLE_TEX2D(_BumpMap, uv0);
	float3 finalNormal = normalize(UnpackNormal(float4(normal.r, 1 - normal.g, normal.b, normal.a)));

    //MULTIPLIERS
    int tempMultiplier = GetMultiplier(_TileCount, 0, i.uv_MainTex);
    diffuse *= tempMultiplier;
    cc *= tempMultiplier;
    aurora *= tempMultiplier;
	#ifdef _RAVE
		raveCC *= tempMultiplier;
	#endif

    finalNormal *= tempMultiplier;
    #ifdef _U1
        tempMultiplier = GetMultiplier(_TileCount, 1, i.uv_MainTex);
        c1 *= tempMultiplier;
        diffuse += c1;

        n1 *= tempMultiplier;
        finalNormal += n1;

        cc1 *= tempMultiplier;
        cc += cc1;

        a1 *= tempMultiplier;
        aurora += a1;

		#ifdef _RAVE
			rave1 *= tempMultiplier;
			raveCC += rave1;
		#endif
    #endif
    #ifdef _U2
        tempMultiplier = GetMultiplier(_TileCount, 2, i.uv_MainTex);
        c2 *= tempMultiplier;
        diffuse += c2;

        n2 *= tempMultiplier;
        finalNormal += n2;

        cc2 *= tempMultiplier;
        cc += cc2;

        a2 *= tempMultiplier;
        aurora += a2;

		#ifdef _RAVE
			rave2 *= tempMultiplier;
			raveCC += rave2;
		#endif
    #endif
    #ifdef _U3
        tempMultiplier = GetMultiplier(_TileCount, 3, i.uv_MainTex);
        c3 *= tempMultiplier;
        diffuse += c3;

        n3 *= tempMultiplier;
        finalNormal += n3;

        cc3 *= tempMultiplier;
        cc += cc3;

        a3 *= tempMultiplier;
        aurora += a3;

		#ifdef _RAVE
			rave3 *= tempMultiplier;
			raveCC += rave3;
		#endif
    #endif
    #ifdef _U4
        tempMultiplier = GetMultiplier(_TileCount, 4, i.uv_MainTex);
        c4 *= tempMultiplier;
        diffuse += c4;

        n4 *= tempMultiplier;
        finalNormal += n4;

        cc4 *= tempMultiplier;
        cc += cc4;

        a4 *= tempMultiplier;
        aurora += a4;

		#ifdef _RAVE
			rave4 *= tempMultiplier;
			raveCC += rave4;
		#endif
    #endif

	//tex
	half illum = aurora.b;
	half occlusionTex = aurora.a;

	//desaturation
	diffuse.xyz = lerp(diffuse.xyz, Luminance(diffuse.xyz), _ColorTexture);
	half4 black = half4(0,0,0,0);
	half4 white = half4(1,1,1,0);

	//color 'deepness'
	half4 stockDiffuse = diffuse;
	diffuse.rgb = lerp(diffuse.rgb, diffuse.rgb * diffuse.rgb, _Deepness);

	//masking
	half3 primary = lerp(black, diffuse * max(0.075, _Color), cc.r);
	half3 secondary = lerp(black, diffuse * max(0.075, _SecondaryColor), cc.g);
	half3 tertiary = lerp(black, diffuse * max(0.075, _TertiaryColor), cc.b);
	half3 passthrough = lerp(diffuse, black, cc.r + cc.g + cc.b);

	//roughness
	float rough = max(1.055 * pow(aurora.g, 0.416666667) - 0.055, 0); //Linear to sRGB conversion!
	half calculatedRoughness = lerp(rough, 1.0, _Roughness);

	//others
	half4 illumination = illum * _IllumColor;
	half displayMetalProperty = aurora.r * _trueMetallic;

	//finalColor
	half4 finalColor = half4(primary + secondary + tertiary + passthrough, diffuse.a);
	#ifdef _DECALS
		half4 decals = tex2D(_Decals, i.uv2_Decals.xy);
		half3 decalsNormal = UnpackNormal(tex2D(_DecalNormal, i.uv2_Decals.xy));
		finalColor = lerp(finalColor, half4(decals.rgb, finalColor.a), decals.a);
		finalNormal += normalize(decalsNormal);
	#endif

	//rave section
	#ifdef _RAVE
		half4 raveColor = black;

		half4 time = _Time.xxxx;

		#ifdef _VRCAUDIOLINK
			//this slows the scrolling down to 10% of it's previous value, and then adds the chronotensity time (equivalent to _Time.x) on top
			//this means that at max chronotensity.. intensity? the 'time' value will increase at the same rate as _Time.x 
			time.x *= 1.0 - (_chronotensityScroll0 * 0.9);
			time.x += (AudioLinkGetChronoTime(0, 0) * 0.05) * _chronotensityScroll0;

			time.y *= 1.0 - (_chronotensityScroll1 * 0.9);
			time.y += (AudioLinkGetChronoTime(0, 1) * 0.05) * _chronotensityScroll1;

			time.z *= 1.0 - (_chronotensityScroll2 * 0.9);
			time.z += (AudioLinkGetChronoTime(0, 0) * 0.05) * _chronotensityScroll2;

			time.w *= 1.0 - (_chronotensityScroll3 * 0.9);
			time.w += (AudioLinkGetChronoTime(0, 2) * 0.05) * _chronotensityScroll3;
		#endif

		half raveR = tex2D(_RaveMask, i.uv_RaveMask + half2(_Time.x * _RaveRG.r, time.x * _RaveRG.g)).r;
		half raveG = tex2D(_RaveMask, i.uv_RaveMask + half2(_Time.x * _RaveRG.b, time.y * _RaveRG.a)).g;
		half raveB = tex2D(_RaveMask, i.uv_RaveMask + half2(_Time.x * _RaveBA.r, time.z * _RaveBA.g)).b;
		half raveA = tex2D(_RaveMask, i.uv_RaveMask + half2(_Time.x * _RaveBA.b, time.w * _RaveBA.a)).a;

		half4 raveColor0 = _RaveColor;
		half4 raveColor1 = _RaveSecondaryColor;
		half4 raveColor2 = _RaveTertiaryColor;
		half4 raveColor3 = _RaveQuaternaryColor;

		#ifdef _VRCAUDIOLINK
			//check if audiolink is available in the environment
			int ALPresent = step(0.1, AudioLinkDecodeDataAsUInt(ALPASS_GENERALVU_INSTANCE_TIME));
			_audioLinkExclusive0 = saturate(_audioLinkExclusive0 - ALPresent);
			_audioLinkExclusive1 = saturate(_audioLinkExclusive1 - ALPresent);
			_audioLinkExclusive2 = saturate(_audioLinkExclusive2 - ALPresent);
			_audioLinkExclusive3 = saturate(_audioLinkExclusive3 - ALPresent);

			half4 audioLink = AudioLinkData(ALPASS_AUDIOLINK + int2(0, i.uv_MainTex.y));
			raveR *= 0.75;
			raveR += audioLink.r * 0.25 * _audioLinkAdd0;
			raveR = saturate(raveR);
			raveR *= (1.0 - _audioLinkExclusive0);
			//Going to gamma and back again to get the equivalent of an HDR 'intensity' of 2 in the color picker
			half4 themeColor0 = black;
			themeColor0.rgb = LinearToGammaSpace(AudioLinkData(ALPASS_THEME_COLOR0)) * 4;
			themeColor0.rgb = GammaToLinearSpace(themeColor0.rgb);
			raveColor0 = lerp(_RaveColor, themeColor0, _useALThemeColor0 * step(0.1, Luminance(themeColor0)));

			raveG *= 0.75;
			raveG += audioLink.g * 0.25 * _audioLinkAdd1;
			raveG = saturate(raveG);
			raveG *= (1.0 - _audioLinkExclusive1);
			//Going to gamma and back again to get the equivalent of an HDR 'intensity' of 2 in the color picker
			half4 themeColor1 = black;
			themeColor1.rgb = LinearToGammaSpace(AudioLinkData(ALPASS_THEME_COLOR1)) * 4;
			themeColor1.rgb = GammaToLinearSpace(themeColor1.rgb);
			raveColor1 = lerp(_RaveSecondaryColor, themeColor1, _useALThemeColor1 * step(0.1, Luminance(themeColor1)));
					
			raveB *= 0.75;
			raveB += audioLink.b * 0.25 * _audioLinkAdd2;
			raveB = saturate(raveB);
			raveB *= (1.0 - _audioLinkExclusive2);
			//Going to gamma and back again to get the equivalent of an HDR 'intensity' of 2 in the color picker
			half4 themeColor2 = black;
			themeColor2.rgb = LinearToGammaSpace(AudioLinkData(ALPASS_THEME_COLOR2)) * 4;
			themeColor2.rgb = GammaToLinearSpace(themeColor2.rgb);
			raveColor2 = lerp(_RaveTertiaryColor, themeColor2, _useALThemeColor2 * step(0.1, Luminance(themeColor2)));

			raveA *= 0.75;
			raveA += audioLink.r * 0.25 * _audioLinkAdd3;
			raveA = saturate(raveA);
			raveA *= (1.0 - _audioLinkExclusive3);
			//Going to gamma and back again to get the equivalent of an HDR 'intensity' of 2 in the color picker
			half4 themeColor3 = black;
			themeColor3.rgb = LinearToGammaSpace(AudioLinkData(ALPASS_THEME_COLOR3)) * 4;
			themeColor3.rgb = GammaToLinearSpace(themeColor3.rgb);
			raveColor3 = lerp(_RaveQuaternaryColor, themeColor3, _useALThemeColor3 * step(0.1, Luminance(themeColor3)));
		#endif

		raveColor += half4(raveCC.r, raveCC.r, raveCC.r, 1) * raveColor0 * raveR;
		raveColor += half4(raveCC.g, raveCC.g, raveCC.g, 1) * raveColor1 * raveG;
		raveColor += half4(raveCC.b, raveCC.b, raveCC.b, 1) * raveColor2 * raveB;
		raveColor += half4(raveCC.a, raveCC.a, raveCC.a, 1) * raveColor3 * raveA;
	#endif

	// We only use the hessian input (tangent space vectors) here because we are trying to enhance normal detail, not mesh detail
	float weight = 1.0;
	float curve = curvature(weight, finalNormal, _RSGain);

	float finalOcclusion = lerp(1.0, pow(occlusionTex, max(_occlusionStrength, 1)), saturate(_occlusionStrength));

	o.Albedo = finalColor;
	o.Normal = finalNormal;
	o.Occlusion = finalOcclusion;
	o.Metal = displayMetalProperty;
	o.Roughness = calculatedRoughness;
	o.RadianceScaling = lerp(1.0, greyDescriptor(curve, 1.0) * UNITY_PI * 0.5, _RSStrength);
	o.Emission = illumination;
	#ifdef _RAVE
		o.Emission += raveColor;
	#endif
	o.Alpha = finalColor.a;
}