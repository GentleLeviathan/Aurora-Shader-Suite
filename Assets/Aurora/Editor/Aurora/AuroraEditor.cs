using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using GentleShaders.Aurora.Helpers;
using UnityEditor.PackageManager;
using System.Linq;
using GentleShaders.Aurora.Common;
using GentleShaders.Aurora.A3.Helpers;

/// <summary>
/// Aurora Shader Editor + UI.
/// WIP!
/// </summary>

namespace GentleShaders.Aurora
{
    [CanEditMultipleObjects]
    public class AuroraEditor : ShaderGUI
    {
        #region Properties and Fields

        //properties
        MaterialProperty mainTex;
        MaterialProperty cc;
        MaterialProperty auroraTex;
        MaterialProperty normal;
        MaterialProperty raveCC;

        MaterialProperty mainTex1;
        MaterialProperty cc1;
        MaterialProperty auroraTex1;
        MaterialProperty normal1;
        MaterialProperty raveCC1;

        MaterialProperty mainTex2;
        MaterialProperty cc2;
        MaterialProperty auroraTex2;
        MaterialProperty normal2;
        MaterialProperty raveCC2;

        MaterialProperty mainTex3;
        MaterialProperty cc3;
        MaterialProperty auroraTex3;
        MaterialProperty normal3;
        MaterialProperty raveCC3;

        MaterialProperty mainTex4;
        MaterialProperty cc4;
        MaterialProperty auroraTex4;
        MaterialProperty normal4;
        MaterialProperty raveCC4;

        MaterialProperty decals;
        MaterialProperty decalNormal;

        MaterialProperty primaryColor;
        MaterialProperty secondaryColor;
        MaterialProperty tertiaryColor;

        MaterialProperty metalness;
        MaterialProperty roughness;
        MaterialProperty deepness;
        MaterialProperty alphaCutoff;

        MaterialProperty colorTexture;
        MaterialProperty illuminationColor;

        MaterialProperty raveMask;
        MaterialProperty raveColor;
        MaterialProperty raveSecondary;
        MaterialProperty raveTertiary;
        MaterialProperty raveQuaternary;
        MaterialProperty raveRG;
        MaterialProperty raveBA;

        MaterialProperty chronoScroll0;
        MaterialProperty chronoScroll1;
        MaterialProperty chronoScroll2;
        MaterialProperty chronoScroll3;

        MaterialProperty alExclusive0;
        MaterialProperty alExclusive1;
        MaterialProperty alExclusive2;
        MaterialProperty alExclusive3;

        MaterialProperty themeColor0;
        MaterialProperty themeColor1;
        MaterialProperty themeColor2;
        MaterialProperty themeColor3;

        private Texture2D header;
        private bool texFound;
        private bool performedUpdateCheck;
        private string updateCheckResult;
        private string updateString;
        private bool gotProperties;
        private bool supportsAudioLink;

        private bool colorTexToggle;
        private bool alphaRoughnessToggle;

        private int dimensionCount = 0;
        private bool n0, n1, n2, n3, n4;

        private bool showAltTextures;
        private bool showRave;

        private bool useAudioLink;
        private bool audioLinkCheckComplete;
        private bool showHelpers;
        private bool updateReady;

        private bool useChronoScroll0;
        private bool useChronoScroll1;
        private bool useChronoScroll2;
        private bool useChronoScroll3;

        private bool audioLinkExclusive0;
        private bool audioLinkExclusive1;
        private bool audioLinkExclusive2;
        private bool audioLinkExclusive3;

        private bool useALThemeColor0;
        private bool useALThemeColor1;
        private bool useALThemeColor2;
        private bool useALThemeColor3;

        private Aurora_Transparency_Setting transparencySetting = Aurora_Transparency_Setting.Opaque;
        private Aurora_GI_Setting giSetting = Aurora_GI_Setting.Normal;
        private Aurora_Lighting_Setting lightingSetting = Aurora_Lighting_Setting.Lit;
        private Aurora_BLSH_Setting bakedLightSetting = Aurora_BLSH_Setting.AccountForBLSH;
        private Aurora_UVLayout_Setting uvLayoutSetting = Aurora_UVLayout_Setting.FiveX;

        int selectedTab;
        int shaderVariantId;

        private UnityEditor.PackageManager.Requests.ListRequest packageManagerListRequest;

        #endregion

        #region Logic

        private void InitTex()
        {
            header = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/GentleShaders/Aurora/Editor/Aurora/Assets/Aurora_Header.png", typeof(Texture2D)) ?? new Texture2D(1, 1);
            texFound = true;
        }

        private async Task PerformUpdateCheck()
        {
            performedUpdateCheck = true;
            updateCheckResult = await AuroraUpdateChecker.GetNewestVersionString();

            int masterVersion = int.Parse(new String(updateCheckResult.Where(Char.IsDigit).ToArray()));
            int currentVersionNumber = int.Parse(new String(AuroraCommon.currentVersion.Where(Char.IsDigit).ToArray()));

            if (masterVersion > currentVersionNumber)
            {
                updateString = "An update is available! (Current Version " + AuroraCommon.currentVersion + ")" + "\n(New Version " + updateCheckResult + ")";
                updateReady = true;
            }
            if(currentVersionNumber > masterVersion)
            {
                updateString = "You are using an unpublished experimental version.";
            }
        }

