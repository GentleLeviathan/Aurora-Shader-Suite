using UnityEngine;
using UnityEditor;
using System.IO;
using GentleShaders.Aurora.Common;
using System;

namespace GentleShaders.Aurora.Helpers
{
    /// <summary>
    /// Aurora Shader included helper. Packs metallic, roughness, illumination, and occlusion textures into an Aurora A3 swizzle texture.
    /// WIP!
    /// </summary>
    public class AuroraPacker : EditorWindow
    {
        private static AuroraPacker window;
        private bool setup = false;
        private Vector2 scrollPos;

        private TextureChannels metallicChannel = TextureChannels.Red;
        private SurfaceFinishWorkflow surfaceFinishWorkflow = SurfaceFinishWorkflow.Roughness;
        private TextureChannels surfaceFinishChannel = TextureChannels.Red;
        private TextureChannels illuminationChannel = TextureChannels.Red;
        private TextureChannels occlusionChannel = TextureChannels.Green;

        private NormalMapFormat normalMapFormat = NormalMapFormat.OpenGL;

        private GUIStyle h1;
        private GUIStyle h2;
        private GUIStyle h3;
        private GUIStyle boldLabels;
        private GUIStyle common;

        private Texture2D diffuseTex;

        private Texture2D metallicTex;
        private Texture2D illuminationTex;
        private Texture2D roughnessTex;
        private Texture2D occlusionTex;

        private Texture2D normalMap;

        public static void Init()
        {
            window = (AuroraPacker)EditorWindow.GetWindow(typeof(AuroraPacker));
            window.titleContent = new GUIContent("Aurora - A3 Packer");
            window.minSize = new Vector2(480f, 600f);
            window.Show();
        }

        public static void Init(Texture2D diffuse)
        {
            window = (AuroraPacker)EditorWindow.GetWindow(typeof(AuroraPacker));
            window.titleContent = new GUIContent("Aurora - A3 Packer");
            window.minSize = new Vector2(480f, 600f);
            window.ShowWithDiffuse(diffuse);
        }

        public void ShowWithDiffuse(Texture2D diffuse)
        {
            this.Show();
            diffuseTex = diffuse;
        }

        private void OnGUI()
        {
            if (!setup) { SetupStyles(); }

            DrawHeader();
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            DrawAuroraPacker();
            DrawNormalConversion();
            GUILayout.EndScrollView();
        }

        private void SetupStyles()
        {
            //header
            Color text = new Color(0.8f, 0.8f, 0.8f);
            h1 = new GUIStyle();
            h1.alignment = TextAnchor.MiddleCenter;
            h1.fontSize = 24;
            h1.padding = new RectOffset(0, 0, 10, 10);
            h1.normal.textColor = text;

            //header2
            text = new Color(0.8f, 0.8f, 0.8f);
            h2 = new GUIStyle();
            h2.alignment = TextAnchor.MiddleCenter;
            h2.fontStyle = FontStyle.Bold;
            h2.fontSize = 16;
            h2.padding = new RectOffset(0, 0, 10, 10);
            h2.normal.textColor = text;

            //header3
            text = new Color(0.8f, 0.8f, 0.8f);
            h3 = new GUIStyle();
            h3.alignment = TextAnchor.MiddleCenter;
            h3.fontStyle = FontStyle.Bold;
            h3.fontSize = 12;
            h3.padding = new RectOffset(0, 0, 10, 10);
            h3.normal.textColor = text;


            //boldLabels
            boldLabels = new GUIStyle();
            boldLabels.alignment = TextAnchor.MiddleLeft;
            boldLabels.fontStyle = FontStyle.Bold;
            boldLabels.wordWrap = true;
            boldLabels.margin = new RectOffset(5, 5, 5, 5);
            boldLabels.padding = new RectOffset(5, 5, 5, 5);
            boldLabels.normal.textColor = text;

            //common
            common = new GUIStyle();
            common.alignment = TextAnchor.MiddleLeft;
            common.wordWrap = true;
            common.padding = new RectOffset(5, 5, 2, 2);
            common.margin = new RectOffset(5, 5, 2, 2);
            common.normal.textColor = text;

            setup = true;
        }

