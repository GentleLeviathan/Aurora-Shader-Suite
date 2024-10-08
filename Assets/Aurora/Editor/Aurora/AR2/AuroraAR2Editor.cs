﻿using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using GentleShaders.Aurora.Helpers;
using GentleShaders.Aurora.AR2.Helpers;
using UnityEditor.PackageManager;
using System.Linq;
using GentleShaders.Aurora.Common;

/// <summary>
/// Aurora Shader Editor + UI. Contains a large number of options and features, and holds the interface for the included texture helpers.
/// Access it by creating a material with the Aurora shader applied.
/// WIP!
/// </summary>

namespace GentleShaders.Aurora.AR2
{
    [CanEditMultipleObjects]
    public class AuroraAR2Editor : ShaderGUI
    {
        #region Properties and Fields

        //properties
        MaterialProperty mainTex;
        MaterialProperty decals;
        MaterialProperty decalNormal;
        MaterialProperty cc;
        MaterialProperty auroraTex;
        MaterialProperty pattern;
        MaterialProperty normal;

        MaterialProperty primaryColor;
        MaterialProperty secondaryColor;
        MaterialProperty tertiaryColor;

        MaterialProperty metalness;
        MaterialProperty roughness;
        MaterialProperty deepness;

        MaterialProperty colorTexture;
        MaterialProperty illuminationColor;

        MaterialProperty reflectCube;

        MaterialProperty detailTex;
        MaterialProperty detailNormal;
        MaterialProperty detailStrength;

        MaterialProperty raveCC;
        MaterialProperty raveMask;
        MaterialProperty raveColor;
        MaterialProperty raveSecondary;
        MaterialProperty raveTertiary;
        MaterialProperty raveQuaternary;
        MaterialProperty raveRG;
        MaterialProperty raveBA;

        private Texture2D header;
        private bool texFound;
        private bool performedUpdateCheck;
        private string updateCheckResult;
        private string updateString;
        private bool gotProperties;
        private bool supportsAudioLink;

        private bool colorTexToggle;
        private bool simpleRoughnessToggle;
        private bool alphaRoughnessToggle;
        private bool bypassLighting;
        private bool showAurora;
        private bool showAltTextures;
        private bool showRave;
        private bool useAudioLink;
        private bool audioLinkCheckComplete;
        private bool showHelpers;
        private bool updateReady;

        private UnityEditor.PackageManager.Requests.ListRequest packageManagerListRequest;

        #endregion

        #region Logic