        private void ReadyForUpdate()
        {
            GUILayout.Label(updateString, EditorStyles.boldLabel);
            if (!updateReady) { return; }

            if (GUILayout.Button(MakeLabel("Open Repository", "Opens the Aurora Shader GitHub Repository")))
            {
                AuroraCommon.OpenRepositoryReleases();
            }
        }

        #region AudioLinkCheck

        private void CheckForAudioLink()
        {
            packageManagerListRequest = Client.List();
            EditorApplication.update += AudioLinkCheckUpdate;
        }

        private void AudioLinkCheckUpdate()
        {
            if (!packageManagerListRequest.IsCompleted) { return; }
            if (packageManagerListRequest.Status != StatusCode.Success)
            {
                Debug.LogError("AuroraA3Editor: Could not check package manager. " + packageManagerListRequest.Error);
                EditorApplication.update -= AudioLinkCheckUpdate;
                return;
            }

            UnityEditor.PackageManager.PackageInfo info = packageManagerListRequest.Result.FirstOrDefault(p => p.name == "com.llealloo.audiolink");
            if (info != null)
            {
                supportsAudioLink = true;
            }

            audioLinkCheckComplete = true;
            EditorApplication.update -= AudioLinkCheckUpdate;
        }

        #endregion

        public override void OnClosed(Material material)
        {
            EditorApplication.update -= AudioLinkCheckUpdate;
            base.OnClosed(material);
        }

        private void GetBooleanValues(Material mat)
        {
            if (decals.textureValue != null || decalNormal.textureValue != null)
            {
                showAltTextures = true;
            }

            if (raveCC.textureValue != null)
            {
                showRave = true;
            }

            transparencySetting = mat.GetFloat("_DstBlend") == 0 ? Aurora_Transparency_Setting.Opaque : Aurora_Transparency_Setting.Transparent;
            transparencySetting = mat.GetFloat("_cutoffDisable") == 1 ? transparencySetting : Aurora_Transparency_Setting.Cutout;
            lightingSetting = mat.GetFloat("_lightingBypass") == 0 ? Aurora_Lighting_Setting.Lit : Aurora_Lighting_Setting.Unlit;
            giSetting = mat.GetFloat("_giBoost") == 0 ? Aurora_GI_Setting.Normal : Aurora_GI_Setting.Boosted;
            bakedLightSetting = mat.GetFloat("_accountForBLSH") == 0 ? Aurora_BLSH_Setting.Ignore : Aurora_BLSH_Setting.AccountForBLSH;
            uvLayoutSetting = mat.GetFloat("_uvMethodSwitch") == 0 ? Aurora_UVLayout_Setting.FiveX : Aurora_UVLayout_Setting.ND5;

            useAudioLink = mat.IsKeywordEnabled("_VRCAUDIOLINK");

            useChronoScroll0 = chronoScroll0.floatValue > 0;
            useChronoScroll1 = chronoScroll1.floatValue > 0;
            useChronoScroll2 = chronoScroll2.floatValue > 0;
            useChronoScroll3 = chronoScroll3.floatValue > 0;

            audioLinkExclusive0 = alExclusive0.floatValue > 0;
            audioLinkExclusive1 = alExclusive1.floatValue > 0;
            audioLinkExclusive2 = alExclusive2.floatValue > 0;
            audioLinkExclusive3 = alExclusive3.floatValue > 0;

            useALThemeColor0 = themeColor0.floatValue > 0;
            useALThemeColor1 = themeColor1.floatValue > 0;
            useALThemeColor2 = themeColor2.floatValue > 0;
            useALThemeColor3 = themeColor3.floatValue > 0;

            if (dimensionCount == 0)
            {
                dimensionCount += mat.IsKeywordEnabled("_U1") ? 1 : 0;
                dimensionCount += mat.IsKeywordEnabled("_U2") ? 1 : 0;
                dimensionCount += mat.IsKeywordEnabled("_U3") ? 1 : 0;
                dimensionCount += mat.IsKeywordEnabled("_U4") ? 1 : 0;
            }
            //Show the default textures automatically if there is only one texture set
            if(dimensionCount == 0)
            {
                n0 = true;
            }
        }

#endregion

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (!texFound) { InitTex(); }

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            // Note: This is intentional!
            if (!performedUpdateCheck) { PerformUpdateCheck(); }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

            Material target = materialEditor.target as Material;

            if (!gotProperties)
            {
                //----------------------------------Properties-----------------------------------------
                try
                {
                    GetProperties(materialEditor, properties, target);
                    gotProperties = true;

                    CheckForAudioLink();
                }
                catch (NullReferenceException e)
                {
                    Debug.LogError("AuroraEditor: " + e.Message);
                    return;
                }
            }