        private void DrawHeader()
        {
            GUILayout.Label("Aurora Packer", h1);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("-----------------------------------", boldLabels);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        private void DrawAuroraPacker()
        {
            GUILayout.Label("Aurora A3 Packing", h2);
            GUILayout.Label("This section is for packing textures into an 'Aurora A3' swizzle.", boldLabels);
            GUILayout.Label("Assign a diffuse texture below to auto calculate a more-accurate save path. ('DiffuseName_Aurora_A3_Packed.png')\nOtherwise, it will be saved in the same location as the last provided texture.", common);

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Diffuse (Save Path Calculation) (Optional)", common);
            GUILayout.FlexibleSpace();
            diffuseTex = (Texture2D)EditorGUILayout.ObjectField(diffuseTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Metallic Texture", common);
            GUILayout.FlexibleSpace();
            metallicTex = (Texture2D)EditorGUILayout.ObjectField(metallicTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(80f);
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("Metallic Texture Channel", "Select which channel contains the metallic information. This is typically red for Metallic_Smoothness textures for Unity."));
            metallicChannel = (TextureChannels)EditorGUILayout.EnumPopup(metallicChannel);
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label(surfaceFinishWorkflow.ToString() + " Texture", common);
            GUILayout.FlexibleSpace();
            roughnessTex = (Texture2D)EditorGUILayout.ObjectField(roughnessTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(90f);
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("Surface Finish Workflow", "Select the surface finish texture type. Roughness or Smoothness/Gloss"));
            surfaceFinishWorkflow = (SurfaceFinishWorkflow)EditorGUILayout.EnumPopup(surfaceFinishWorkflow);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Space(80f);
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("   " + surfaceFinishWorkflow.ToString() + " Texture Channel", "Select which channel contains the surface finish information. This is typically red for roughness textures, alpha for Metallic_Smoothness textures."));
            surfaceFinishChannel = (TextureChannels)EditorGUILayout.EnumPopup(surfaceFinishChannel);
            GUILayout.EndHorizontal();


            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Illumination Texture", common);
            GUILayout.FlexibleSpace();
            illuminationTex = (Texture2D)EditorGUILayout.ObjectField(illuminationTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(80f);
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("Illumination Texture Channel", "Select which channel contains the illumination information. The Aurora swizzle (by definition) can only produce white illumination tinted by the color property of the material."));
            illuminationChannel = (TextureChannels)EditorGUILayout.EnumPopup(illuminationChannel);
            GUILayout.EndHorizontal();

            GUILayout.Space(20f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Ambient Occlusion Texture", common);
            GUILayout.FlexibleSpace();
            occlusionTex = (Texture2D)EditorGUILayout.ObjectField(occlusionTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(80f);
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("Occlusion Texture Channel", "Select which channel contains the Ambient Occlusion information. This is typically green for AO textures for Unity."));
            occlusionChannel = (TextureChannels)EditorGUILayout.EnumPopup(occlusionChannel);
            GUILayout.EndHorizontal();

            GUILayout.Space(40f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Pack Aurora A3", GUILayout.Width(150f)))
            {
                if (!PackAurora())
                {
                    EditorUtility.DisplayDialog("Failed", "Either no textures were selected, or one of the source textures is not of the same resolution as the others. Check the console for more details.", "Shucks.");
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        private void DrawNormalConversion()
        {
            GUILayout.Label("OpenGL to DirectX", h2);
            GUILayout.Label("Normal Conversion", h3);
            GUILayout.Label("This section is for flipping the Y (Green) channel of the normal map and saving as a new texture.", boldLabels);

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Normal Map", common);
            GUILayout.FlexibleSpace();
            normalMap = (Texture2D)EditorGUILayout.ObjectField(normalMap, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(80f);
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent("Current Format", "Select which format your texture currently is, and a new normal map of the opposite format will be generated."));
            normalMapFormat = (NormalMapFormat)EditorGUILayout.EnumPopup(normalMapFormat);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            int oppositeNormalFormat = ((int)normalMapFormat + 1) % 2;
            if (GUILayout.Button("Create " + ((NormalMapFormat)oppositeNormalFormat).ToString() + " Normal Map", GUILayout.Width(250f)))
            {
                if (!ConvertNormal(oppositeNormalFormat))
                {
                    EditorUtility.DisplayDialog("Failed to convert Normal Map", "Check the console for more details.", "Shucks.");
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        private bool PackAurora()
        {
            int averageSize = 0;
            int count = 0;
            Vector2Int knownSize = Vector2Int.zero;
            Color[][] texturePixels = new Color[System.Enum.GetNames(typeof(AuroraA3SwizzleTextures)).Length][];

            string savePath = "";

            if (metallicTex)
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(metallicTex);
                    savePath = assetPath;
                    TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                    tempImporter.isReadable = true;
                    tempImporter.crunchedCompression = false;
                    tempImporter.SaveAndReimport();
                }
                catch(Exception e)
                {
                    Log("Error 0m - " + e.Message);
                }

                knownSize = GetResolution(metallicTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                texturePixels[(int)AuroraA3SwizzleTextures.Metallic] = metallicTex.GetPixels();
            }
            if (roughnessTex)
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(roughnessTex);
                    savePath = assetPath;
                    TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                    tempImporter.isReadable = true;
                    tempImporter.crunchedCompression = false;
                    tempImporter.SaveAndReimport();
                }
                catch (Exception e)
                {
                    Log("Error 0r - " + e.Message);
                }

                knownSize = GetResolution(roughnessTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                texturePixels[(int)AuroraA3SwizzleTextures.Roughness] = roughnessTex.GetPixels();
            }
            if (illuminationTex)
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(illuminationTex);
                    savePath = assetPath;
                    TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                    tempImporter.isReadable = true;
                    tempImporter.crunchedCompression = false;
                    tempImporter.SaveAndReimport();
                }
                catch (Exception e)
                {
                    Log("Error 0i - " + e.Message);
                }

                knownSize = GetResolution(illuminationTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                texturePixels[(int)AuroraA3SwizzleTextures.Illumination] = illuminationTex.GetPixels();
            }
            if (occlusionTex)
            {
                
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(occlusionTex);
                    savePath = assetPath;
                    TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                    tempImporter.isReadable = true;
                    tempImporter.crunchedCompression = false;
                    tempImporter.SaveAndReimport();
                }
                catch (Exception e)
                {
                    Log("Error 0o - " + e.Message);
                }

                knownSize = GetResolution(occlusionTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                texturePixels[(int)AuroraA3SwizzleTextures.Occlusion] = occlusionTex.GetPixels();
            }

            //No textures were assigned
            if (count < 1)
            {
                Log("Error 1 - No textures assigned. You must assign at least one texture to pack an Aurora A3 swizzle texture.");
                return false;
            }

            averageSize /= count;

            //There was a discrepancy between texture resolutions
            if (averageSize != (int)(knownSize.x + knownSize.y))
            {
                Log("Error 2 - All textures must be the same resolution. (This might be fixed in the texture import settings)");
                return false;
            }

            Texture2D auroraTexture = new Texture2D(knownSize.x, knownSize.y);
            Color[] auroraPixels = new Color[GetSize(auroraTexture)];

            for (int i = 0; i < auroraPixels.Length; i++)
            {
                //Metallic
                if (texturePixels[(int)AuroraA3SwizzleTextures.Metallic] != null)
                {
                    float finishData = 0f;
                    switch (metallicChannel)
                    {
                        case TextureChannels.Red:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Metallic][i].r;
                            break;
                        case TextureChannels.Green:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Metallic][i].g;
                            break;
                        case TextureChannels.Blue:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Metallic][i].b;
                            break;
                        case TextureChannels.Alpha:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Metallic][i].a;
                            break;
                    }
                    auroraPixels[i].r = finishData;
                }
                else
                {
                    auroraPixels[i].r = 1.0f;
                }

                //Roughness
                if (texturePixels[(int)AuroraA3SwizzleTextures.Roughness] != null)
                {
                    float finishData = 0f;
                    switch (surfaceFinishChannel)
                    {
                        case TextureChannels.Red:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Roughness][i].r;
                            break;
                        case TextureChannels.Green:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Roughness][i].g;
                            break;
                        case TextureChannels.Blue:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Roughness][i].b;
                            break;
                        case TextureChannels.Alpha:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Roughness][i].a;
                            break;
                    }
                    if(surfaceFinishWorkflow != SurfaceFinishWorkflow.Roughness)
                    {
                        finishData = 1.0f - finishData;
                    }
                    auroraPixels[i].g = finishData;
                }
                else
                {
                    auroraPixels[i].g = 0.0f;
                }