        private void InitTex()
        {
            header = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/GentleShaders/Aurora/Editor/Aurora/Assets/AR2/Aurora_AR2_Header.png", typeof(Texture2D)) ?? new Texture2D(1, 1);
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
            if (currentVersionNumber > masterVersion)
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
                Debug.LogError("AuroraEditor: Could not check package manager. " + packageManagerListRequest.Error);
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
            if (reflectCube.textureValue != null || detailTex.textureValue != null || detailNormal.textureValue != null)
            {
                showAltTextures = true;
            }

            if(raveCC.textureValue != null)
            {
                showRave = true;
            }

            simpleRoughnessToggle = mat.IsKeywordEnabled("_SIMPLE_ROUGHNESS");
            useAudioLink = mat.IsKeywordEnabled("_VRCAUDIOLINK");
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
            decals = ShaderGUI.FindProperty("_Decals", properties);
            decalNormal = ShaderGUI.FindProperty("_DecalNormal", properties);
            cc = ShaderGUI.FindProperty("_CC", properties);
            pattern = ShaderGUI.FindProperty("_Pattern", properties);
            auroraTex = ShaderGUI.FindProperty("_Aurora", properties);
            normal = ShaderGUI.FindProperty("_BumpMap", properties);

            //-----------Colors
            primaryColor = ShaderGUI.FindProperty("_Color", properties);
            secondaryColor = ShaderGUI.FindProperty("_SecondaryColor", properties);
            tertiaryColor = ShaderGUI.FindProperty("_TertiaryColor", properties);

            //-----------Values
            roughness = ShaderGUI.FindProperty("_Roughness", properties);
            deepness = ShaderGUI.FindProperty("_Deepness", properties);

            //-----------Toggles
            metalness = ShaderGUI.FindProperty("_trueMetallic", properties);
            colorTexture = ShaderGUI.FindProperty("_ColorTexture", properties);

            //-----------Illum
            illuminationColor = ShaderGUI.FindProperty("_IllumColor", properties);

            //-----------Custom Textures
            reflectCube = ShaderGUI.FindProperty("_CubeReflection", properties);

            //-----------Detail Textures
            detailTex = ShaderGUI.FindProperty("_DetailMap", properties);
            detailNormal = ShaderGUI.FindProperty("_DetailNormal", properties);
            detailStrength = ShaderGUI.FindProperty("_DetailStrength", properties);

            //-----------Rave Section
            raveCC = ShaderGUI.FindProperty("_RaveCC", properties);
            raveMask = ShaderGUI.FindProperty("_RaveMask", properties);
            raveColor = ShaderGUI.FindProperty("_RaveColor", properties);
            raveSecondary = ShaderGUI.FindProperty("_RaveSecondaryColor", properties);
            raveTertiary = ShaderGUI.FindProperty("_RaveTertiaryColor", properties);
            raveQuaternary = ShaderGUI.FindProperty("_RaveQuaternaryColor", properties);
            raveRG = ShaderGUI.FindProperty("_RaveRG", properties);
            raveBA = ShaderGUI.FindProperty("_RaveBA", properties);
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

            #region Textures and Colors

            //Textures
            GUILayout.Space(4f);
            GUILayout.Label("Main Textures", EditorStyles.boldLabel);
            materialEditor.TextureProperty(mainTex, "Diffuse", false);
            //desaturation toggle
            colorTexToggle = GUILayout.Toggle(colorTexture.floatValue > 0 ? true : false, MakeLabel(" Desaturate?", "This checkbox desaturates the Diffuse texture, eliminating existing colors to improve color control accuracy."));

            materialEditor.TextureProperty(cc, "Color Control (CC)", false);
            materialEditor.TextureProperty(normal, "Normal Map (DirectX)", false);
            materialEditor.TextureCompatibilityWarning(normal);
            materialEditor.TextureScaleOffsetProperty(mainTex);

            //Colors
            GUILayout.Space(8f);
            GUILayout.Label(MakeLabel("Main Colors", "These are the colors assigned to RGB zones of the CC."), EditorStyles.boldLabel);
            materialEditor.ColorProperty(primaryColor, "Primary Color (R)");
            materialEditor.ColorProperty(secondaryColor, "Secondary Color (G)");
            materialEditor.ColorProperty(tertiaryColor, "Tertiary Color (B)");

            //Values
            GUILayout.Space(8f);
            GUILayout.Label("Properties", EditorStyles.boldLabel);
            materialEditor.RangeProperty(metalness, "Metalness");
            materialEditor.RangeProperty(roughness, "Roughness (1 - 0)");
            materialEditor.RangeProperty(deepness, "Color Depth");

            //Toggles
            GUILayout.Space(8f);
            GUILayout.Label("Toggles", EditorStyles.boldLabel);
            simpleRoughnessToggle = GUILayout.Toggle(simpleRoughnessToggle, MakeLabel(" Ignore Texture Roughness?", "This checkbox forces the shader to use the Roughness property as the roughness value, with no additional calculations. (i.e., a roughness of 1 is no smoothness, roughness of 0.5 is half-smoothness)"));

            GUILayout.Space(8f);

            #endregion

            #region Drawers

            //Aurora
            if (GUILayout.Button(MakeLabel("Show Aurora", "Displays the Aurora properties section of the shader.")))
            {
                showAurora = !showAurora;
            }
            if (showAurora)
            {
                GUILayout.Space(4f);
                materialEditor.TextureProperty(auroraTex, "Aurora Texture", false);
                GUILayout.Space(4f);

                GUILayout.Label("Illumination", EditorStyles.boldLabel);
                materialEditor.ColorProperty(illuminationColor, "Color (HDR)");
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
                GUILayout.Space(4f);

                GUILayout.Space(4f);
                GUILayout.Label("Pattern", EditorStyles.boldLabel);
                materialEditor.TextureProperty(pattern, "Pattern (RGBA)", false);
                GUILayout.BeginHorizontal();
                GUILayout.Label(MakeLabel("Pattern Tiling", "The tiling applied to the pattern texture"));
                GUILayout.FlexibleSpace();
                pattern.textureScaleAndOffset = EditorGUILayout.Vector2Field("", pattern.textureScaleAndOffset);
                GUILayout.EndHorizontal();
                GUILayout.Space(4f);
            }

            //Custom Textures
            if (GUILayout.Button(MakeLabel("Show Alt Textures", "Displays the alt textures section (i.e., Roughness, Occlusion, etc) of the shader. This will always be visible if there are filled properties.")))
            {
                showAltTextures = !showAltTextures;
            }
            if (showAltTextures)
            {
                GUILayout.Space(4f);
                GUILayout.Label("Custom Textures", EditorStyles.boldLabel);
                materialEditor.TextureProperty(reflectCube, "Reflection Cubemap");
                GUILayout.Space(10f);
                GUILayout.Label("Decal Textures (UV2)", EditorStyles.boldLabel);
                materialEditor.TextureProperty(decals, "Decals (RGB+A)", false);
                materialEditor.TextureProperty(decalNormal, "Decal Normal (RGB+A)", false);

                GUILayout.Space(10f);
                GUILayout.Label("Detail Textures (UV)", EditorStyles.boldLabel);
                materialEditor.TextureProperty(detailTex, "Detail Map", false);
                materialEditor.TextureProperty(detailNormal, "Detail Normal (OpenGL)", false);
                materialEditor.RangeProperty(detailStrength, "Strength");
                materialEditor.TextureScaleOffsetProperty(detailTex);
            }

            //Rave Section
            if (GUILayout.Button(MakeLabel("Show Rave Section", "Displays the 'rave' section for additional emission. This will always be visible if there are filled properties.")))
            {
                showRave = !showRave;
            }
            if (showRave)
            {
                GUILayout.Space(4f);
                materialEditor.TextureProperty(raveCC, "Rave CC");
                materialEditor.TextureProperty(raveMask, "Rave Mask");
                GUILayout.Space(10f);
                materialEditor.ColorProperty(raveColor, "Rave Color (HDR)");
                materialEditor.ColorProperty(raveSecondary, "Rave Secondary Color (HDR)");
                materialEditor.ColorProperty(raveTertiary, "Rave Tertiary Color (HDR)");
                materialEditor.ColorProperty(raveQuaternary, "Rave Quaternary Color (HDR)");
                GUILayout.Space(10f);
                materialEditor.VectorProperty(raveRG, "Rave Scroll R+G");
                materialEditor.VectorProperty(raveBA, "Rave Scroll B+A");

                if (audioLinkCheckComplete)
                {
                    if (supportsAudioLink)
                    {
                        GUILayout.Space(10f);
                        useAudioLink = GUILayout.Toggle(useAudioLink, new GUIContent("Use VRC AudioLink?", "Check if you would like the rave effect's strength to be modified by llealloo's 'VRC Udon Audio Link'"));
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
            if (GUILayout.Button(MakeLabel("Show Helpers", "Displays additional tools and helper functions included with the Utility shader.")))
            {
                showHelpers = !showHelpers;
            }
            if (showHelpers)
            {
                GUILayout.Space(4f);
                GUILayout.Label("Helpers and Utilities", EditorStyles.boldLabel);
                GUILayout.Space(4f);

                if (GUILayout.Button(new GUIContent("Pack Aurora Textures", "Packs existing metallic, occlusion, illumination, and other textures into the 'Aurora' format."), GUILayout.Width(250f)))
                {
                    AuroraAR2Packer.Init();
                }

                //DECOMPOSERS
                GUILayout.Label("Game Texture Decomposers", EditorStyles.label);
                GUILayout.Space(2f);
                if (GUILayout.Button(new GUIContent("Decompose Halo CE Textures", "Decomposes assigned Halo: Combat Evolved 'Multipurpose' textures into illumination, CC, metallic smoothness, and roughness textures."), GUILayout.Width(250f)))
                {
                    //run the cyborg decompose
                    CyborgDecomposer.DecomposeMultipurposeTexture(cc.textureValue as Texture2D, cc.textureValue as UnityEngine.Object);
                }
                GUILayout.Space(4f);
                if (GUILayout.Button(new GUIContent("Decompose Storm Textures", "Decomposes assigned Halo 4 'Storm' textures into traditional diffuse, metallic/specular ('technically' your choice), and Halo 5-compatible color control textures."), GUILayout.Width(250f)))
                {
                    //run the storm decompose
                    StormDecomposer.DecomposeStormTexture(mainTex.textureValue as Texture2D, mainTex.textureValue as UnityEngine.Object);
                }
                GUILayout.Space(4f);
                if (GUILayout.Button(new GUIContent("Decompose Halo 5 Textures", "Decomposes assigned Halo 5 Forge textures into Diffuse RGB + Roughness Alpha, and RG color control textures, and applies them."), GUILayout.Width(250f)))
                {
                    //run the fforge decompose
                    //apply the result as the assigned textures
                    Texture2D[] temp = ForgeDecomposer.DecomposeForgeTextures(mainTex.textureValue as UnityEngine.Object, cc.textureValue as UnityEngine.Object);
                    if (temp != null && temp.Length > 2)
                    {
                        target.SetTexture("_MainTex", temp[0]);
                        target.SetTexture("_CC", temp[1]);
                        target.SetTexture("_MetallicTex", temp[2]);
                    }
                }
                GUILayout.Space(4f);

                //OTHER UTILITIES
                GUILayout.Label("Other Utilities", EditorStyles.label);
                GUILayout.Space(4f);
                if (GUILayout.Button(new GUIContent("DX2GL Normal Conversion", "Creates and applies an OpenGL Normal Map from an assigned DirectX Normal or vice-versa."), GUILayout.Width(250f)))
                {
                    //run the opengl/dx conversion
                    //apply the result as the assigned texture
                    normal.textureValue = DX2GLNormalConverter.ConvertToDXOGL(normal.textureValue as Texture2D, normal.textureValue as UnityEngine.Object);
                }
                GUILayout.Space(4f);
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Bake Material", "Bakes the material's output into a texture, optionally including environment lighting."), GUILayout.Width(250f)))
                {
                    //run the baking process
                    AuroraAR2Baker.BakeMaterialAsTexture((Material)materialEditor.target, !bypassLighting);
                }
                bypassLighting = GUILayout.Toggle(bypassLighting, MakeLabel("Include Lighting?", "Bake the current environment sample and point/directional lighting into the texture."));
                GUILayout.EndHorizontal();
                GUILayout.Space(6f);
            }

            #endregion

            CopyScaleOffsetMain();
            CopyScaleOffsetSecondary();

            if (EditorGUI.EndChangeCheck())
            {
                materialEditor.PropertiesChanged();

                gotProperties = false;
            }

            if (GUILayout.Button(new GUIContent("About...", "Displays the About pop-up for the shader, including a short FAQ and links to source.")))
            {
                //display about Window
                AuroraAR2AboutWindow.Init();
            }

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

        private void CopyScaleOffsetSecondary()
        {
            detailNormal.textureScaleAndOffset = detailTex.textureScaleAndOffset;
        }

        private void ApplyShaderFeatures(Material mat)
        {
            if (reflectCube.textureValue) { mat.EnableKeyword("_CUBE_REFLECTION"); } else { mat.DisableKeyword("_CUBE_REFLECTION"); }

            if (detailNormal.textureValue || detailTex.textureValue) { mat.EnableKeyword("_DETAIL_TEXTURE"); } else { mat.DisableKeyword("_DETAIL_TEXTURE"); }

            if (pattern.textureValue) { mat.EnableKeyword("_PATTERN"); } else { mat.DisableKeyword("_PATTERN"); }

            if (simpleRoughnessToggle) { mat.EnableKeyword("_SIMPLE_ROUGHNESS"); } else { mat.DisableKeyword("_SIMPLE_ROUGHNESS"); }

            if (illuminationColor.colorValue != Color.black) { mat.EnableKeyword("_ILLUMINATION"); } else { mat.DisableKeyword("_ILLUMINATION"); }

            if (decals.textureValue != null || decalNormal.textureValue != null) { mat.EnableKeyword("_DECALS"); } else { mat.DisableKeyword("_DECALS"); }

            if (raveCC.textureValue != null) { mat.EnableKeyword("_RAVE"); } else { mat.DisableKeyword("_RAVE"); }


            //Only if the project supports audio link which is determined by checking the PackageManager's list of dependencies
            if (!supportsAudioLink) { return; }
            if (useAudioLink) { mat.EnableKeyword("_VRCAUDIOLINK"); } else {  mat.DisableKeyword("_VRCAUDIOLINK"); }
        }

        private void ApplyToggles(Material mat)
        {
            mat.SetFloat("_AlphaRoughness", alphaRoughnessToggle ? 1 : 0);
            mat.SetFloat("_SimpleRoughness", simpleRoughnessToggle ? 1 : 0);
            mat.SetFloat("_ColorTexture", colorTexToggle ? 1 : 0);
        }

        private GUIContent MakeLabel(string displayName, string tooltip = null)
        {
            GUIContent staticLabel = new GUIContent();
            staticLabel.text = displayName;
            staticLabel.tooltip = tooltip;
            return staticLabel;
        }

        /// <summary>
        /// Tests if two float values are within a tolerance of each other.
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