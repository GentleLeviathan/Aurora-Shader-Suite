/*

MIT License

Copyright (c) 2021 GentleLeviathan

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


Shader "GentleShaders/Aurora_AR2"
{
	Properties
	{
		//Textures
		_MainTex("Diffuse", 2D) = "white" {}
		_CC("CC", 2D) = "red" {}
		[Normal]_BumpMap("Normal Map", 2D) = "bump" {}
		_Aurora("Aurora Texture", 2D) = "black" {}
		_Pattern("Pattern Texture", 2D) = "white" {}
		_Decals("Decal Texture (UV2)", 2D) = "black" {}
		_DecalNormal("Decal Normal (UV2)", 2D) = "bump" {}

		//Colors
		_Color("Color", Color) = (1,0,0,0)
		_SecondaryColor("Secondary Color", Color) = (0,1,0,0)
		_TertiaryColor("Tertiary Color", Color) = (1,1,1,1)
		[HDR]_IllumColor("Illumination Color", Color) = (0,0,0,0)

		//Values
		_trueMetallic("Metalness", Range(0, 1)) = 0.5
		_Roughness("Roughness", Range(0 , 1)) = 0.3
		_Deepness("Color Deepness", Range(0,1)) = 0

		//Toggles
		_ColorTexture("Color Texture", Int) = 0

		//Alt Textures
		_CubeReflection("Cubemapped Reflection", CUBE) = "" {}
		_DetailMap("Detail Map", 2D) = "white" {}
		_DetailNormal("Detail Normal", 2D) = "bump" {}

		//Rave Section
		_RaveCC("Rave CC (RGBA)", 2D) = "black" {}
		[HDR]_RaveColor("Rave Color", Color) = (1,0,0,0)
		[HDR]_RaveSecondaryColor("Rave Secondary Color", Color) = (0,1,0,0)
		[HDR]_RaveTertiaryColor("Rave Tertiary Color", Color) = (1,1,1,1)
		[HDR]_RaveQuaternaryColor("Rave Quaternary Color", Color) = (1,1,1,1)

		_RaveMask("Rave Mask (RGBA)", 2D) = "white" {}
		_RaveRG("Rave Scroll R+G", Vector) = (1,1,1,1)
		_RaveBA("Rave Scroll B+A", Vector) = (1,1,1,1)

		//Alt Toggles
		_DetailStrength("Detail Map Strength", Range(0, 2)) = 0.5
		_lightingBypass("bypassLighting", Int) = 0

		[Enum(Add,0,Subtract,1,Multiply,2,Divide,3)] _BlendMode ("Pattern Blend Mode", Float) = 0
	}

    SubShader
    {
		Tags { "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		ZWrite On
		ZTest LEqual
		CGPROGRAM

		#include "../../CgInc/GentleUtilities.cginc"
		#pragma target 4.0
		#pragma only_renderers d3d11 glcore gles
		#pragma surface surface FinalityCleanBRDF addshadow fullforwardshadows
		//local features
		#pragma shader_feature_local _DETAIL_TEXTURE
		#pragma shader_feature_local _CUBE_REFLECTION
		#pragma shader_feature_local _PATTERN
		#pragma shader_feature_local _ILLUMINATION
		#pragma shader_feature_local _SIMPLE_ROUGHNESS
		#pragma shader_feature_local _DECALS
		#pragma shader_feature_local _RAVE
		#pragma shader_feature_local _VRCAUDIOLINK

		#ifdef _VRCAUDIOLINK
			#include "../../CgInc/AudioLink/AuroraAL.cginc"
		#endif

		uniform fixed _lightingBypass;

		half4 LightingFinalityCleanBRDF(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			half3 h = normalize (lightDir + viewDir);
			half diff = max (0, dot (s.Normal, lightDir));
			half4 NdotL = dot(s.Normal, lightDir);
			half4 NdotH = dot(s.Normal, h);
			half4 NdotV = dot(s.Normal, viewDir);
			half4 VdotH = dot(viewDir, h);
			half4 LdotH = dot(lightDir, h);

			float specDist = BeckmannNDF(s.Gloss, NdotH) * 0.5;
			specDist += GGXNDF(s.Gloss, NdotH);

			float shadowDist = WalterEtAlGSF(NdotL, NdotV, s.Gloss);
			shadowDist = pow(shadowDist, 3);

			float fresnel = SphericalGFF(LdotH, s.Specular);

			half4 spec = (specDist * fresnel * shadowDist) / (1.0 * (  NdotL * NdotV));

			half3 hsvAlbedo = rgb2hsv(s.Albedo.rgb);
			half3 specMetalColor = hsv2rgb(half3(hsvAlbedo.r, hsvAlbedo.g, 1));
			spec = lerp(spec, spec * half4(specMetalColor, 1), s.Alpha);
			spec = saturate(spec);

			half4 c;
			c.rgb = (((s.Albedo + spec) * diff) * atten) * _LightColor0;
			c.a = 1;
			return c;
		}

		struct Input
		{
			float2 uv_MainTex;
			float2 uv2_Decals;
			float2 uv_BumpMap;
			float2 uv_MetallicTex;
			float2 uv_RoughnessTex;
			float2 uv_IllumTex;
			float2 uv_Occlusion;
			float2 uv_DetailMap;
			float2 uv_Pattern;
			half3 worldPos;
			half3 worldRefl; INTERNAL_DATA
			half3 worldNormal;
			half3 viewDir;
			half3 lightDir;
			half4 tangentDir;
		};

        uniform sampler2D _MainTex;

		#ifdef _DECALS
			uniform sampler2D _Decals;
			uniform sampler2D _DecalNormal;
		#endif

		uniform sampler2D _BumpMap;
		uniform sampler2D _CC;
		uniform sampler2D _Aurora;
		UNITY_DECLARE_TEXCUBE(_CubeReflection);

		#ifdef _DETAIL_TEXTURE
			uniform sampler2D _DetailMap;
			uniform sampler2D _DetailNormal;
		#endif

		#ifdef _PATTERN
			uniform sampler2D _Pattern;
		#endif

		#ifdef _RAVE
			uniform sampler2D _RaveCC;
			uniform sampler2D _RaveMask;
			uniform half4 _RaveColor;
			uniform half4 _RaveSecondaryColor;
			uniform half4 _RaveTertiaryColor;
			uniform half4 _RaveQuaternaryColor;
			uniform half4 _RaveRG;
			uniform half4 _RaveBA;
		#endif

		uniform half4 _Color;
		uniform half4 _SecondaryColor;
		uniform half4 _TertiaryColor;
		uniform half4 _IllumColor;
		uniform half _Roughness;
		uniform half _Deepness;
		uniform half _DetailStrength;

		uniform fixed _trueMetallic;
		uniform fixed _ColorTexture;
		uniform fixed _BlendMode;

        
		void surface( Input i, inout SurfaceOutput o )
		{
			//tex
			half4 diffuse = tex2D(_MainTex, i.uv_MainTex.xy);
			half4 cc = tex2D(_CC, i.uv_MainTex.xy);
			half4 auroraTex = tex2D(_Aurora, i.uv_MainTex.xy);
			#ifdef _ILLUMINATION
				half illum = auroraTex.b;
			#endif
			half occlusionTex = auroraTex.a;
			float4 normal = tex2D(_BumpMap, i.uv_MainTex.xy);
			float3 finalNormal = normalize(UnpackNormal(float4(normal.r, 1 - normal.g, normal.b, normal.a)));
			#ifdef _DETAIL_TEXTURE
				half4 detailTex = tex2D(_DetailMap, i.uv_DetailMap.xy) * _DetailStrength;
				float4 detailNormal = tex2D(_DetailNormal, i.uv_DetailMap.xy);
				finalNormal += UnpackNormal(detailNormal);
			#endif
			#ifdef _PATTERN
				half4 patternTex = tex2D(_Pattern, i.uv_Pattern.xy);
			#endif

			//desaturation
			diffuse.xyz = lerp(diffuse.xyz, Desaturate(diffuse.xyz), gT(_ColorTexture, 0));
			half4 black = half4(0,0,0,0);
			half4 white = half4(1,1,1,0);

			//color 'deepness'
			half4 stockDiffuse = diffuse;
			diffuse = lerp(diffuse, diffuse * diffuse, _Deepness);

			//masking
			half4 primary = lerp(black, diffuse * max(0.075, _Color), cc.r);
			half4 secondary = lerp(black, diffuse * max(0.075, _SecondaryColor), cc.g);
			half4 tertiary = lerp(black, diffuse * max(0.075, _TertiaryColor), cc.b);
			half4 passthrough = lerp(diffuse, black, cc.r + cc.g + cc.b);

			//Pattern
			#ifdef _PATTERN
				primary *= patternTex.r;
				secondary *= patternTex.g;
				tertiary *= patternTex.b;
				passthrough *= patternTex.a;
			#endif

			//roughness
			half calculatedRoughness = saturate(diffuse.a + (1.0 * _Roughness));
			#ifdef _SIMPLE_ROUGHNESS
				calculatedRoughness = _Roughness;
			#endif

			//others
			#ifdef _ILLUMINATION
				half4 illumination = illum * _IllumColor;
			#endif

			//finalColor
			half4 finalColor = primary + secondary + tertiary + passthrough;
			//detailTex *= calculatedRoughness;
			#ifdef _DETAIL_TEXTURE
				finalColor *= detailTex;
			#endif
			#ifdef _DECALS
				half4 decals = tex2D(_Decals, i.uv2_Decals.xy);
				half3 decalsNormal = UnpackNormal(tex2D(_DecalNormal, i.uv2_Decals.xy));
				finalColor = lerp(finalColor, half4(decals.rgb, finalColor.a), decals.a);
				finalNormal += normalize(decalsNormal);
			#endif

			//specular calcs
			half4 finalSpecularColor = stockDiffuse * occlusionTex;
			#ifdef _PATTERN
				finalSpecularColor *= lerp(white, patternTex.r, cc.r);
				finalSpecularColor *= lerp(white, patternTex.g, cc.g);
				finalSpecularColor *= lerp(white, patternTex.b, cc.b);
				finalSpecularColor *= lerp(white, patternTex.a, cc.r + cc.g + cc.b);
			#endif

			//reflection probe support
			half3 reflectDir = WorldReflectionVector(i, finalNormal);
			half3 boxProjectionDir = BoxProjectedCubemapDirection(reflectDir + 0.001, i.worldPos, unity_SpecCube0_ProbePosition, unity_SpecCube0_BoxMin, unity_SpecCube0_BoxMax);

			//reflectionData
			Unity_GlossyEnvironmentData envData; envData.roughness = calculatedRoughness; envData.reflUVW = boxProjectionDir;

			//probe blending
			half3 skyColor = Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE(unity_SpecCube0), unity_SpecCube0_HDR, envData);
			half3 skyColor2 = Unity_GlossyEnvironment(UNITY_PASS_TEXCUBE_SAMPLER(unity_SpecCube1, unity_SpecCube0), unity_SpecCube1_HDR, envData);
			skyColor = lerp(skyColor2, skyColor, unity_SpecCube0_BoxMin.w);

			//cubemapped reflection support
			#ifdef _CUBE_REFLECTION
				skyColor = UNITY_SAMPLE_TEXCUBE_LOD(_CubeReflection, reflectDir, envData.roughness * UNITY_SPECCUBE_LOD_STEPS).rgb;
			#endif
			skyColor *= occlusionTex;
			skyColor = max(half3(0,0,0), skyColor * 3);

			//metallic effect
			half displayMetalProperty = auroraTex.r * _trueMetallic;
			skyColor *= lerp(Desaturate(finalColor.xyz), finalColor.xyz, displayMetalProperty);
			skyColor *= 1 - _lightingBypass;

			//rave section
			#ifdef _RAVE
				half4 raveColor = black;
				half4 raveCC = tex2D(_RaveCC, i.uv_MainTex);
				half raveR = tex2D(_RaveMask, i.uv_MainTex + half2(_Time.x * _RaveRG.r, _Time.x * _RaveRG.g)).r;
				half raveG = tex2D(_RaveMask, i.uv_MainTex + half2(_Time.x * _RaveRG.b, _Time.x * _RaveRG.a)).g;
				half raveB = tex2D(_RaveMask, i.uv_MainTex + half2(_Time.x * _RaveBA.r, _Time.x * _RaveBA.g)).b;
				half raveA = tex2D(_RaveMask, i.uv_MainTex + half2(_Time.x * _RaveBA.b, _Time.x * _RaveBA.a)).a;

				#ifdef _VRCAUDIOLINK
					half4 audioLink = AudioLinkData(ALPASS_AUDIOLINK + int2(0, i.uv_MainTex.y));
					raveR = audioLink.r;

					raveG *= 0.75;
					raveG += audioLink.g * 0.25;
					raveG = saturate(raveG);
					
					raveB *= 0.75;
					raveB += audioLink.b * 0.25;
					raveB = saturate(raveB);

					raveA *= 0.75;
					raveA += audioLink.r * 0.25;
					raveA = saturate(raveA);
				#endif

				raveColor += half4(raveCC.r, raveCC.r, raveCC.r, 1) * _RaveColor * raveR;
				raveColor += half4(raveCC.g, raveCC.g, raveCC.g, 1) * _RaveSecondaryColor * raveG;
				raveColor += half4(raveCC.b, raveCC.b, raveCC.b, 1) * _RaveTertiaryColor * raveB;
				raveColor += half4(raveCC.a, raveCC.a, raveCC.a, 1) * _RaveQuaternaryColor * raveA;
			#endif

			//out
			o.Albedo = finalColor * (1.001 - (displayMetalProperty));
			o.Normal = finalNormal;
			o.Specular = finalSpecularColor;
			o.Gloss = calculatedRoughness;
			o.Emission = skyColor + (finalColor * _lightingBypass);
			#ifdef _ILLUMINATION
				o.Emission += illumination;
			#endif
			#ifdef _RAVE
				o.Emission += raveColor;
			#endif
			o.Alpha = displayMetalProperty;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "GentleShaders.Aurora.AR2.AuroraAR2Editor"
}