                //Illumination
                if (texturePixels[(int)AuroraA3SwizzleTextures.Illumination] != null)
                {
                    float finishData = 0f;
                    switch (illuminationChannel)
                    {
                        case TextureChannels.Red:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Illumination][i].r;
                            break;
                        case TextureChannels.Green:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Illumination][i].g;
                            break;
                        case TextureChannels.Blue:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Illumination][i].b;
                            break;
                        case TextureChannels.Alpha:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Illumination][i].a;
                            break;
                    }
                    auroraPixels[i].b = finishData;
                }
                else
                {
                    auroraPixels[i].b = 0.0f;
                }

                //Occlusion
                if (texturePixels[(int)AuroraA3SwizzleTextures.Occlusion] != null)
                {
                    float finishData = 0f;
                    switch (occlusionChannel)
                    {
                        case TextureChannels.Red:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Occlusion][i].r;
                            break;
                        case TextureChannels.Green:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Occlusion][i].g;
                            break;
                        case TextureChannels.Blue:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Occlusion][i].b;
                            break;
                        case TextureChannels.Alpha:
                            finishData = texturePixels[(int)AuroraA3SwizzleTextures.Occlusion][i].a;
                            break;
                    }
                    auroraPixels[i].a = finishData;
                }
                else
                {
                    auroraPixels[i].a = 1.0f;
                }
            }

            auroraTexture.SetPixels(auroraPixels);
            auroraTexture.Apply();

            TextureImporter importer;
            try
            {
                savePath = diffuseTex != null ? AssetDatabase.GetAssetPath(diffuseTex) : savePath;
                savePath = savePath.Replace("_Albedo", "").Replace("_AlbedoTransparency", "").Replace("_Albedo_Transparency", "").Replace("_Diffuse", "");
                savePath = savePath.Replace(".png", "").Replace(".tif", "").Replace(".bmp", "").Replace(".jpg", "").Replace(".jpeg", "").Replace(".dds", "");

                File.WriteAllBytes(savePath + "_Aurora_A3_Packed.png", auroraTexture.EncodeToPNG());
                AssetDatabase.Refresh();

                importer = (TextureImporter)AssetImporter.GetAtPath(savePath + "_Aurora_A3_Packed.png");
                importer.crunchedCompression = true;
                importer.SaveAndReimport();
            }
            catch(Exception e)
            {
                Log("Error 3a - " + e.Message);
                return false;
            }

            if (metallicTex)
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(metallicTex);
                    importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                    importer.isReadable = true;
                    importer.crunchedCompression = true;
                    importer.SaveAndReimport();
                }
                catch(Exception e)
                {
                    Log("Error 3m - " + e.Message);
                }
            }
            if (roughnessTex)
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(roughnessTex);
                    importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                    importer.isReadable = true;
                    importer.crunchedCompression = true;
                    importer.SaveAndReimport();
                }
                catch (Exception e)
                {
                    Log("Error 3r - " + e.Message);
                }
            }
            if (illuminationTex)
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(illuminationTex);
                    importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                    importer.isReadable = true;
                    importer.crunchedCompression = true;
                    importer.SaveAndReimport();
                }
                catch (Exception e)
                {
                    Log("Error 3i - " + e.Message);
                }
            }
            if (occlusionTex)
            {
                try
                {
                    string assetPath = AssetDatabase.GetAssetPath(occlusionTex);
                    importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                    importer.isReadable = true;
                    importer.crunchedCompression = true;
                    importer.SaveAndReimport();
                }
                catch (Exception e)
                {
                    Log("Error 3o - " + e.Message);
                }
            }

            AssetDatabase.Refresh();

            return true;
        }

        private bool ConvertNormal(int desiredFormat)
        {
            string assetPath = AssetDatabase.GetAssetPath(normalMap);
            string savePath = assetPath.Replace(normalMap.name, "").Replace(".png", "").Replace(".jpg", "").Replace(".jpeg", "").Replace(".bmp", "").Replace(".tif", "").Replace(".dds", "").Replace(".tga", "");
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(assetPath);
            importer.textureType = TextureImporterType.Default;
            if (importer.crunchedCompression)
            {
                importer.crunchedCompression = false;
            }
            importer.isReadable = true;
            importer.SaveAndReimport();

            Texture2D newNormal = new Texture2D(normalMap.width, normalMap.height);

            if (desiredFormat == 0)
            {
                newNormal.name = normalMap.name.Replace("_DirectX", "") + "_OpenGL";
            }
            else
            {
                newNormal.name = normalMap.name.Replace("_OpenGL", "") + "_DirectX";
            }

            Color[] normalPixels = normalMap.GetPixels();
            Color[] convertedPixels = new Color[normalMap.height * normalMap.width];

            for (int i = 0; i < normalPixels.Length; i++)
            {
                convertedPixels[i] = new Color(normalPixels[i].r, 1.0f - normalPixels[i].g, normalPixels[i].b);
            }

            newNormal.SetPixels(convertedPixels);
            newNormal.Apply();

            byte[] normalBytes = newNormal.EncodeToPNG();

            try
            {
                //Converted Normal
                FileStream normalStream = new FileStream(savePath + newNormal.name + ".png", FileMode.Create);
                BinaryWriter normalWriter = new BinaryWriter(normalStream);
                normalWriter.Write(normalBytes);
                normalWriter.Close();

                AssetDatabase.Refresh();

                TextureImporter newNormalImporter = (TextureImporter)AssetImporter.GetAtPath(savePath + newNormal.name + ".png");
                newNormalImporter.streamingMipmaps = true;
                newNormalImporter.textureType = TextureImporterType.NormalMap;
                newNormalImporter.crunchedCompression = true;
                newNormalImporter.SaveAndReimport();
            }
            catch (Exception e)
            {
                Log("NormalConversion - " + e.Message);
                return false;
            }

            if (importer != null)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.crunchedCompression = true;
                importer.isReadable = false;
                importer.SaveAndReimport();
            }

            return true;
        }

        private int GetSize(Texture2D texture)
        {
            return texture.width * texture.height;
        }

        private Vector2Int GetResolution(Texture2D texture)
        {
            return new Vector2Int(texture.width, texture.height);
        }

        private void Log(string message)
        {
            Debug.LogError("Aurora Packer - " + message);
        }
    }
}
