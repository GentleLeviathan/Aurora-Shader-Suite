using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace GentleShaders.Aurora.Helpers
{
    /// <summary>
    /// Aurora Shader included helper. Packs metallic, occlusion, illumination, curvature textures into an Aurora swizzle texture.
    /// WIP!
    /// </summary>
    public class AuroraPacker : EditorWindow
    {
        private static AuroraPacker window;
        private bool setup = false;

        private bool smoothnessTexture;
        private bool alphaChannelSmoothness;

        private GUIStyle h1;
        private GUIStyle h2;
        private GUIStyle boldLabels;
        private GUIStyle common;

        private Texture2D diffuseTex;
        private Texture2D roughnessTex;

        private Texture2D metallicTex;
        private Texture2D illuminationTex;
        private Texture2D occlusionTex;
        private Texture2D curvatureTex;

        public static void Init()
        {
            window = (AuroraPacker)EditorWindow.GetWindow(typeof(AuroraPacker));
            window.titleContent = new GUIContent("Finality - Packer");
            window.minSize = new Vector2(450f, 600f);
            window.Show();
        }

        private void OnGUI()
        {
            if (!setup) { SetupStyles(); }

            DrawHeader();
            DrawDiffuseRoughness();
            DrawEtc();
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
            GUILayout.Label("Finality Aurora Packer", h1);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField("-----------------------------------", boldLabels);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        private void DrawDiffuseRoughness()
        {
            GUILayout.Label("Diffuse + Roughness", h2);
            GUILayout.Label("This section is for packing roughness into your diffuse's alpha channel. It may be skipped if that is already the case.", boldLabels);

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Diffuse", common);
            GUILayout.FlexibleSpace();
            diffuseTex = (Texture2D)EditorGUILayout.ObjectField(diffuseTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Roughness/Smoothness", common);
            GUILayout.FlexibleSpace();
            roughnessTex = (Texture2D)EditorGUILayout.ObjectField(roughnessTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(2f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(100f);
            smoothnessTexture = GUILayout.Toggle(smoothnessTexture, new GUIContent("   Is Smoothness?", "Select this option if your asset's workflow utilizes Smoothness."));
            GUILayout.FlexibleSpace();
            alphaChannelSmoothness = GUILayout.Toggle(alphaChannelSmoothness, new GUIContent("   Read from alpha channel?", "Select this option if the provided texture " +
                "has the roughness/smoothness in the alpha channel. This is common with 'Metallic_Smoothness' textures."));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Pack!", GUILayout.Width(150f)))
            {
                if (!PackDiffuse())
                {
                    EditorUtility.DisplayDialog("Failed", "Either a diffuse or roughness/smoothness texture was not selected, or they are not the same resolution.", "Shucks.");
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        private void DrawEtc()
        {
            GUILayout.Label("Aurora Packing", h2);
            GUILayout.Label("This section is for packing textures into an 'Aurora' swizzle.", boldLabels);
            GUILayout.Label("Assign a diffuse texture above to auto calculate a more-accurate save path. ('DiffuseName_Aurora.png')", common);

            GUILayout.Space(6f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Metallic", common);
            GUILayout.FlexibleSpace();
            metallicTex = (Texture2D)EditorGUILayout.ObjectField(metallicTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Occlusion", common);
            GUILayout.FlexibleSpace();
            occlusionTex = (Texture2D)EditorGUILayout.ObjectField(occlusionTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Illumination", common);
            GUILayout.FlexibleSpace();
            illuminationTex = (Texture2D)EditorGUILayout.ObjectField(illuminationTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(4f);

            GUILayout.BeginHorizontal();
            GUILayout.Space(40f);
            GUILayout.Label("Curvature", common);
            GUILayout.FlexibleSpace();
            curvatureTex = (Texture2D)EditorGUILayout.ObjectField(curvatureTex, typeof(Texture2D), false);
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Pack!", GUILayout.Width(150f)))
            {
                if (!PackEtc())
                {
                    EditorUtility.DisplayDialog("Failed", "Either no textures were selected, or one of the source textures is not of the same resolution as the others. All textures must be the same resolution. (This can be fixed in their import settings)", "Shucks.");
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.Space(10f);
        }

        private bool PackDiffuse()
        {
            if (!diffuseTex || !roughnessTex) { return false; }
            if ((diffuseTex.width + diffuseTex.height) != (roughnessTex.width + roughnessTex.height)) { return false; }

            string savePath = AssetDatabase.GetAssetPath(diffuseTex);

            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(savePath);
            importer.isReadable = true;
            importer.crunchedCompression = false;
            importer.SaveAndReimport();

            Texture2D packedDiffuse = new Texture2D(diffuseTex.width, diffuseTex.height);
            Color[] diffuseColors = diffuseTex.GetPixels();
            Color[] roughnessColors = roughnessTex.GetPixels();

            importer.crunchedCompression = true;
            importer.SaveAndReimport();

            for (int i = 0; i < diffuseColors.Length; i++)
            {
                if (smoothnessTexture)
                {
                    if (alphaChannelSmoothness)
                    {
                        diffuseColors[i].a = 1.0f - roughnessColors[i].a;
                    }
                    else
                    {
                        diffuseColors[i].a = 1.0f - roughnessColors[i].r;
                    }
                }
                else
                {
                    if (alphaChannelSmoothness)
                    {
                        diffuseColors[i].a = roughnessColors[i].a;
                    }
                    else
                    {
                        diffuseColors[i].a = roughnessColors[i].r;
                    }
                }
            }

            packedDiffuse.SetPixels(diffuseColors);

            File.WriteAllBytes(savePath + "_Roughness_A", packedDiffuse.EncodeToPNG());
            AssetDatabase.Refresh();

            importer = (TextureImporter)TextureImporter.GetAtPath(savePath + "_Roughness_A");
            importer.crunchedCompression = true;
            importer.SaveAndReimport();
            AssetDatabase.Refresh();

            return true;
        }

        private bool PackEtc()
        {
            int averageSize = 0;
            int count = 0;
            Vector2Int knownSize = Vector2Int.zero;
            Dictionary<string, Color[]> pixels = new Dictionary<string, Color[]>();

            string savePath = "";

            if (metallicTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(metallicTex);
                savePath = assetPath;
                TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                tempImporter.isReadable = true;
                tempImporter.crunchedCompression = false;
                tempImporter.SaveAndReimport();

                knownSize = GetResolution(metallicTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                pixels.Add("metallic", metallicTex.GetPixels());
            }
            if (curvatureTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(curvatureTex);
                savePath = assetPath;
                TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                tempImporter.isReadable = true;
                tempImporter.crunchedCompression = false;
                tempImporter.SaveAndReimport();

                knownSize = GetResolution(curvatureTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                pixels.Add("curvature", curvatureTex.GetPixels());
            }
            if (illuminationTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(illuminationTex);
                savePath = assetPath;
                TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                tempImporter.isReadable = true;
                tempImporter.crunchedCompression = false;
                tempImporter.SaveAndReimport();

                knownSize = GetResolution(illuminationTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                pixels.Add("illumination", illuminationTex.GetPixels());
            }
            if (occlusionTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(occlusionTex);
                savePath = assetPath;
                TextureImporter tempImporter = (TextureImporter)TextureImporter.GetAtPath(assetPath);
                tempImporter.isReadable = true;
                tempImporter.crunchedCompression = false;
                tempImporter.SaveAndReimport();

                knownSize = GetResolution(occlusionTex);
                averageSize += (int)(knownSize.x + knownSize.y);
                count++;

                pixels.Add("occlusion", occlusionTex.GetPixels());
            }

            //No textures were assigned
            if(count < 1)
            {
                return false;
            }

            averageSize /= count;

            //There was a discrepancy between texture resolutions
            if (averageSize != (int)(knownSize.x + knownSize.y))
            {
                return false;
            }

            Texture2D auroraTexture = new Texture2D(knownSize.x, knownSize.y);
            Color[] auroraPixels = new Color[GetSize(auroraTexture)];

            for(int i = 0; i < auroraPixels.Length; i++)
            {
                //Metallic
                if (pixels.ContainsKey("metallic"))
                {
                    auroraPixels[i].r = pixels["metallic"][i].r;
                }
                else
                {
                    auroraPixels[i].r = 0f;
                }

                //Curvature
                if (pixels.ContainsKey("curvature"))
                {
                    auroraPixels[i].g = pixels["curvature"][i].r;
                }
                else
                {
                    auroraPixels[i].g = 0f;
                }

                //Illumination
                if (pixels.ContainsKey("illumination"))
                {
                    auroraPixels[i].b = pixels["illumination"][i].r;
                }
                else
                {
                    auroraPixels[i].b = 0f;
                }

                //Occlusion
                if (pixels.ContainsKey("occlusion"))
                {
                    auroraPixels[i].a = pixels["occlusion"][i].r;
                }
                else
                {
                    auroraPixels[i].a = 1.0f;
                }
            }

            auroraTexture.SetPixels(auroraPixels);
            auroraTexture.Apply();

            savePath = diffuseTex != null ? AssetDatabase.GetAssetPath(diffuseTex) : savePath;
            savePath = savePath.Replace(".png", "").Replace(".tif", "").Replace(".bmp", "").Replace(".jpg", "").Replace(".jpeg", "").Replace(".dds", "");

            File.WriteAllBytes(savePath + "_Aurora.png", auroraTexture.EncodeToPNG());
            AssetDatabase.Refresh();

            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(savePath + "_Aurora.png");
            importer.crunchedCompression = true;
            importer.SaveAndReimport();

            if (metallicTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(metallicTex);
                importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.isReadable = true;
                importer.crunchedCompression = true;
                importer.SaveAndReimport();
            }

            if (curvatureTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(curvatureTex);
                importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.isReadable = true;
                importer.crunchedCompression = true;
                importer.SaveAndReimport();
            }
            if (illuminationTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(illuminationTex);
                importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.isReadable = true;
                importer.crunchedCompression = true;
                importer.SaveAndReimport();
            }
            if (occlusionTex)
            {
                string assetPath = AssetDatabase.GetAssetPath(occlusionTex);
                importer = (TextureImporter)AssetImporter.GetAtPath(assetPath);
                importer.isReadable = true;
                importer.crunchedCompression = true;
                importer.SaveAndReimport();
            }

            AssetDatabase.Refresh();

            return true;
        }

        private int GetResolutionInt(Texture2D texture)
        {
            return texture.width + texture.height;
        }

        private int GetSize(Texture2D texture)
        {
            return texture.width * texture.height;
        }

        private Vector2Int GetResolution(Texture2D texture)
        {
            return new Vector2Int(texture.width, texture.height);
        }
    }
}
