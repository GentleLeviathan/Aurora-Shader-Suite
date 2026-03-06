/*

MIT License

Copyright (c) 2026 GentleLeviathan

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
		_occlusionStrength("Ambient Occlusion Strength", Range(0, 4)) = 1.0

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
		_giBoost("ambientBoost", Range(0, 4)) = 0
		_giBoostEnabled("ambientBoostToggle", Range(0, 1)) = 0

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

		//'Inverse Gloss' ??
		_ViewSpecularEnabled("inverseGlossToggle", Range(0, 1)) = 0
		_ViewSpecular("Inverse Gloss", Range(0, 1)) = 0
		_ViewSpecularGain("Inverse Gloss Gain", Range(0, 15)) = 2.0
		_ViewSpecularSpecSaturation("Inverse Gloss Specular Saturation", Range(0, 1)) = 0.5
		_ViewSpecularSpecValue("Inverse Gloss Specular Value", Range(0, 1)) = 0.5
		_ViewSpecularRoughnessTerm("Inverse Gloss Roughness Term", Range(0, 1)) = 1.0
		_ViewSpecularColorMixing("Inverse Gloss Color Mix", Range(0, 1)) = 0.0
		[HDR]_ViewSpecularColor("Inverse Gloss Color", Color) = (1,1,1,1)

		//Radiance Scaling
		_RSStrength("Radiance Scaling", Range(0, 1)) = 0
		_RSGain("Radiance Scaling Gain", Range(0, 4)) = 1

		//ACEL Properties
		_RimLightingPower("Rim Lighting Power", Range(0.01, 1)) = 0.1
		_RimLightingStrength("Rim Lighting Strength", Range(0, 1)) = 0.5
		_RimLightingDiffuseInfluence("Rim Lighting Diffuse Influence", Range(0, 1)) = 0.0
		_RimLightingColorInfluence("Rim Lighting Color Influence", Range(0, 1)) = 0.0
		[HDR]_RimLightingColor("Rim Lighting Color", Color) = (1,1,1,1)

		_ACELDiffuseStrength("ACEL Diffuse Strength", Range(0, 10)) = 1.0
		_ACELSpecularStrength("ACEL Specular Strength", Range(0, 10)) = 1.0

		_ACELAmbientDiffuseStrength("ACEL Ambient Diffuse Strength", Range(0, 10)) = 1.0
		_ACELAmbientSpecularStrength("ACEL Ambient Specular Strength", Range(0, 10)) = 1.0

		_ACELOutlineWidth("ACEL Outline Width", Range(0, 10)) = 0.1
		_ACELOutlineStrength("ACEL Outline Strength", Range(0, 1)) = 0.5
		_ACELOutlineThreshold("ACEL Outline Threshold", Range(0, 1)) = 0.1

		//UI
		[HideInInspector] _TextureSetName_0 ("Texture Set 0 Name", Vector) = (66658369, 0, 0, 0)
		[HideInInspector] _TextureSetName_1 ("Texture Set 1 Name", Vector) = (83698449, 0, 0, 0)
		[HideInInspector] _TextureSetName_2 ("Texture Set 2 Name", Vector) = (83698450, 0, 0, 0)
		[HideInInspector] _TextureSetName_3 ("Texture Set 3 Name", Vector) = (83698451, 0, 0, 0)
		[HideInInspector] _TextureSetName_4 ("Texture Set 4 Name", Vector) = (83698452, 0, 0, 0)

		//Blending
		[HideInInspector] _SrcBlend ("_srcBlend", Float) = 1.0
		[HideInInspector] _DstBlend ("_dstBlend", Float) = 10.0
		[HideInInspector] _ZWrite ("_zWrite", Float) = 1.0
		[HideInInspector] _ZTest ("_zTest", Float) = 4.0
		[HideInInspector] _Cull ("_Cull", Int) = 2
	}

    SubShader
    {
		Tags { "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull [_Cull]
		ZWrite [_ZWrite]
		ZTest [_ZTest]

		CGPROGRAM

		#include "../CgInc/AuroraCommon.cginc"
		#include "../CgInc/AuroraFiveX.cginc"
		#include "../CgInc/RadianceScaling.cginc"
		#include "../CgInc/AuroraBRDFs.cginc"

		#pragma target 4.0
		#pragma only_renderers d3d11 glcore gles
		#pragma surface surface Aurora addshadow fullforwardshadows

		//local features
		#pragma shader_feature_local _DECALS				//2
		#pragma shader_feature_local _RAVE					//4
		#pragma shader_feature_local _VRCAUDIOLINK			//8

		#pragma shader_feature_local _U1					//16
        #pragma shader_feature_local _U2					//32
        #pragma shader_feature_local _U3					//64
        #pragma shader_feature_local _U4					//128

		#pragma shader_feature_local _BRDF5 _BRDF4 _ACEL	//768

		#ifdef _VRCAUDIOLINK
			#include "../CgInc/AudioLink/AuroraAL.cginc"
		#endif

		#include "../CgInc/AuroraLighting.cginc"
		#include "../CgInc/AuroraInput.cginc"
        
		#include "../CgInc/AuroraSurface.cginc"

		ENDCG

		Pass
		{
			Name "ACEL_Outline"
			Cull Front
			CGPROGRAM

			#include "../CgInc/AuroraACEL_Outline.cginc"

			ENDCG
		}
	}
	
	Fallback "Diffuse"
	CustomEditor "GentleShaders.Aurora.Five.AuroraEditor"
}