            if (gotProperties)
            {
                GetBooleanValues(target);
            }

            //-----------------------------------DISPLAY-------------------------------------------
            DisplayCustomGUI(materialEditor, target);
        }

        private void GetProperties(MaterialEditor materialEditor, MaterialProperty[] properties, Material target)
        {
            //-----------Textures
            mainTex = ShaderGUI.FindProperty("_MainTex", properties);
            cc = ShaderGUI.FindProperty("_CC", properties);
            auroraTex = ShaderGUI.FindProperty("_Aurora", properties);
            normal = ShaderGUI.FindProperty("_BumpMap", properties);
            raveCC = ShaderGUI.FindProperty("_RaveCC", properties);

            mainTex1 = ShaderGUI.FindProperty("_MainTex1", properties);
            cc1 = ShaderGUI.FindProperty("_CC1", properties);
            auroraTex1 = ShaderGUI.FindProperty("_Aurora1", properties);
            normal1 = ShaderGUI.FindProperty("_BumpMap1", properties);
            raveCC1 = ShaderGUI.FindProperty("_RaveCC1", properties);

            mainTex2 = ShaderGUI.FindProperty("_MainTex2", properties);
            cc2 = ShaderGUI.FindProperty("_CC2", properties);
            auroraTex2 = ShaderGUI.FindProperty("_Aurora2", properties);
            normal2 = ShaderGUI.FindProperty("_BumpMap2", properties);
            raveCC2 = ShaderGUI.FindProperty("_RaveCC2", properties);

            mainTex3 = ShaderGUI.FindProperty("_MainTex3", properties);
            cc3 = ShaderGUI.FindProperty("_CC3", properties);
            auroraTex3 = ShaderGUI.FindProperty("_Aurora3", properties);
            normal3 = ShaderGUI.FindProperty("_BumpMap3", properties);
            raveCC3 = ShaderGUI.FindProperty("_RaveCC3", properties);

            mainTex4 = ShaderGUI.FindProperty("_MainTex4", properties);
            cc4 = ShaderGUI.FindProperty("_CC4", properties);
            auroraTex4 = ShaderGUI.FindProperty("_Aurora4", properties);
            normal4 = ShaderGUI.FindProperty("_BumpMap4", properties);
            raveCC4 = ShaderGUI.FindProperty("_RaveCC4", properties);

            decals = ShaderGUI.FindProperty("_Decals", properties);
            decalNormal = ShaderGUI.FindProperty("_DecalNormal", properties);

            //-----------Colors
            primaryColor = ShaderGUI.FindProperty("_Color", properties);
            secondaryColor = ShaderGUI.FindProperty("_SecondaryColor", properties);
            tertiaryColor = ShaderGUI.FindProperty("_TertiaryColor", properties);

            //-----------Values
            metalness = ShaderGUI.FindProperty("_trueMetallic", properties);
            roughness = ShaderGUI.FindProperty("_Roughness", properties);
            deepness = ShaderGUI.FindProperty("_Deepness", properties);
            alphaCutoff = ShaderGUI.FindProperty("_alphaCutoff", properties);

            //-----------Toggles
            colorTexture = ShaderGUI.FindProperty("_ColorTexture", properties);

            //-----------Illum
            illuminationColor = ShaderGUI.FindProperty("_IllumColor", properties);

            //-----------Rave Section
            raveMask = ShaderGUI.FindProperty("_RaveMask", properties);
            raveColor = ShaderGUI.FindProperty("_RaveColor", properties);
            raveSecondary = ShaderGUI.FindProperty("_RaveSecondaryColor", properties);
            raveTertiary = ShaderGUI.FindProperty("_RaveTertiaryColor", properties);
            raveQuaternary = ShaderGUI.FindProperty("_RaveQuaternaryColor", properties);
            raveRG = ShaderGUI.FindProperty("_RaveRG", properties);
            raveBA = ShaderGUI.FindProperty("_RaveBA", properties);

            chronoScroll0 = ShaderGUI.FindProperty("_chronotensityScroll0", properties);
            chronoScroll1 = ShaderGUI.FindProperty("_chronotensityScroll1", properties);
            chronoScroll2 = ShaderGUI.FindProperty("_chronotensityScroll2", properties);
            chronoScroll3 = ShaderGUI.FindProperty("_chronotensityScroll3", properties);

            themeColor0 = ShaderGUI.FindProperty("_useALThemeColor0", properties);
            themeColor1 = ShaderGUI.FindProperty("_useALThemeColor1", properties);
            themeColor2 = ShaderGUI.FindProperty("_useALThemeColor2", properties);
            themeColor3 = ShaderGUI.FindProperty("_useALThemeColor3", properties);

            alExclusive0 = ShaderGUI.FindProperty("_audioLinkExclusive0", properties);
            alExclusive1 = ShaderGUI.FindProperty("_audioLinkExclusive1", properties);
            alExclusive2 = ShaderGUI.FindProperty("_audioLinkExclusive2", properties);
            alExclusive3 = ShaderGUI.FindProperty("_audioLinkExclusive3", properties);
        }

