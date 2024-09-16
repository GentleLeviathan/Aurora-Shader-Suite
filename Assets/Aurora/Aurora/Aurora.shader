/*

MIT License

Copyright (c) 2024 GentleLeviathan

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

*/


Shader "GentleShaders/Aurora"
{
	Properties
	{
		//Textures
        _MainTex ("Diffuse 0 (RGB)", 2D) = "white" {}
        [Normal]_BumpMap ("Normal 0", 2D) = "bump" {}
        _CC ("Color Control 0", 2D) = "red" {}
        _Aurora ("Aurora 0", 2D) = "white" {}
		_RaveCC ("Rave CC 0 (RGBA)", 2D) = "black"{}

        _MainTex1 ("Diffuse 1 (RGB)", 2D) = "white" {}
        [Normal]_BumpMap1 ("Normal 1", 2D) = "bump" {}
        _CC1 ("Color Control 1", 2D) = "black" {}
        _Aurora1 ("Aurora 1", 2D) = "white" {}
		_RaveCC1 ("Rave CC 1 (RGBA)", 2D) = "black"{}

        _MainTex2 ("Diffuse 2 (RGB)", 2D) = "white" {}
        [Normal]_BumpMap2 ("Normal 2", 2D) = "bump" {}
        _CC2 ("Color Control 2", 2D) = "black" {}
        _Aurora2 ("Aurora 2", 2D) = "white" {}
		_RaveCC2 ("Rave CC 2 (RGBA)", 2D) = "black"{}

        _MainTex3 ("Diffuse 3 (RGB)", 2D) = "white" {}
        [Normal]_BumpMap3 ("Normal 3", 2D) = "bump" {}
        _CC3 ("Color Control 3", 2D) = "black" {}
        _Aurora3 ("Aurora 3", 2D) = "white" {}
		_RaveCC3 ("Rave CC 3 (RGBA)", 2D) = "black"{}

        _MainTex4 ("Diffuse 4 (RGB)", 2D) = "white" {}
        [Normal]_BumpMap4 ("Normal 4", 2D) = "bump" {}
        _CC4 ("Color Control 4", 2D) = "black" {}
        _Aurora4 ("Aurora 4", 2D) = "white" {}
		_RaveCC4 ("Rave CC 4 (RGBA)", 2D) = "black"{}

		_Decals("Decal Texture (UV2)", 2D) = "black" {}
		_DecalNormal("Decal Normal (UV2)", 2D) = "bump" {}

		//Colors
		_Color("Color", Color) = (1,1,1,1)
		_SecondaryColor("Secondary Color", Color) = (0,1,0,1)
		_TertiaryColor("Tertiary Color", Color) = (1,1,1,1)
		[HDR]_IllumColor("Illumination Color", Color) = (0,0,0,1)

		//Values
		_trueMetallic("Metalness", Range(0, 1)) = 1
		_Roughness("Roughness", Range(0 , 1)) = 0.0
		_Deepness("Color Deepness", Range(0,1)) = 0
		_alphaCutoff("Alpha Cutoff", Range(0, 1)) = 0.5

		//Toggles
		_ColorTexture("Color Texture", Int) = 0
		_cutoffDisable("Cutoff Disabled", Int) = 1

		//Rave Section
		[HDR]_RaveColor("Rave Color", Color) = (1,0,0,1)
		[HDR]_RaveSecondaryColor("Rave Secondary Color", Color) = (0,1,0,1)
		[HDR]_RaveTertiaryColor("Rave Tertiary Color", Color) = (1,1,1,1)
		[HDR]_RaveQuaternaryColor("Rave Quaternary Color", Color) = (1,1,1,1)

		_RaveMask("Rave Mask (RGBA)", 2D) = "white" {}
		_RaveRG("Rave Scroll R+G", Vector) = (1,1,1,1)
		_RaveBA("Rave Scroll B+A", Vector) = (1,1,1,1)

		//Alt Toggles
		_DetailStrength("Detail Map Strength", Range(0, 2)) = 0.5
		_lightingBypass("bypassLighting", Int) = 0
		_accountForBLSH("Account for BLSH?", Int) = 1
		_giBoost("ambientBoost", Int) = 0
		_uvMethodSwitch("altUVMethod", Int) = 0

		//AudioLink properties
		_useALThemeColor0("AL Theme Color 0", Int) = 0
		_useALThemeColor1("AL Theme Color 1", Int) = 0
		_useALThemeColor2("AL Theme Color 2", Int) = 0
		_useALThemeColor3("AL Theme Color 3", Int) = 0

		_chronotensityScroll0("Chronotensity Scroll 0", Int) = 0
		_chronotensityScroll1("Chronotensity Scroll 0", Int) = 0
		_chronotensityScroll2("Chronotensity Scroll 0", Int) = 0
		_chronotensityScroll3("Chronotensity Scroll 0", Int) = 0

		_audioLinkExclusive0("AL Exclusive 0", Int) = 0
		_audioLinkExclusive1("AL Exclusive 0", Int) = 0
		_audioLinkExclusive2("AL Exclusive 0", Int) = 0
		_audioLinkExclusive3("AL Exclusive 0", Int) = 0
		
		_audioLinkAdd0("Add AL Value 0", Int) = 1
		_audioLinkAdd1("Add AL Value 1", Int) = 1
		_audioLinkAdd2("Add AL Value 2", Int) = 1
		_audioLinkAdd3("Add AL Value 3", Int) = 1

		//Blending
		[HideInInspector] _SrcBlend ("_srcBlend", Float) = 1.0
		[HideInInspector] _DstBlend ("_dstBlend", Float) = 10.0
		[HideInInspector] _ZWrite ("_zWrite", Float) = 1.0
		[HideInInspector] _ZTest ("_zTest", Float) = 4.0
	}

    SubShader
    {
		Tags { "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Blend [_SrcBlend] [_DstBlend]
		Cull Back
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		CGPROGRAM

		#include "../CgInc/AuroraCommon.cginc"
		#include "../CgInc/AuroraBRDF.cginc"
		#include "../CgInc/AuroraFiveX.cginc"
		#pragma target 4.0
		#pragma only_renderers d3d11 glcore gles
		#pragma surface surface Aurora addshadow fullforwardshadows keepalpha

		//local features
		#pragma shader_feature_local _DECALS			//2
		#pragma shader_feature_local _RAVE				//4
		#pragma shader_feature_local _VRCAUDIOLINK		//8

		#pragma shader_feature_local _U1				//16
        #pragma shader_feature_local _U2				//32
        #pragma shader_feature_local _U3				//64
        #pragma shader_feature_local _U4				//128

		#ifdef _VRCAUDIOLINK
			#include "../CgInc/AudioLink/AuroraAL.cginc"
		#endif

		uniform uint _lightingBypass;
		uniform uint _accountForBLSH;
		uniform uint _giBoost;
		uniform uint _uvMethodSwitch;
		uniform uint _cutoffDisable;

		void LightingAurora_GI (SurfaceOutput s, UnityGIInput data, inout UnityGI gi)
		{
			#if defined(UNITY_PASS_DEFERRED) && UNITY_ENABLE_REFLECTION_BUFFERS
				gi = UnityGlobalIllumination(data, s.Specular, s.Normal);
			#else
				float2 unpack = unpack_floats_4bpf(s.Alpha);
				Unity_GlossyEnvironmentData g = UnityGlossyEnvironmentSetup(1.0 - s.Gloss, data.worldViewDir, s.Normal, lerp(unity_ColorSpaceDielectricSpec.rgb, s.Albedo, unpack.r));
				gi = UnityGlobalIllumination(data, s.Specular, s.Normal, g);
				half3 probeLightDir = AuroraBLSHAccount(data, gi, _accountForBLSH);
			#endif
		}

		float4 LightingAurora(SurfaceOutput s, float3 viewDir, UnityGI gi)
		{
			s.Normal = normalize(s.Normal);

			half oneMinusReflectivity;
			half3 specColor;
			float2 unpack = unpack_floats_4bpf(s.Alpha);

			unpack.r *= 1.7 - 0.7 * unpack.r;
			half3 diffuse = DiffuseAndSpecularFromMetallic (s.Albedo, unpack.r, /*out*/ specColor, /*out*/ oneMinusReflectivity);

			half4 c;
			c = AuroraBRDF4(s.Albedo, diffuse, specColor, unpack.g, oneMinusReflectivity, unpack.r, s.Gloss, s.Specular, s.Normal, viewDir, gi.light, gi.indirect, _lightingBypass, _giBoost);
			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv2_Decals;
			float2 uv_RaveMask;
		};

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

		uint _TileCount;
        
		void surface( Input i, inout SurfaceOutput o )
		{
			_TileCount = 1;
            #ifdef _U1
                _TileCount += _uvMethodSwitch;
            #endif
            #ifdef _U2
				_TileCount += _uvMethodSwitch;
            #endif
            #ifdef _U3
                _TileCount += _uvMethodSwitch;
            #endif
            #ifdef _U4
                _TileCount += _uvMethodSwitch;
            #endif

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

			//out
			o.Albedo = finalColor;
			o.Normal = finalNormal;
			o.Specular = occlusionTex;
			o.Gloss = calculatedRoughness;
			o.Emission = illumination;
			#ifdef _RAVE
				o.Emission += raveColor;
			#endif
			o.Alpha = pack_floats_4bpf(displayMetalProperty, finalColor.a);
			clip((finalColor.a - _alphaCutoff) + _cutoffDisable);
		}

		ENDCG
	}
	

	Fallback "Diffuse"
	CustomEditor "GentleShaders.Aurora.AuroraEditor"
}