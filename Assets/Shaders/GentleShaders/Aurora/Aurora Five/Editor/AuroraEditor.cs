using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GentleShaders.Aurora.Five
{
    [CanEditMultipleObjects]
    public class AuroraEditor : ShaderGUI
    {
        #region Material Properties

        //properties
        MaterialProperty _MainTex;
        MaterialProperty _BumpMap;
        MaterialProperty _CC;
        MaterialProperty _Aurora;
        MaterialProperty _RaveCC;

        MaterialProperty _MainTex1;
        MaterialProperty _BumpMap1;
        MaterialProperty _CC1;
        MaterialProperty _Aurora1;
        MaterialProperty _RaveCC1;

        MaterialProperty _MainTex2;
        MaterialProperty _BumpMap2;
        MaterialProperty _CC2;
        MaterialProperty _Aurora2;
        MaterialProperty _RaveCC2;

        MaterialProperty _MainTex3;
        MaterialProperty _BumpMap3;
        MaterialProperty _CC3;
        MaterialProperty _Aurora3;
        MaterialProperty _RaveCC3;

        MaterialProperty _MainTex4;
        MaterialProperty _BumpMap4;
        MaterialProperty _CC4;
        MaterialProperty _Aurora4;
        MaterialProperty _RaveCC4;

        MaterialProperty _Decals;
        MaterialProperty _DecalNormal;

        MaterialProperty _Color;
        MaterialProperty _SecondaryColor;
        MaterialProperty _TertiaryColor;
        MaterialProperty _IllumColor;

        MaterialProperty _trueMetallic;
        MaterialProperty _Roughness;
        MaterialProperty _Deepness;
        MaterialProperty _occlusionStrength;

        MaterialProperty _ColorTexture;

        MaterialProperty _RaveColor;
        MaterialProperty _RaveSecondaryColor;
        MaterialProperty _RaveTertiaryColor;
        MaterialProperty _RaveQuaternaryColor;

        MaterialProperty _RaveMask;
        MaterialProperty _RaveRG;
        MaterialProperty _RaveBA;

        MaterialProperty _DetailStrength;
        MaterialProperty _lightingBypass;
        MaterialProperty _accountForBLSH;
        MaterialProperty _giBoost;
        MaterialProperty _giBoostEnabled;

        MaterialProperty _useALThemeColor0;
        MaterialProperty _useALThemeColor1;
        MaterialProperty _useALThemeColor2;
        MaterialProperty _useALThemeColor3;

        MaterialProperty _chronotensityScroll0;
        MaterialProperty _chronotensityScroll1;
        MaterialProperty _chronotensityScroll2;
        MaterialProperty _chronotensityScroll3;

        MaterialProperty _audioLinkExclusive0;
        MaterialProperty _audioLinkExclusive1;
        MaterialProperty _audioLinkExclusive2;
        MaterialProperty _audioLinkExclusive3;

        MaterialProperty _audioLinkAdd0;
        MaterialProperty _audioLinkAdd1;
        MaterialProperty _audioLinkAdd2;
        MaterialProperty _audioLinkAdd3;

        MaterialProperty _ViewSpecularEnabled;
        MaterialProperty _ViewSpecular;
        MaterialProperty _ViewSpecularGain;
        MaterialProperty _ViewSpecularSpecSaturation;
        MaterialProperty _ViewSpecularSpecValue;
        MaterialProperty _ViewSpecularRoughnessTerm;
        MaterialProperty _ViewSpecularColorMixing;
        MaterialProperty _ViewSpecularColor;

        MaterialProperty _RSStrength;
        MaterialProperty _RSGain;

        MaterialProperty _RimLightingPower;
        MaterialProperty _RimLightingStrength;
        MaterialProperty _RimLightingDiffuseInfluence;
        MaterialProperty _RimLightingColorInfluence;
        MaterialProperty _RimLightingColor;

        MaterialProperty _ACELDiffuseStrength;
        MaterialProperty _ACELSpecularStrength;
        MaterialProperty _ACELAmbientDiffuseStrength;
        MaterialProperty _ACELAmbientSpecularStrength;
        MaterialProperty _ACELOutlineWidth;
        MaterialProperty _ACELOutlineStrength;
        MaterialProperty _ACELOutlineThreshold;

        MaterialProperty _TextureSetName_0;
        MaterialProperty _TextureSetName_1;
        MaterialProperty _TextureSetName_2;
        MaterialProperty _TextureSetName_3;
        MaterialProperty _TextureSetName_4;

        MaterialProperty _SrcBlend;
        MaterialProperty _DstBlend;
        MaterialProperty _ZWrite;
        MaterialProperty _ZTest;
        MaterialProperty _Cull;

        #endregion

        #region Editor Properties

        Material material;
        Texture2D headerImage;
        bool headerImageInitialized;
        int selectedTab;
        int shaderVariantId;
        int dimensionCount;

        bool uv2DrawerVisible;
        bool raveDrawerVisible;
        bool helperDrawerVisible;

        bool useAudioLink;
        bool supportsAudioLink;

        Aurora_GI_Setting giSetting;
        Aurora_BLSH_Setting blshSetting;
        Aurora_ViewSpecular_Setting igSetting;
        Aurora_Light_Model selectedLightModel;

        bool textureSet0_Visible = false;
        bool textureSet1_Visible = false;
        bool textureSet2_Visible = false;
        bool textureSet3_Visible = false;
        bool textureSet4_Visible = false;

        private string textureSetName0_Label;
        private string textureSetName1_Label;
        private string textureSetName2_Label;
        private string textureSetName3_Label;
        private string textureSetName4_Label;

        private const float FLOAT_POSITIVE = 0.001f;

        private bool performedUpdateCheck;
        private string updateMessageString = "";

        SL_ZWrite zWriteEnum = SL_ZWrite.On;
        SL_ZTest zTestEnum = SL_ZTest.LEqual;
        SL_Cull cullEnum = SL_Cull.Back;

        #endregion

        void GetMaterialProperties(MaterialProperty[] props)
        {
            _MainTex = FindProperty("_MainTex", props);
            _BumpMap = FindProperty("_BumpMap", props);
            _CC = FindProperty("_CC", props);
            _Aurora = FindProperty("_Aurora", props);
            _RaveCC = FindProperty("_RaveCC", props);

            _MainTex1 = FindProperty("_MainTex1", props);
            _BumpMap1 = FindProperty("_BumpMap1", props);
            _CC1 = FindProperty("_CC1", props);
            _Aurora1 = FindProperty("_Aurora1", props);
            _RaveCC1 = FindProperty("_RaveCC1", props);

            _MainTex2 = FindProperty("_MainTex2", props);
            _BumpMap2 = FindProperty("_BumpMap2", props);
            _CC2 = FindProperty("_CC2", props);
            _Aurora2 = FindProperty("_Aurora2", props);
            _RaveCC2 = FindProperty("_RaveCC2", props);

            _MainTex3 = FindProperty("_MainTex3", props);
            _BumpMap3 = FindProperty("_BumpMap3", props);
            _CC3 = FindProperty("_CC3", props);
            _Aurora3 = FindProperty("_Aurora3", props);
            _RaveCC3 = FindProperty("_RaveCC3", props);

            _MainTex4 = FindProperty("_MainTex4", props);
            _BumpMap4 = FindProperty("_BumpMap4", props);
            _CC4 = FindProperty("_CC4", props);
            _Aurora4 = FindProperty("_Aurora4", props);
            _RaveCC4 = FindProperty("_RaveCC4", props);

            _Decals = FindProperty("_Decals", props);
            _DecalNormal = FindProperty("_DecalNormal", props);

            _Color = FindProperty("_Color", props);
            _SecondaryColor = FindProperty("_SecondaryColor", props);
            _TertiaryColor = FindProperty("_TertiaryColor", props);
            _IllumColor = FindProperty("_IllumColor", props);

            _trueMetallic = FindProperty("_trueMetallic", props);
            _Roughness = FindProperty("_Roughness", props);
            _Deepness = FindProperty("_Deepness", props);
            _occlusionStrength = FindProperty("_occlusionStrength", props);

            _ColorTexture = FindProperty("_ColorTexture", props);

            _RaveColor = FindProperty("_RaveColor", props);
            _RaveSecondaryColor = FindProperty("_RaveSecondaryColor", props);
            _RaveTertiaryColor = FindProperty("_RaveTertiaryColor", props);
            _RaveQuaternaryColor = FindProperty("_RaveQuaternaryColor", props);

            _RaveMask = FindProperty("_RaveMask", props);
            _RaveRG = FindProperty("_RaveRG", props);
            _RaveBA = FindProperty("_RaveBA", props);

            _DetailStrength = FindProperty("_DetailStrength", props);
            _lightingBypass = FindProperty("_lightingBypass", props);
            _accountForBLSH = FindProperty("_accountForBLSH", props);
            _giBoost = FindProperty("_giBoost", props);
            _giBoostEnabled = FindProperty("_giBoostEnabled", props);

            _useALThemeColor0 = FindProperty("_useALThemeColor0", props);
            _useALThemeColor1 = FindProperty("_useALThemeColor1", props);
            _useALThemeColor2 = FindProperty("_useALThemeColor2", props);
            _useALThemeColor3 = FindProperty("_useALThemeColor3", props);

            _chronotensityScroll0 = FindProperty("_chronotensityScroll0", props);
            _chronotensityScroll1 = FindProperty("_chronotensityScroll1", props);
            _chronotensityScroll2 = FindProperty("_chronotensityScroll2", props);
            _chronotensityScroll3 = FindProperty("_chronotensityScroll3", props);

            _audioLinkExclusive0 = FindProperty("_audioLinkExclusive0", props);
            _audioLinkExclusive1 = FindProperty("_audioLinkExclusive1", props);
            _audioLinkExclusive2 = FindProperty("_audioLinkExclusive2", props);
            _audioLinkExclusive3 = FindProperty("_audioLinkExclusive3", props);

            _audioLinkAdd0 = FindProperty("_audioLinkAdd0", props);
            _audioLinkAdd1 = FindProperty("_audioLinkAdd1", props);
            _audioLinkAdd2 = FindProperty("_audioLinkAdd2", props);
            _audioLinkAdd3 = FindProperty("_audioLinkAdd3", props);

            _ViewSpecularEnabled = FindProperty("_ViewSpecularEnabled", props);
            _ViewSpecular = FindProperty("_ViewSpecular", props);
            _ViewSpecularGain = FindProperty("_ViewSpecularGain", props);
            _ViewSpecularSpecSaturation = FindProperty("_ViewSpecularSpecSaturation", props);
            _ViewSpecularSpecValue = FindProperty("_ViewSpecularSpecValue", props);
            _ViewSpecularRoughnessTerm = FindProperty("_ViewSpecularRoughnessTerm", props);
            _ViewSpecularColorMixing = FindProperty("_ViewSpecularColorMixing", props);
            _ViewSpecularColor = FindProperty("_ViewSpecularColor", props);

            _RSStrength = FindProperty("_RSStrength", props);
            _RSGain = FindProperty("_RSGain", props);

            _RimLightingPower = FindProperty("_RimLightingPower", props);
            _RimLightingStrength = FindProperty("_RimLightingStrength", props);
            _RimLightingDiffuseInfluence = FindProperty("_RimLightingDiffuseInfluence", props);
            _RimLightingColorInfluence = FindProperty("_RimLightingColorInfluence", props);
            _RimLightingColor = FindProperty("_RimLightingColor", props);

            _ACELDiffuseStrength = FindProperty("_ACELDiffuseStrength", props);
            _ACELSpecularStrength = FindProperty("_ACELSpecularStrength", props);
            _ACELAmbientDiffuseStrength = FindProperty("_ACELAmbientDiffuseStrength", props);
            _ACELAmbientSpecularStrength = FindProperty("_ACELAmbientSpecularStrength", props);
            _ACELOutlineWidth = FindProperty("_ACELOutlineWidth", props);
            _ACELOutlineStrength = FindProperty("_ACELOutlineStrength", props);
            _ACELOutlineThreshold = FindProperty("_ACELOutlineThreshold", props);

            _TextureSetName_0 = FindProperty("_TextureSetName_0", props);
            _TextureSetName_1 = FindProperty("_TextureSetName_1", props);
            _TextureSetName_2 = FindProperty("_TextureSetName_2", props);
            _TextureSetName_3 = FindProperty("_TextureSetName_3", props);
            _TextureSetName_4 = FindProperty("_TextureSetName_4", props);

            _SrcBlend = FindProperty("_SrcBlend", props);
            _DstBlend = FindProperty("_DstBlend", props);
            _ZWrite = FindProperty("_ZWrite", props);
            _ZTest = FindProperty("_ZTest", props);
            _Cull = FindProperty("_Cull", props);
        }

        void UpdateEditorProperties(Material mat)
        {
            if (_TextureSetName_0.vectorValue.x == 66658369 || _TextureSetName_0.vectorValue.x == 83698448) { _TextureSetName_0.vectorValue = StringToASCIIVector("SET 0"); }
            if (_TextureSetName_1.vectorValue.x == 66658368 || _TextureSetName_1.vectorValue.x == 83698448) { _TextureSetName_1.vectorValue = StringToASCIIVector("SET 1"); }
            if (_TextureSetName_2.vectorValue.x == 66658368 || _TextureSetName_2.vectorValue.x == 83698448) { _TextureSetName_2.vectorValue = StringToASCIIVector("SET 2"); }
            if (_TextureSetName_3.vectorValue.x == 66658368 || _TextureSetName_3.vectorValue.x == 83698448) { _TextureSetName_3.vectorValue = StringToASCIIVector("SET 3"); }
            if (_TextureSetName_4.vectorValue.x == 66658368 || _TextureSetName_4.vectorValue.x == 83698448) { _TextureSetName_4.vectorValue = StringToASCIIVector("SET 4"); }

            textureSetName0_Label = ASCIIFloatToString(_TextureSetName_0.vectorValue.x) + ASCIIFloatToString(_TextureSetName_0.vectorValue.y)
                + ASCIIFloatToString(_TextureSetName_0.vectorValue.z) + ASCIIFloatToString(_TextureSetName_0.vectorValue.w);

            textureSetName1_Label = ASCIIFloatToString(_TextureSetName_1.vectorValue.x) + ASCIIFloatToString(_TextureSetName_1.vectorValue.y)
                + ASCIIFloatToString(_TextureSetName_1.vectorValue.z) + ASCIIFloatToString(_TextureSetName_1.vectorValue.w);

            textureSetName2_Label = ASCIIFloatToString(_TextureSetName_2.vectorValue.x) + ASCIIFloatToString(_TextureSetName_2.vectorValue.y)
                + ASCIIFloatToString(_TextureSetName_2.vectorValue.z) + ASCIIFloatToString(_TextureSetName_2.vectorValue.w);

            textureSetName3_Label = ASCIIFloatToString(_TextureSetName_3.vectorValue.x) + ASCIIFloatToString(_TextureSetName_3.vectorValue.y)
                + ASCIIFloatToString(_TextureSetName_3.vectorValue.z) + ASCIIFloatToString(_TextureSetName_3.vectorValue.w);

            textureSetName4_Label = ASCIIFloatToString(_TextureSetName_4.vectorValue.x) + ASCIIFloatToString(_TextureSetName_4.vectorValue.y)
                + ASCIIFloatToString(_TextureSetName_4.vectorValue.z) + ASCIIFloatToString(_TextureSetName_4.vectorValue.w);

            if (dimensionCount == 0)
            {
                dimensionCount += mat.IsKeywordEnabled("_U1") ? 1 : 0;
                dimensionCount += mat.IsKeywordEnabled("_U2") ? 1 : 0;
                dimensionCount += mat.IsKeywordEnabled("_U3") ? 1 : 0;
                dimensionCount += mat.IsKeywordEnabled("_U4") ? 1 : 0;
            }
            // Hide default textures if there is more than one UV set
            if (dimensionCount == 0)
            {
                textureSet0_Visible = true;
            }

            giSetting = _giBoostEnabled.floatValue > FLOAT_POSITIVE ? Aurora_GI_Setting.Custom : Aurora_GI_Setting.Normal;
            blshSetting = _accountForBLSH.floatValue > FLOAT_POSITIVE ? Aurora_BLSH_Setting.AccountForBLSH : Aurora_BLSH_Setting.Ignore;
            igSetting = _ViewSpecularEnabled.floatValue > FLOAT_POSITIVE ? Aurora_ViewSpecular_Setting.Enabled : Aurora_ViewSpecular_Setting.Disabled;

            zWriteEnum = _ZWrite.floatValue > FLOAT_POSITIVE ? SL_ZWrite.On : SL_ZWrite.Off;
            zTestEnum = (SL_ZTest)Mathf.RoundToInt(_ZTest.floatValue);
            cullEnum = (SL_Cull)Mathf.RoundToInt(_Cull.floatValue);

            selectedLightModel = mat.IsKeywordEnabled("_BRDF4") ? Aurora_Light_Model.BRDF4 : Aurora_Light_Model.BRDF5;
            bool brdf4 = mat.IsKeywordEnabled("_BRDF4");
            bool brdf5 = mat.IsKeywordEnabled("_BRDF5");
            bool acell = mat.IsKeywordEnabled("_ACEL");
            if (brdf4) { selectedLightModel = Aurora_Light_Model.BRDF4; }
            if (acell) { selectedLightModel = Aurora_Light_Model.ACEL; }
            if (brdf5) { selectedLightModel = Aurora_Light_Model.BRDF5; }

            // Check for AudioLink
            for (int i = 0; i < Shader.globalKeywords.Length; i++)
            {
                if (Shader.globalKeywords[i].name.ToUpper().Contains("AUDIOLINK"))
                {
                    supportsAudioLink = true;
                }
            }
        }

        void UpdateMaterialProperties()
        {
            _TextureSetName_0.vectorValue = StringToASCIIVector(textureSetName0_Label.ToUpper());
            _TextureSetName_1.vectorValue = StringToASCIIVector(textureSetName1_Label.ToUpper());
            _TextureSetName_2.vectorValue = StringToASCIIVector(textureSetName2_Label.ToUpper());
            _TextureSetName_3.vectorValue = StringToASCIIVector(textureSetName3_Label.ToUpper());
            _TextureSetName_4.vectorValue = StringToASCIIVector(textureSetName4_Label.ToUpper());

            _accountForBLSH.floatValue = blshSetting == Aurora_BLSH_Setting.AccountForBLSH ? 1.0f : 0.0f;
            _giBoostEnabled.floatValue = giSetting == Aurora_GI_Setting.Custom ? 1.0f : 0.0f;
            _ViewSpecularEnabled.floatValue = igSetting == Aurora_ViewSpecular_Setting.Enabled ? 1.0f : 0.0f;

            _ZWrite.floatValue = (int)zWriteEnum;
            _ZTest.floatValue = (int)zTestEnum;
            _Cull.floatValue = (int)cullEnum;
        }

        private void FindHeaderImage()
        {
            if (headerImageInitialized) { return; }
            headerImage = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Shaders/GentleShaders/Aurora/Aurora Five/Editor/Assets/Avrora_Five_Header.png", typeof(Texture2D)) ?? new Texture2D(1, 1);
            headerImageInitialized = true;
        }

        private void SetKeywords(Material mat)
        {
            if (useAudioLink)
            {
                mat.EnableKeyword("_VRCAUDIOLINK");
            }
            else
            {
                mat.DisableKeyword("_VRCAUDIOLINK");
            }

            switch (dimensionCount)
            {
                case 0:
                    mat.DisableKeyword("_U1");
                    mat.DisableKeyword("_U2");
                    mat.DisableKeyword("_U3");
                    mat.DisableKeyword("_U4");
                    break;
                case 1:
                    mat.EnableKeyword("_U1");
                    mat.DisableKeyword("_U2");
                    mat.DisableKeyword("_U3");
                    mat.DisableKeyword("_U4");
                    break;
                case 2:
                    mat.EnableKeyword("_U1");
                    mat.EnableKeyword("_U2");
                    mat.DisableKeyword("_U3");
                    mat.DisableKeyword("_U4");
                    break;
                case 3:
                    mat.EnableKeyword("_U1");
                    mat.EnableKeyword("_U2");
                    mat.EnableKeyword("_U3");
                    mat.DisableKeyword("_U4");
                    break;
                case 4:
                    mat.EnableKeyword("_U1");
                    mat.EnableKeyword("_U2");
                    mat.EnableKeyword("_U3");
                    mat.EnableKeyword("_U4");
                    break;
            }

            if (selectedLightModel == Aurora_Light_Model.BRDF4)
            {
                mat.EnableKeyword("_BRDF4");
                mat.DisableKeyword("_BRDF5");
                mat.DisableKeyword("_ACEL");
            }
            else if (selectedLightModel == Aurora_Light_Model.ACEL)
            {
                mat.EnableKeyword("_ACEL");
                mat.DisableKeyword("_BRDF4");
                mat.DisableKeyword("_BRDF5");
            }
            else if (selectedLightModel == Aurora_Light_Model.BRDF5)
            {
                mat.EnableKeyword("_BRDF5");
                mat.DisableKeyword("_BRDF4");
                mat.DisableKeyword("_ACEL");
            }
        }

        private async Task CheckForUpdate()
        {
            performedUpdateCheck = true;
            updateMessageString = await AuroraUpdateChecker.PerformUpdateCheck();
        }

        private void DrawUpdateGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(updateMessageString.Replace("<", "").Replace(">", ""), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            if (updateMessageString.Contains(">"))
            {
                if (GUILayout.Button(new GUIContent("Open Repository", "Opens the Aurora Shader GitHub Repository")))
                {
                    AuroraCommon.OpenRepositoryReleases();
                }
            }
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (!material)
            {
                material = materialEditor.target as Material;
            }
            FindHeaderImage();

            #pragma warning disable CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            // Note: This is intentional!
            if (!performedUpdateCheck) { CheckForUpdate(); }
            #pragma warning restore CS4014
            // Because this call is not awaited, execution of the current method continues before the call is completed
            DrawUpdateGUI();

            GetMaterialProperties(properties);
            UpdateEditorProperties(material);

            AuroraGUI(material, materialEditor);

            UpdateMaterialProperties();
            SetKeywords(material);
        }

        private void AuroraGUI(Material mat, MaterialEditor editor)
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(headerImage, GUILayout.MinWidth(240), GUILayout.MaxWidth(445), GUILayout.MinHeight(50), GUILayout.MaxHeight(200));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(10f);

            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Material", "Shader", "Statistics" });

            switch (selectedTab)
            {
                case 0:
                    AuroraMaterialGUI(material, editor);
                    break;
                case 1:
                    AuroraShaderSettingsGUI(material, editor);
                    break;
                case 2:
                    AuroraStatisticsGUI(material, editor);
                    break;
            }
        }


        private void AuroraShaderSettingsGUI(Material mat, MaterialEditor editor)
        {
            GUILayout.Space(10f);
            if (selectedLightModel == Aurora_Light_Model.ACEL)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();

                GUILayout.Label("Please note: ACEL lighting model is in early alpha!", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);

            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            selectedLightModel = (Aurora_Light_Model)EditorGUILayout.EnumPopup("Light Model *", selectedLightModel);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            giSetting = (Aurora_GI_Setting)EditorGUILayout.EnumPopup("Global Illumination Setting *", giSetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            if (giSetting == Aurora_GI_Setting.Custom)
            {
                _giBoost.floatValue = editor.RangeProperty(_giBoost, "GI Boost *");
            }
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            blshSetting = (Aurora_BLSH_Setting)EditorGUILayout.EnumPopup("Baked Light Setting *", blshSetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            igSetting = (Aurora_ViewSpecular_Setting)EditorGUILayout.EnumPopup("View Gloss Setting *", igSetting);
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();
            if (igSetting == Aurora_ViewSpecular_Setting.Enabled)
            {
                GUILayout.Space(10f);
                _ViewSpecular.floatValue = editor.RangeProperty(_ViewSpecular, "View Specular *");
                _ViewSpecularGain.floatValue = editor.RangeProperty(_ViewSpecularGain, "View Specular Gain *");
                _ViewSpecularSpecSaturation.floatValue = editor.RangeProperty(_ViewSpecularSpecSaturation, "View Specular Specular Saturation *");
                _ViewSpecularSpecValue.floatValue = editor.RangeProperty(_ViewSpecularSpecValue, "View Specular Specular Value *");
                _ViewSpecularRoughnessTerm.floatValue = editor.RangeProperty(_ViewSpecularRoughnessTerm, "View Specular Roughness Term *");
                _ViewSpecularColorMixing.floatValue = editor.RangeProperty(_ViewSpecularColorMixing, "View Specular Color Mix *");
                _ViewSpecularColor.colorValue = editor.ColorProperty(_ViewSpecularColor, "View Specular Color *");
            }

            GUILayout.Space(30f);
            GUILayout.BeginHorizontal();
            GUILayout.Space(4f);
            GUILayout.TextArea("Settings marked with an asterisk (*) are settings that may change the lighting model such that it is no longer energy-conserving. " +
                "However, some settings may appear more 'realistic' than an otherwise energy-conserving model." +
                "\n\nFor example, the default 'Baked Lighting Setting' is set to account for baked lights not providing a light direction for specularity. This often results in a more grounded appearance despite violating energy conservation.");
            GUILayout.Space(4f);
            GUILayout.EndHorizontal();

            GUILayout.Space(30f);
            zWriteEnum = (SL_ZWrite)EditorGUILayout.EnumPopup("Z-Write", zWriteEnum);
            GUILayout.Space(10f);
            zTestEnum = (SL_ZTest)EditorGUILayout.EnumPopup("Z-Test", zTestEnum);
            GUILayout.Space(10f);
            cullEnum = (SL_Cull)EditorGUILayout.EnumPopup("Face Culling", cullEnum);
        }


        private void AuroraMaterialGUI(Material mat, MaterialEditor editor)
        {
            #region Texture Drawers
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(textureSetName0_Label, "Displays the section for the 0-1 UV space textures.")))
            {
                textureSet0_Visible = !textureSet0_Visible;
            }
            GUILayout.Button(new GUIContent("", ""), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f));
            GUILayout.EndHorizontal();
            if (textureSet0_Visible)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.LabelField("Texture Set Name: ");
                GUILayout.FlexibleSpace();
                textureSetName0_Label = EditorGUILayout.TextField(textureSetName0_Label);
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                editor.TextureProperty(_Aurora, "Aurora Texture", false);
                editor.TextureProperty(_MainTex, "Diffuse", false);
                editor.TextureProperty(_BumpMap, "Normal Map (DirectX)", false);
                editor.TextureProperty(_CC, "Color Control (RGBA)", false);
                editor.TextureProperty(_RaveCC, "Rave CC (RGBA)", false);
            }

            if(dimensionCount > 0)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(textureSetName1_Label, "Displays the section for the 1-2 UV space textures.")))
                {
                    textureSet1_Visible = !textureSet1_Visible;
                }
                if (dimensionCount < 2)
                {
                    if (GUILayout.Button(new GUIContent("X", "Removes this texture set and assigned properties."), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f)))
                    {
                        dimensionCount--;
                        _MainTex1.textureValue = null;
                        _Aurora1.textureValue = null;
                        _BumpMap1.textureValue = null;
                        _CC1.textureValue = null;
                        _RaveCC1.textureValue = null;
                    }
                }
                else
                {
                    GUILayout.Button(new GUIContent("", ""), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f));
                }
                GUILayout.EndHorizontal();
                if (textureSet1_Visible)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Texture Set Name: ");
                    GUILayout.FlexibleSpace();
                    textureSetName1_Label = EditorGUILayout.TextField(textureSetName1_Label);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    editor.TextureProperty(_Aurora1, "Aurora Texture", false);
                    editor.TextureProperty(_MainTex1, "Diffuse", false);
                    editor.TextureProperty(_BumpMap1, "Normal Map (DirectX)", false);
                    editor.TextureProperty(_CC1, "Color Control (RGBA)", false);
                    editor.TextureProperty(_RaveCC1, "Rave CC (RGBA)", false);
                }
            }

            if(dimensionCount > 1)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(textureSetName2_Label, "Displays the section for the 2-3 UV space textures.")))
                {
                    textureSet2_Visible = !textureSet2_Visible;
                }
                if (dimensionCount < 3)
                {
                    if (GUILayout.Button(new GUIContent("X", "Removes this texture set and assigned properties."), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f)))
                    {
                        dimensionCount--;
                        _MainTex2.textureValue = null;
                        _Aurora2.textureValue = null;
                        _BumpMap2.textureValue = null;
                        _CC2.textureValue = null;
                        _RaveCC2.textureValue = null;
                    }
                }
                else
                {
                    GUILayout.Button(new GUIContent("", ""), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f));
                }
                GUILayout.EndHorizontal();
                if (textureSet2_Visible)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Texture Set Name: ");
                    GUILayout.FlexibleSpace();
                    textureSetName2_Label = EditorGUILayout.TextField(textureSetName2_Label);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    editor.TextureProperty(_Aurora2, "Aurora Texture", false);
                    editor.TextureProperty(_MainTex2, "Diffuse", false);
                    editor.TextureProperty(_BumpMap2, "Normal Map (DirectX)", false);
                    editor.TextureProperty(_CC2, "Color Control (RGBA)", false);
                    editor.TextureProperty(_RaveCC2, "Rave CC (RGBA)", false);
                }
            }
            
            if(dimensionCount > 2)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(textureSetName3_Label, "Displays the section for the 3-4 UV space textures.")))
                {
                    textureSet3_Visible = !textureSet3_Visible;
                }
                if (dimensionCount < 4)
                {
                    if (GUILayout.Button(new GUIContent("X", "Removes this texture set and assigned properties."), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f)))
                    {
                        dimensionCount--;
                        _MainTex3.textureValue = null;
                        _Aurora3.textureValue = null;
                        _BumpMap3.textureValue = null;
                        _CC3.textureValue = null;
                        _RaveCC3.textureValue = null;
                    }
                }
                else
                {
                    GUILayout.Button(new GUIContent("", ""), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f));
                }
                GUILayout.EndHorizontal();
                if (textureSet3_Visible)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Texture Set Name: ");
                    GUILayout.FlexibleSpace();
                    textureSetName3_Label = EditorGUILayout.TextField(textureSetName3_Label);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    editor.TextureProperty(_Aurora3, "Aurora Texture", false);
                    editor.TextureProperty(_MainTex3, "Diffuse", false);
                    editor.TextureProperty(_BumpMap3, "Normal Map (DirectX)", false);
                    editor.TextureProperty(_CC3, "Color Control (RGBA)", false);
                    editor.TextureProperty(_RaveCC3, "Rave CC (RGBA)", false);
                }
            }
            
            if(dimensionCount > 3)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent(textureSetName4_Label, "Displays the section for the 4-5 UV space textures.")))
                {
                    textureSet4_Visible = !textureSet4_Visible;
                }
                if (GUILayout.Button(new GUIContent("X", "Removes this texture set and assigned properties."), GUILayout.MinWidth(25f), GUILayout.MaxWidth(25f)))
                {
                    dimensionCount--;
                    _MainTex4.textureValue = null;
                    _Aurora4.textureValue = null;
                    _BumpMap4.textureValue = null;
                    _CC4.textureValue = null;
                    _RaveCC4.textureValue = null;
                }
                EditorGUILayout.EndHorizontal();
                if (textureSet4_Visible)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Texture Set Name: ");
                    GUILayout.FlexibleSpace();
                    textureSetName4_Label = EditorGUILayout.TextField(textureSetName4_Label);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();

                    editor.TextureProperty(_Aurora4, "Aurora Texture", false);
                    editor.TextureProperty(_MainTex4, "Diffuse", false);
                    editor.TextureProperty(_BumpMap4, "Normal Map (DirectX)", false);
                    editor.TextureProperty(_CC4, "Color Control (RGBA)", false);
                    editor.TextureProperty(_RaveCC4, "Rave CC (RGBA)", false);
                }
            }

            GUILayout.Space(10f);
            if (dimensionCount >= 4)
            {
                if (GUILayout.Button(new GUIContent("Maximum Sets Reached", "Only 5 unique texture sets are supported at this time.")))
                {

                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Add Texture Set...", "Adds a horizontal UV texture set to the material.")))
                {
                    dimensionCount++;
                }
            }
            #endregion

            #region Colors and Values

            //Colors
            GUILayout.Space(8f);
            GUILayout.Label(new GUIContent("Main Colors", "These are the colors assigned to RGB zones of the CC."), EditorStyles.boldLabel);
            editor.ColorProperty(_Color, "Primary Color (R)");
            editor.ColorProperty(_SecondaryColor, "Secondary Color (G)");
            editor.ColorProperty(_TertiaryColor, "Tertiary Color (B)");

            GUILayout.Space(6f);
            GUILayout.Label("Illumination", EditorStyles.boldLabel);
            editor.ColorProperty(_IllumColor, "Color (HDR)");
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button(new GUIContent("Guess Illumination Color", "Attempts to create a neutral and appealing illumination color based on the chosen primary color."), GUILayout.Width(230f)))
            {
                if (Mathf.Abs(_Color.colorValue.r - _Color.colorValue.g) < 0.025f && Mathf.Abs(_Color.colorValue.g - _Color.colorValue.b) < 0.025f)
                {
                    _IllumColor.colorValue = new Color(0.00392156863f * 82f, 0.00392156863f * 137f, 0.00392156863f * 215f) * 2f;
                }
                else
                {
                    _IllumColor.colorValue = (_Color.colorValue * 1.25f) + (Color.white * 0.5f);
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(4f);

            //Values
            GUILayout.Space(8f);
            GUILayout.Label("Physical Properties", EditorStyles.boldLabel);
            editor.RangeProperty(_trueMetallic, "Metalness");
            editor.RangeProperty(_Roughness, "Roughness");
            editor.RangeProperty(_Deepness, "Color Depth");

            GUILayout.Space(8f);
            GUILayout.Label("Non-Physical Properties", EditorStyles.boldLabel);
            editor.RangeProperty(_occlusionStrength, "Occlusion Strength *");

            editor.RangeProperty(_RSStrength, "Radiance Scaling *");
            editor.RangeProperty(_RSGain, "Radiance Scaling Gain *");

            if (selectedLightModel == Aurora_Light_Model.ACEL)
            {
                GUILayout.Space(8f);
                GUILayout.Label("ACEL Properties", EditorStyles.boldLabel);
                GUILayout.Label("Rim Lighting", EditorStyles.miniBoldLabel);
                editor.RangeProperty(_RimLightingPower, "Rim Lighting Power");
                editor.RangeProperty(_RimLightingStrength, "Rim Lighting Strength");
                editor.RangeProperty(_RimLightingDiffuseInfluence, "Rim Lighting Diffuse Influence");
                editor.RangeProperty(_RimLightingColorInfluence, "Rim Lighting Color Influence");
                editor.ColorProperty(_RimLightingColor, "Rim Lighting Color");
                GUILayout.Space(4f);
                GUILayout.Label("Value Boosts", EditorStyles.miniBoldLabel);
                editor.RangeProperty(_ACELDiffuseStrength, "ACEL Diffuse Strength");
                editor.RangeProperty(_ACELSpecularStrength, "ACEL Specular Strength");
                editor.RangeProperty(_ACELAmbientDiffuseStrength, "ACEL Ambient Diffuse Strength");
                editor.RangeProperty(_ACELAmbientSpecularStrength, "ACEL Ambient Specular Strength");
                GUILayout.Space(4f);
                GUILayout.Label("Outline", EditorStyles.miniBoldLabel);
                editor.RangeProperty(_ACELOutlineWidth, "ACEL Outline Width");
                //editor.RangeProperty(_ACELOutlineStrength, "ACEL Outline Strength");
                //editor.RangeProperty(_ACELOutlineThreshold, "ACEL Outline Threshold");
            }

            GUILayout.Space(8f);

            #endregion

            #region Drawers

            //Custom Textures
            if (GUILayout.Button(new GUIContent("Show UV2 Section", "Show decal textures applied via UV2. This will always be visible if there are filled properties.")))
            {
                uv2DrawerVisible = !uv2DrawerVisible;
            }
            if (uv2DrawerVisible)
            {
                GUILayout.Space(10f);
                GUILayout.Label("Decal Textures (UV2)", EditorStyles.boldLabel);
                editor.TextureProperty(_Decals, "Decals (RGBA)", false);
                editor.TextureProperty(_DecalNormal, "Decal Normal", false);
            }

            //Rave Section
            if (GUILayout.Button(new GUIContent("Show Rave Section", "Displays the 'rave' section for additional emission and AudioLink. This will always be visible if there are filled properties.")))
            {
                raveDrawerVisible = !raveDrawerVisible;
            }
            if (raveDrawerVisible)
            {
                GUILayout.Space(4f);
                editor.TextureProperty(_RaveMask, "Rave Mask");
                GUILayout.Space(10f);

                GUILayout.BeginHorizontal();
                GUILayout.Label("R-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    _audioLinkExclusive0.intValue = GUILayout.Toggle(_audioLinkExclusive0.intValue > 0 ? true : false, new GUIContent("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink.")) ? 1 : 0;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    _useALThemeColor0.intValue = GUILayout.Toggle(_useALThemeColor0.intValue > 0 ? true : false, new GUIContent("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world.")) ? 1 : 0;
                }
                if (_useALThemeColor0.intValue < 1)
                {
                    GUILayout.FlexibleSpace();
                    editor.ColorProperty(_RaveColor, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("G-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    _audioLinkExclusive1.intValue = GUILayout.Toggle(_audioLinkExclusive1.intValue > 0 ? true : false, new GUIContent("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink.")) ? 1 : 0;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    _useALThemeColor1.intValue = GUILayout.Toggle(_useALThemeColor1.intValue > 0 ? true : false, new GUIContent("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world.")) ? 1 : 0;
                }
                if (_useALThemeColor1.intValue < 1)
                {
                    GUILayout.FlexibleSpace();
                    editor.ColorProperty(_RaveSecondaryColor, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("B-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    _audioLinkExclusive2.intValue = GUILayout.Toggle(_audioLinkExclusive2.intValue > 0 ? true : false, new GUIContent("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink.")) ? 1 : 0;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    _useALThemeColor2.intValue = GUILayout.Toggle(_useALThemeColor2.intValue > 0 ? true : false, new GUIContent("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world.")) ? 1 : 0;
                }
                if (_useALThemeColor2.intValue < 1)
                {
                    GUILayout.FlexibleSpace();
                    editor.ColorProperty(_RaveTertiaryColor, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("A-Color (HDR)");
                GUILayout.FlexibleSpace();
                if (useAudioLink)
                {
                    _audioLinkExclusive3.intValue = GUILayout.Toggle(_audioLinkExclusive3.intValue > 0 ? true : false, new GUIContent("AudioLink Exclusive?", "If enabled, the selected channel will not be visible in an environment without AudioLink.")) ? 1 : 0;
                }
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Space(15f);
                if (useAudioLink)
                {
                    _useALThemeColor3.intValue = GUILayout.Toggle(_useALThemeColor3.intValue > 0 ? true : false, new GUIContent("Use ThemeColor?", "If enabled, this color will be driven by the AudioLink Theme Color to match the avatar to the world.")) ? 1 : 0;
                }
                if (_useALThemeColor3.intValue < 1)
                {
                    GUILayout.FlexibleSpace();
                    editor.ColorProperty(_RaveQuaternaryColor, "");
                }
                else { GUILayout.FlexibleSpace(); }
                GUILayout.EndHorizontal();

                GUILayout.Space(10f);

                Vector2 red = new Vector2(_RaveRG.vectorValue.x, _RaveRG.vectorValue.y);
                Vector2 green = new Vector2(_RaveRG.vectorValue.z, _RaveRG.vectorValue.w);
                Vector2 blue = new Vector2(_RaveBA.vectorValue.x, _RaveBA.vectorValue.y);
                Vector2 alpha = new Vector2(_RaveBA.vectorValue.z, _RaveBA.vectorValue.w);

                if (useAudioLink)
                {
                    GUILayout.BeginHorizontal();
                    _chronotensityScroll0.intValue = GUILayout.Toggle(_chronotensityScroll0.intValue > 0 ? true : false, new GUIContent("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity.")) ? 1 : 0;
                    GUILayout.FlexibleSpace();
                    _chronotensityScroll1.intValue = GUILayout.Toggle(_chronotensityScroll1.intValue > 0 ? true : false, new GUIContent("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity.")) ? 1 : 0;
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
                    _chronotensityScroll2.intValue = GUILayout.Toggle(_chronotensityScroll2.intValue > 0 ? true : false, new GUIContent("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity.")) ? 1 : 0;
                    GUILayout.FlexibleSpace();
                    _chronotensityScroll3.intValue = GUILayout.Toggle(_chronotensityScroll3.intValue > 0 ? true : false, new GUIContent("Chronotensity-Scroll?", "Uses Chronotensity to speed up and slow down the scrolling based on audio intensity.")) ? 1 : 0;
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

                _RaveRG.vectorValue = new Vector4(red.x, red.y, green.x, green.y);
                _RaveBA.vectorValue = new Vector4(blue.x, blue.y, alpha.x, alpha.y);

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
                        if (GUILayout.Button(new GUIContent("Download AudioLink")))
                        {
                            Application.OpenURL("https://github.com/llealloo/vrc-udon-audio-link/releases");
                        }
                        if (GUILayout.Button(new GUIContent("Disable AudioLink")))
                        {
                            useAudioLink = false;
                            ((Material)editor.target).DisableKeyword("_VRCAUDIOLINK");
                        }
                        GUILayout.Space(10f);
                    }
                }
            }

            #endregion

            if (GUILayout.Button(new GUIContent("About...", "Displays the About pop-up for the shader, including a short FAQ and links to source.")))
            {
                //display about Window
                //AuroraAboutWindow.Init();
            }

        }

        private void AuroraStatisticsGUI(Material mat, MaterialEditor editor)
        {
            CalculateShaderVariantID(mat);

            int textureCount = 0;
            long textureByteCount = 0;
            if (_MainTex.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_MainTex.textureValue); }
            if (_MainTex1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_MainTex1.textureValue); }
            if (_MainTex2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_MainTex2.textureValue); }
            if (_MainTex3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_MainTex3.textureValue); }
            if (_MainTex4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_MainTex4.textureValue); }

            if (_Aurora.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_Aurora.textureValue); }
            if (_Aurora1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_Aurora1.textureValue); }
            if (_Aurora2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_Aurora2.textureValue); }
            if (_Aurora3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_Aurora3.textureValue); }
            if (_Aurora4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_Aurora4.textureValue); }

            if (_CC.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_CC.textureValue); }
            if (_CC1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_CC1.textureValue); }
            if (_CC2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_CC2.textureValue); }
            if (_CC3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_CC3.textureValue); }
            if (_CC4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_CC4.textureValue); }

            if (_BumpMap.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_BumpMap.textureValue); }
            if (_BumpMap1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_BumpMap1.textureValue); }
            if (_BumpMap2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_BumpMap2.textureValue); }
            if (_BumpMap3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_BumpMap3.textureValue); }
            if (_BumpMap4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_BumpMap4.textureValue); }

            if (_RaveCC.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_RaveCC.textureValue); }
            if (_RaveCC1.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_RaveCC1.textureValue); }
            if (_RaveCC2.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_RaveCC2.textureValue); }
            if (_RaveCC3.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_RaveCC3.textureValue); }
            if (_RaveCC4.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_RaveCC4.textureValue); }

            if (_RaveMask.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_RaveMask.textureValue); }
            if (_Decals.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_Decals.textureValue); }
            if (_DecalNormal.textureValue) { textureCount++; textureByteCount += AuroraCommon.GetUncompressedTexture2DByteCount((Texture2D)_DecalNormal.textureValue); }

            GUILayout.Space(40f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("Material Statistics", "Provides detailed information about the material."), EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
            EditorGUILayout.IntField("Texture Count", textureCount);
            GUILayout.Space(10f);
            EditorGUILayout.TextField("Uncompressed Texture Size (VRAM)", AuroraCommon.GetUncompressedTexture2DSizeString(textureByteCount));
            GUILayout.Space(10f);
            EditorGUILayout.IntField("Shader Variant ID", shaderVariantId);
            GUILayout.Space(10f);
            EditorGUILayout.TextField("Material Keywords", GetShaderKeywordString(mat));
            GUILayout.Space(10f);
            EditorGUILayout.TextField("Suite Version String", AuroraCommon.currentVersion);
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
            int variantBase = 0;
            int keywords = 0;
            int variantMod = 0;
            if (mat.shaderKeywords.Length > 0)
            {
                keywords = 2;
            }
            for (int i = 0; i < mat.shaderKeywords.Length; i++)
            {
                if(mat.shaderKeywords[i] == "_BRDF4")
                {
                    variantBase += 400;
                    continue;
                }
                if (mat.shaderKeywords[i] == "_BRDF5")
                {
                    variantBase += 500;
                    continue;
                }
                if (mat.shaderKeywords[i] == "_ACEL")
                {
                    variantBase += 200;
                    continue;
                }
                keywords *= 2;
            }
            for (int i = 0; i < mat.shaderKeywords.Length; i++)
            {
                if(mat.shaderKeywords[i] == "_RAVE")
                {
                    variantMod += 1;
                }
            }
            shaderVariantId = variantBase + keywords + variantMod;
        }

        private Vector4 StringToASCIIVector(string text)
        {
            if (text.Length < 12)
            {
                for (int i = text.Length - 1; i < 12; i++)
                {
                    text += " ";
                }
            }
            if (text.Length > 12)
            {
                text = text.Substring(0, 12);
            }

            string x = text.Substring(0, 3);
            string y = text.Substring(3, 3);
            string z = text.Substring(6, 3);
            string w = text.Substring(9, 3);

            int valueX = 0;
            for (int i = 0; i < 3; i++)
            {
                int character = (int)text[i];
                valueX += character;
                if (i < 2)
                {
                    valueX *= 100;
                }
            }
            int valueY = 0;
            for (int i = 3; i < 6; i++)
            {
                int character = (int)text[i];
                valueY += character;
                if (i < 5)
                {
                    valueY *= 100;
                }
            }
            int valueZ = 0;
            for (int i = 6; i < 9; i++)
            {
                int character = (int)text[i];
                valueZ += character;
                if (i < 8)
                {
                    valueZ *= 100;
                }
            }
            int valueW = 0;
            for (int i = 9; i < 12; i++)
            {
                int character = (int)text[i];
                valueW += character;
                if (i < 11)
                {
                    valueW *= 100;
                }
            }

            Vector4 vector = new Vector4(valueX, valueY, valueZ, valueW);

            return vector;
        }

        private string ASCIIFloatToString(float value)
        {
            int worker1, worker2, worker3 = 0;
            string text = "";

            //first char
            worker1 = (int)value;
            worker1 = worker1 / 10000;
            char character = (char)worker1;
            text += character;

            //second char
            worker2 = (int)value - (worker1 * 10000);
            worker2 = worker2 / 100;
            character = (char)worker2;
            text += character;

            //third char
            worker3 = ((int)value - (worker1 * 10000)) - (worker2 * 100);
            character = (char)worker3;
            text += character;

            return text;
        }
    }
}