        private void DisplayCustomGUI(MaterialEditor materialEditor, Material target)
        {
            ReadyForUpdate();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(header, GUILayout.MinWidth(240), GUILayout.MaxWidth(445), GUILayout.MinHeight(50), GUILayout.MaxHeight(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            #region ToolbarTabs

            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Material", "Shader", "Statistics" });

            switch (selectedTab)
            {
                case 0:
                    DisplayMaterialGUI(materialEditor, target);
                    break;
                case 1:
                    DisplayShaderSettingsGUI(materialEditor, target);
                    break;
                case 2:
                    DisplayStatisticsGUI(materialEditor, target);
                    break;
            }

            #endregion
        }

        private void DisplayShaderSettingsGUI(MaterialEditor materialEditor, Material target)
        {
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            transparencySetting = (Aurora_Transparency_Setting)EditorGUILayout.EnumPopup("Transparency Setting", transparencySetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            giSetting = (Aurora_GI_Setting)EditorGUILayout.EnumPopup("Global Illumination Setting *", giSetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            lightingSetting = (Aurora_Lighting_Setting)EditorGUILayout.EnumPopup("Lighting Setting *", lightingSetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            bakedLightSetting = (Aurora_BLSH_Setting)EditorGUILayout.EnumPopup("Baked Light Setting *", bakedLightSetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            uvLayoutSetting = (Aurora_UVLayout_Setting)EditorGUILayout.EnumPopup("UV Layout Setting", uvLayoutSetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(30f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            GUILayout.TextArea("Settings marked with an asterisk (*) are settings that may change the lighting model such that it is no longer energy-conserving. " +
                "However, some settings may appear more 'realistic' than an otherwise energy-conserving model." +
                "\n\nFor example, the default 'Baked Lighting Setting' is set to account for baked lights not providing a light direction for specularity. This often results in a more grounded appearance despite violating energy conservation.");
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            ApplyShaderFeatures(target);
            CalculateShaderVariantID(target);
            ApplyToggles(target);
        }

        private void DisplayStatisticsGUI(MaterialEditor materialEditor, Material target)
        {
            ApplyShaderFeatures(target);
            CalculateShaderVariantID(target);
            ApplyToggles(target);

            int textureCount = 0;
            long textureByteCount = 0;
            if (mainTex.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)mainTex.textureValue); }
            if (mainTex1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)mainTex1.textureValue); }
            if (mainTex2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)mainTex2.textureValue); }
            if (mainTex3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)mainTex3.textureValue); }
            if (mainTex4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)mainTex4.textureValue); }

            if (auroraTex.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)auroraTex.textureValue); }
            if (auroraTex1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)auroraTex1.textureValue); }
            if (auroraTex2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)auroraTex2.textureValue); }
            if (auroraTex3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)auroraTex3.textureValue); }
            if (auroraTex4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)auroraTex4.textureValue); }

            if (cc.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)cc.textureValue); }
            if (cc1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)cc1.textureValue); }
            if (cc2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)cc2.textureValue); }
            if (cc3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)cc3.textureValue); }
            if (cc4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)cc4.textureValue); }

            if (normal.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)normal.textureValue); }
            if (normal1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)normal1.textureValue); }
            if (normal2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)normal2.textureValue); }
            if (normal3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)normal3.textureValue); }
            if (normal4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)normal4.textureValue); }

            if (raveCC.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)raveCC.textureValue); }
            if (raveCC1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)raveCC1.textureValue); }
            if (raveCC2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)raveCC2.textureValue); }
            if (raveCC3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)raveCC3.textureValue); }
            if (raveCC4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)raveCC4.textureValue); }

            GUILayout.Space(40f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(MakeLabel("Material Statistics", "Provides detailed information about the material."), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            EditorGUILayout.IntField("Texture Count", textureCount);
            GUILayout.Space(10f);
            EditorGUILayout.TextField("Uncompressed Texture Size (VRAM)", AuroraCommon.GetUncompressedTexture2DSizeString(textureByteCount));
            GUILayout.Space(10f);
            EditorGUILayout.IntField("Shader Variant ID", shaderVariantId);
            GUILayout.Space(10f);
            EditorGUILayout.TextField("Material Keywords", GetShaderKeywordString(target));
            GUILayout.Space(10f);
        }

        private string GetShaderKeywordString(Material mat)
        {
            string keywords = "";
            for (int i = 0; i < mat.shaderKeywords.Length; i++)
            {
                keywords += mat.shaderKeywords[i];
                if (i != mat.shaderKeywords.Length - 1)
                {
                    keywords += ", ";
                }
            }
            return keywords;
        }

        private void CalculateShaderVariantID(Material mat)
        {
            int variantId = 0;
            if(mat.shaderKeywords.Length > 0)
            {
                variantId = 2;
            }
            for (int i = 1; i < mat.shaderKeywords.Length; i++)
            {
                variantId *= 2;
            }
            shaderVariantId = variantId;
        }

        private void DisplayMaterialGUI(MaterialEditor materialEditor, Material target)
        {
            #region Textures and Colors

            //Textures
            GUILayout.Space(4f);

            #region TextureDrawers

            //Only show the toggle button if there is more than one set of textures
            if (dimensionCount > 0)
            {
                if (GUILayout.Button(MakeLabel("Base Textures", "Displays the section for the base 0-1 UV space textures.")))
                {
                    n0 = !n0;
                }
            }
            if (n0)
            {
                materialEditor.TextureProperty(mainTex, "Diffuse", false);
                //materialEditor.TextureScaleOffsetProperty(mainTex);
                //desaturation toggle
                colorTexToggle = GUILayout.Toggle(colorTexture.floatValue > 0 ? true : false, MakeLabel(" Desaturate?", "This checkbox desaturates the Diffuse texture, eliminating existing colors to improve color control accuracy."));

                materialEditor.TextureProperty(cc, "Color Control (CC)", false);
                materialEditor.TextureProperty(normal, "Normal Map (DirectX)", false);
                materialEditor.TextureCompatibilityWarning(normal);

                materialEditor.TextureProperty(auroraTex, "Aurora Texture", false);
                materialEditor.TextureProperty(raveCC, "Rave CC (RGBA)", false);

                GUILayout.Space(4f);
            }

            if (dimensionCount > 0)
            {
                if (GUILayout.Button(MakeLabel("Texture Set 1", "Displays the section for the additional 1-2 UV space textures.")))
                {
                    n1 = !n1;
                }
                if (n1)
                {
                    materialEditor.TextureProperty(mainTex1, "Diffuse", false);
                    materialEditor.TextureProperty(cc1, "Color Control (CC)", false);
                    materialEditor.TextureProperty(normal1, "Normal Map (DirectX)", false);
                    materialEditor.TextureCompatibilityWarning(normal1);
                    //materialEditor.TextureScaleOffsetProperty(mainTex1);

                    materialEditor.TextureProperty(auroraTex1, "Aurora Texture", false);
                    materialEditor.TextureProperty(raveCC1, "Rave CC (RGBA)", false);
                    GUILayout.Space(4f);
                }
            }
            if (dimensionCount > 1)
            {
                if (GUILayout.Button(MakeLabel("Texture Set 2", "Displays the section for the additional 2-3 UV space textures.")))
                {
                    n2 = !n2;
                }
                if (n2)
                {
                    materialEditor.TextureProperty(mainTex2, "Diffuse", false);
                    materialEditor.TextureProperty(cc2, "Color Control (CC)", false);
                    materialEditor.TextureProperty(normal2, "Normal Map (DirectX)", false);
                    materialEditor.TextureCompatibilityWarning(normal2);
                    //materialEditor.TextureScaleOffsetProperty(mainTex1);

                    materialEditor.TextureProperty(auroraTex2, "Aurora Texture", false);
                    materialEditor.TextureProperty(raveCC2, "Rave CC (RGBA)", false);
                    GUILayout.Space(4f);
                }
            }
            if (dimensionCount > 2)
            {
                if (GUILayout.Button(MakeLabel("Texture Set 3", "Displays the section for the additional 3-4 UV space textures.")))
                {
                    n3 = !n3;
                }
                if (n3)
                {
                    materialEditor.TextureProperty(mainTex3, "Diffuse", false);
                    materialEditor.TextureProperty(cc3, "Color Control (CC)", false);
                    materialEditor.TextureProperty(normal3, "Normal Map (DirectX)", false);
                    materialEditor.TextureCompatibilityWarning(normal3);
                    //materialEditor.TextureScaleOffsetProperty(mainTex1);

                    materialEditor.TextureProperty(auroraTex3, "Aurora Texture", false);
                    materialEditor.TextureProperty(raveCC3, "Rave CC (RGBA)", false);
                    GUILayout.Space(4f);
                }
            }
            if (dimensionCount > 3)
            {
                if (GUILayout.Button(MakeLabel("Texture Set 4", "Displays the section for the additional 4-5 UV space textures.")))
                {
                    n4 = !n4;
                }
                if (n4)
                {
                    materialEditor.TextureProperty(mainTex4, "Diffuse", false);
                    materialEditor.TextureProperty(cc4, "Color Control (CC)", false);
                    materialEditor.TextureProperty(normal4, "Normal Map (DirectX)", false);
                    materialEditor.TextureCompatibilityWarning(normal4);
                    //materialEditor.TextureScaleOffsetProperty(mainTex1);

                    materialEditor.TextureProperty(auroraTex4, "Aurora Texture", false);
                    materialEditor.TextureProperty(raveCC4, "Rave CC (RGBA)", false);
                    GUILayout.Space(4f);
                }
            }

            GUILayout.Space(10f);
            if (dimensionCount >= 4)
            {
                if (GUILayout.Button(MakeLabel("Maximum Sets Reached", "Only 5 unique texture sets are supported at this time.")))
                {

                }
            }
            else
            {
                if (GUILayout.Button(MakeLabel("Add Texture Set...", "Adds a horizontal UV texture set to the material.")))
                {
                    dimensionCount++;
                }
            }

            #endregion

            //Colors
            GUILayout.Space(8f);
            GUILayout.Label(MakeLabel("Main Colors", "These are the colors assigned to RGB zones of the CC."), EditorStyles.boldLabel);
            materialEditor.ColorProperty(primaryColor, "Primary Color (R)");
            materialEditor.ColorProperty(secondaryColor, "Secondary Color (G)");
            materialEditor.ColorProperty(tertiaryColor, "Tertiary Color (B)");

            GUILayout.Space(6f);
            GUILayout.Label("Illumination", EditorStyles.boldLabel);
            materialEditor.ColorProperty(illuminationColor, "Color (HDR)");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(MakeLabel("Guess Illumination Color", "Attempts to create a neutral and appealing illumination color based on the chosen primary color."), GUILayout.Width(230f)))
            {
                if (ValueTestApproximation(primaryColor.colorValue.r, primaryColor.colorValue.g, 0.025f) && ValueTestApproximation(primaryColor.colorValue.g, primaryColor.colorValue.b, 0.025f))
                {
                    illuminationColor.colorValue = new Color(0.00392156863f * 82f, 0.00392156863f * 137f, 0.00392156863f * 215f) * 2f;
                }
                else
                {
                    illuminationColor.colorValue = (primaryColor.colorValue * 1.25f) + (Color.white * 0.5f);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);

            //Values
            GUILayout.Space(8f);
            GUILayout.Label("Properties", EditorStyles.boldLabel);
            materialEditor.RangeProperty(metalness, "Metalness");
            materialEditor.RangeProperty(roughness, "Roughness");
            materialEditor.RangeProperty(deepness, "Color Depth");
            if(transparencySetting == Aurora_Transparency_Setting.Cutout)
            {
                materialEditor.RangeProperty(alphaCutoff, "Alpha Cutoff");
            }

            GUILayout.Space(8f);

            #endregion

            #region Drawers

            //Custom Textures
            if (GUILayout.Button(MakeLabel("Show UV2 Section", "Show decal textures applied via UV2. This will always be visible if there are filled properties.")))
            {
                showAltTextures = !showAltTextures;
            }
            if (showAltTextures)
            {
                GUILayout.Space(10f);
                GUILayout.Label("Decal Textures (UV2)", EditorStyles.boldLabel);
                materialEditor.TextureProperty(decals, "Decals (RGB+A)", false);
                materialEditor.TextureProperty(decalNormal, "Decal Normal (RGB+A)", false);
            }

            //Rave Section
            if (GUILayout.Button(MakeLabel("Show Rave Section", "Displays the 'rave' section for additional emission and AudioLink. This will always be visible if there are filled properties.")))
            {
                showRave = !showRave;
            }
            if (showRave)
            {
                GUILayout.Space(4f);
                materialEditor.TextureProperty(raveMask, "Rave Mask");
                GUILayout.Space(10f);

                GUILayout.BeginHorizontal();
                GUILayout.Label("R-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    audioLinkExclusive0 = GUILayout.Toggle(audioLinkExclusive0, MakeLabel("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink."));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    useALThemeColor0 = GUILayout.Toggle(useALThemeColor0, MakeLabel("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world."));
                }
                if (!useALThemeColor0)
                {
                    GUILayout.FlexibleSpace();
                    materialEditor.ColorProperty(raveColor, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("G-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    audioLinkExclusive1 = GUILayout.Toggle(audioLinkExclusive1, MakeLabel("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink."));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    useALThemeColor1 = GUILayout.Toggle(useALThemeColor1, MakeLabel("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world."));
                }
                if (!useALThemeColor1)
                {
                    GUILayout.FlexibleSpace();
                    materialEditor.ColorProperty(raveSecondary, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("B-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    audioLinkExclusive2 = GUILayout.Toggle(audioLinkExclusive2, MakeLabel("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink."));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    useALThemeColor2 = GUILayout.Toggle(useALThemeColor2, MakeLabel("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world."));
                }
                if (!useALThemeColor2)
                {
                    GUILayout.FlexibleSpace();
                    materialEditor.ColorProperty(raveTertiary, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("A-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    audioLinkExclusive3 = GUILayout.Toggle(audioLinkExclusive3, MakeLabel("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink."));
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    useALThemeColor3 = GUILayout.Toggle(useALThemeColor3, MakeLabel("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world."));
                }
                if (!useALThemeColor3)
                {
                    GUILayout.FlexibleSpace();
                    materialEditor.ColorProperty(raveQuaternary, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.Space(10f);

                Vector2 red = new Vector2(raveRG.vectorValue.x, raveRG.vectorValue.y);
                Vector2 green = new Vector2(raveRG.vectorValue.z, raveRG.vectorValue.w);
                Vector2 blue = new Vector2(raveBA.vectorValue.x, raveBA.vectorValue.y);
                Vector2 alpha = new Vector2(raveBA.vectorValue.z, raveBA.vectorValue.w);

                if (useAudioLink)
                {
                    GUILayout.BeginHorizontal();
                    useChronoScroll0 = GUILayout.Toggle(useChronoScroll0, MakeLabel("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity."));
                    GUILayout.FlexibleSpace();
                    useChronoScroll1 = GUILayout.Toggle(useChronoScroll1, MakeLabel("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity."));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Red");
                red.x = EditorGUILayout.FloatField(red.x);
                red.y = EditorGUILayout.FloatField(red.y);
                GUILayout.Label("Green");
                green.x = EditorGUILayout.FloatField(green.x);
                green.y = EditorGUILayout.FloatField(green.y);
                GUILayout.EndHorizontal();

                if (useAudioLink)
                {
                    GUILayout.BeginHorizontal();
                    useChronoScroll2 = GUILayout.Toggle(useChronoScroll2, MakeLabel("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity."));
                    GUILayout.FlexibleSpace();
                    useChronoScroll3 = GUILayout.Toggle(useChronoScroll3, MakeLabel("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity."));
                    GUILayout.EndHorizontal();
                }

                GUILayout.BeginHorizontal();
                GUILayout.Label("Blue");
                blue.x = EditorGUILayout.FloatField(blue.x);
                blue.y = EditorGUILayout.FloatField(blue.y);
                GUILayout.Label("Alpha");
                alpha.x = EditorGUILayout.FloatField(alpha.x);
                alpha.y = EditorGUILayout.FloatField(alpha.y);
                GUILayout.EndHorizontal();

                raveRG.vectorValue = new Vector4(red.x, red.y, green.x, green.y);
                raveBA.vectorValue = new Vector4(blue.x, blue.y, alpha.x, alpha.y);

                if (audioLinkCheckComplete)
                {
                    if (supportsAudioLink)
                    {
                        GUILayout.Space(10f);
                        useAudioLink = GUILayout.Toggle(useAudioLink, new GUIContent("Use VRC AudioLink?", "Check if you would like the rave effect to be modified by llealloo's 'VRC Udon Audio Link'"));
                    }
                    else
                    {
                        if (useAudioLink)
                        {
                            GUILayout.Space(10f);
                            GUILayout.Label("Material is set to use AudioLink but AudioLink is not installed!");
                            GUILayout.Label("Either download it or disable AudioLink.");
                            if (GUILayout.Button(MakeLabel("Download AudioLink")))
                            {
                                Application.OpenURL("https://github.com/llealloo/vrc-udon-audio-link/releases");
                            }
                            if (GUILayout.Button(MakeLabel("Disable AudioLink")))
                            {
                                useAudioLink = false;
                                ((Material)materialEditor.target).DisableKeyword("_VRCAUDIOLINK");
                            }
                            GUILayout.Space(10f);
                        }
                    }
                }
                else
                {
                    GUILayout.Space(10f);
                    GUILayout.Label("Checking for AudioLink support...");
                }
            }

            #region Helper Drawers

            //Helpers
            if (GUILayout.Button(MakeLabel("Show Helpers", "Displays additional tools and helper functions included with the Aurora shader.")))
            {
                showHelpers = !showHelpers;
            }
            if (showHelpers)
            {
                GUILayout.Space(4f);
                GUILayout.Label("Helpers", EditorStyles.boldLabel);
                GUILayout.Space(4f);

                if (GUILayout.Button(new GUIContent("Pack Aurora Textures", "Packs existing metallic, roughness, illumination, and occlusion textures into the 'Aurora A4' format."), GUILayout.Width(250f)))
                {
                    if (mainTex.textureValue)
                    {
                        AuroraA3Packer.Init((Texture2D)mainTex.textureValue);
                    }
                    else
                    {
                        AuroraA3Packer.Init();
                    }
                }
                GUILayout.Space(6f);
            }

            #endregion

            if (GUILayout.Button(new GUIContent("About...", "Displays the About pop-up for the shader, including a short FAQ and links to source.")))
            {
                //display about Window
                AuroraAboutWindow.Init();
            }

            CopyScaleOffsetMain();

            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.PropertiesChanged();

                gotProperties = false;
            }

            ///AURORA_TODO - Create new AuroraAboutWindow for new A3 format, changes and why they were made, and how existing AR2 materials will be upgraded going forward
            /*if (GUILayout.Button(new GUIContent("About...", "Displays the About pop-up for the shader, including a short FAQ and links to source.")))
            {
                //display about Window
                AuroraAboutWindow.Init();
            }*/

            #endregion

            ApplyShaderFeatures(target);
            ApplyToggles(target);
        }

        #region Utilities

        private void CopyScaleOffsetMain()
        {
            cc.textureScaleAndOffset = mainTex.textureScaleAndOffset;
            normal.textureScaleAndOffset = mainTex.textureScaleAndOffset;
        }

        private void ApplyShaderFeatures(Material mat)
        {
            if (mainTex1.textureValue || normal1.textureValue || cc1.textureValue || auroraTex1.textureValue) { mat.EnableKeyword("_U1"); } else { mat.DisableKeyword("_U1"); }
            if (mainTex2.textureValue || normal2.textureValue || cc2.textureValue || auroraTex2.textureValue) { mat.EnableKeyword("_U2"); } else { mat.DisableKeyword("_U2"); }
            if (mainTex3.textureValue || normal3.textureValue || cc3.textureValue || auroraTex3.textureValue) { mat.EnableKeyword("_U3"); } else { mat.DisableKeyword("_U3"); }
            if (mainTex4.textureValue || normal4.textureValue || cc4.textureValue || auroraTex4.textureValue) { mat.EnableKeyword("_U4"); } else { mat.DisableKeyword("_U4"); }

            if (decals.textureValue || decalNormal.textureValue) { mat.EnableKeyword("_DECALS"); } else { mat.DisableKeyword("_DECALS"); }

            if (raveCC.textureValue || raveCC1.textureValue || raveCC2.textureValue || raveCC3.textureValue || raveCC4.textureValue) { mat.EnableKeyword("_RAVE"); } else { mat.DisableKeyword("_RAVE"); }

            //Only if the project supports audio link which is determined by checking the PackageManager's list of dependencies
            if (!supportsAudioLink) { return; }
            if (useAudioLink) { mat.EnableKeyword("_VRCAUDIOLINK"); } else { mat.DisableKeyword("_VRCAUDIOLINK"); }
        }

        private void ApplyToggles(Material mat)
        {
            mat.SetFloat("_DstBlend", transparencySetting == Aurora_Transparency_Setting.Transparent ? 10 : 0);
            mat.SetFloat("_cutoffDisable", transparencySetting == Aurora_Transparency_Setting.Cutout ? 0 : 1);
            mat.SetFloat("_alphaCutoff", transparencySetting == Aurora_Transparency_Setting.Cutout ? mat.GetFloat("_alphaCutoff") : 0);
            mat.renderQueue = transparencySetting == Aurora_Transparency_Setting.Opaque ? (int)UnityEngine.Rendering.RenderQueue.Geometry : (int)UnityEngine.Rendering.RenderQueue.Transparent;

            mat.SetFloat("_AlphaRoughness", alphaRoughnessToggle ? 1 : 0);
            mat.SetFloat("_ColorTexture", colorTexToggle ? 1 : 0);

            mat.SetInt("_uvMethodSwitch", uvLayoutSetting == Aurora_UVLayout_Setting.FiveX ? 0 : 1);
            mat.SetInt("_giBoost", giSetting == Aurora_GI_Setting.Normal ? 0 : 1);
            mat.SetInt("_lightingBypass", lightingSetting == Aurora_Lighting_Setting.Lit ? 0 : 1);
            mat.SetInt("_accountForBLSH", bakedLightSetting == Aurora_BLSH_Setting.AccountForBLSH ? 1 : 0);

            mat.SetInt("_chronotensityScroll0", useChronoScroll0 ? 1 : 0);
            mat.SetInt("_chronotensityScroll1", useChronoScroll1 ? 1 : 0);
            mat.SetInt("_chronotensityScroll2", useChronoScroll2 ? 1 : 0);
            mat.SetInt("_chronotensityScroll3", useChronoScroll3 ? 1 : 0);

            mat.SetInt("_useALThemeColor0", useALThemeColor0 ? 1 : 0);
            mat.SetInt("_useALThemeColor1", useALThemeColor1 ? 1 : 0);
            mat.SetInt("_useALThemeColor2", useALThemeColor2 ? 1 : 0);
            mat.SetInt("_useALThemeColor3", useALThemeColor3 ? 1 : 0);

            mat.SetInt("_audioLinkExclusive0", audioLinkExclusive0 ? 1 : 0);
            mat.SetInt("_audioLinkExclusive1", audioLinkExclusive1 ? 1 : 0);
            mat.SetInt("_audioLinkExclusive2", audioLinkExclusive2 ? 1 : 0);
            mat.SetInt("_audioLinkExclusive3", audioLinkExclusive3 ? 1 : 0);
        }

        private GUIContent MakeLabel(string displayName, string tooltip = null)
        {
            GUIContent staticLabel = new GUIContent();
            staticLabel.text = displayName;
            staticLabel.tooltip = tooltip;
            return staticLabel;
        }

        /// <summary>
        /// Tests if two float values are within a given tolerance of each other.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="difference"></param>
        /// <returns></returns>
        private bool ValueTestApproximation(float a, float b, float difference)
        {
            return (Mathf.Abs(a - b) < difference);
        }

        #endregion
    }
}