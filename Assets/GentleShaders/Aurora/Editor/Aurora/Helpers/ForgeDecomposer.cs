using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace GentleShaders.Aurora.Helpers
{
    /// <summary>
    /// Aurora Shader included helper. "Decomposes" Halo 5 Forge textures into a format that is more like the Bungie-era game textures.
    /// WIP!
    /// </summary>
    public static class ForgeDecomposer
    {
        public static Texture2D[] DecomposeForgeTextures(UnityEngine.Object assetDiffuse, UnityEngine.Object assetCC)
        {
            string savePath = AssetDatabase.GetAssetPath(assetDiffuse).Replace(assetDiffuse.name, "").Replace(".png", "").Replace(".jpg", "").Replace(".bmp", "").Replace(".tif", "").Replace(".dds", "").Replace(".jpeg", "").Replace(".tga", "") + "Decomposed";
            //Diffuse
            TextureImporter diffuseImporter = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(assetDiffuse));
            diffuseImporter.isReadable = true;
            //CC
            TextureImporter ccImporter = (TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(assetCC));
            ccImporter.isReadable = true;
            if (diffuseImporter.crunchedCompression || ccImporter.crunchedCompression)
            {
                diffuseImporter.crunchedCompression = false;
                ccImporter.crunchedCompression = false;
                diffuseImporter.SaveAndReimport();
                ccImporter.SaveAndReimport();
                Debug.Log("ForgeDecomposer: Textures were crunched, passing importers");
                TextureImporter[] importers = new TextureImporter[2];
                importers[0] = diffuseImporter; importers[1] = ccImporter;

                return Decompose(assetDiffuse as Texture2D, assetCC as Texture2D, savePath, importers, true);
            }
            else
            {
                diffuseImporter.SaveAndReimport();
                ccImporter.SaveAndReimport();
                TextureImporter[] importers = new TextureImporter[2];
                importers[0] = diffuseImporter; importers[1] = ccImporter;
                return Decompose(assetDiffuse as Texture2D, assetCC as Texture2D, savePath, importers, false);
            }
        }

        private static Texture2D[] Decompose(Texture2D forgeTexture, Texture2D forgeCC, string savePath, TextureImporter[] importers = null, bool autoCrunch = true)
        {
            Texture2D[] textures = new Texture2D[3];
            string textureSetName = forgeTexture.name.Replace("_colour", "");
            Debug.Log("ForgeDecomposer: Beginning Decomposition... Texture Set Name: " + textureSetName);

            Texture2D diffuse = new Texture2D(forgeTexture.width, forgeTexture.height);
            Texture2D cc = new Texture2D(forgeCC.width, forgeCC.height);
            Texture2D metallic = new Texture2D(forgeCC.width, forgeCC.height);

            diffuse.name = textureSetName + "_Diffuse_RGB_Roughness_A";
            cc.name = textureSetName + "_CC_RG";
            metallic.name = textureSetName + "_Metallic_Smoothness";

            Debug.LogWarning("ForgeDecomposer: Setup complete, beginning copy process..");

            //Get and create pixel array's
            Color[] forgePixels = forgeTexture.GetPixels();
            Color[] forgeCCPixels = forgeCC.GetPixels();
            Color[] diffusePixels = new Color[forgeTexture.height * forgeTexture.width];
            Color[] ccPixels = new Color[forgeCC.height * forgeCC.width];
            Color[] metallicPixels = new Color[forgeCC.height * forgeCC.width];

            if (forgeCC.width + forgeCC.height == diffuse.width + diffuse.height)
            {
                for (int i = 0; i < forgePixels.Length; i++)
                {
                    Color forgeColor = forgePixels[i];
                    Color forgeCCColor = forgeCCPixels[i];
                    Color texture = new Color(forgeColor.r, forgeColor.g, forgeColor.b, forgeCCColor.a);
                    Color metallicColor = new Color(1.0f - forgeCCColor.b, 1.0f - forgeCCColor.b, 1.0f - forgeCCColor.b, forgeCCColor.a);

                    metallicPixels[i] = metallicColor;

                    diffusePixels[i] = texture;

                    //Passthrough (black)
                    ccPixels[i] = Color.black;
                    //Red channel
                    ccPixels[i].r = forgeCCColor.r;
                    //Green channel
                    ccPixels[i].g = forgeCCColor.g;

                    //Artifact correction
                    if (forgeCCColor.r > 0.15)
                    {
                        ccPixels[i] = Color.red;
                    }
                    if (forgeCCColor.g > 0.3 && forgeCCColor.r < 0.15)
                    {
                        ccPixels[i] = Color.green;
                    }
                }

                //Set Pixels
                diffuse.SetPixels(diffusePixels);
                cc.SetPixels(ccPixels);
                metallic.SetPixels(metallicPixels);

                //Apply to textures
                diffuse.Apply();
                cc.Apply();
                metallic.Apply();

                Debug.Log("ForgeDecomposer: Copy complete, beginning write process...");

                byte[] diffuseBytes = diffuse.EncodeToPNG();
                byte[] ccBytes = cc.EncodeToPNG();
                byte[] metallicBytes = metallic.EncodeToPNG();

                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                try
                {
                    //Diffuse
                    FileStream diffuseStream = new FileStream(savePath + "/" + diffuse.name + ".png", FileMode.Create);
                    BinaryWriter diffuseWriter = new BinaryWriter(diffuseStream);
                    diffuseWriter.Write(diffuseBytes);
                    diffuseWriter.Close();
                    //CC
                    FileStream ccStream = new FileStream(savePath + "/" + cc.name + ".png", FileMode.Create);
                    BinaryWriter ccWriter = new BinaryWriter(ccStream);
                    ccWriter.Write(ccBytes);
                    ccWriter.Close();
                    //Metallic
                    FileStream metallicStream = new FileStream(savePath + "/" + metallic.name + ".png", FileMode.Create);
                    BinaryWriter metallicWriter = new BinaryWriter(metallicStream);
                    metallicWriter.Write(metallicBytes);
                    metallicWriter.Close();

                    AssetDatabase.Refresh();

                    //diffuse
                    TextureImporter importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + diffuse.name + ".png");
                    importedTextureCorrector.streamingMipmaps = true;
                    importedTextureCorrector.crunchedCompression = true;
                    importedTextureCorrector.SaveAndReimport();
                    textures[0] = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath + "/" + diffuse.name + ".png");
                    //cc
                    importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + cc.name + ".png");
                    importedTextureCorrector.streamingMipmaps = true;
                    importedTextureCorrector.crunchedCompression = true;
                    importedTextureCorrector.SaveAndReimport();
                    textures[1] = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath + "/" + cc.name + ".png");
                    //metallic
                    importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + metallic.name + ".png");
                    importedTextureCorrector.streamingMipmaps = true;
                    importedTextureCorrector.crunchedCompression = true;
                    importedTextureCorrector.SaveAndReimport();
                    textures[2] = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath + "/" + metallic.name + ".png");
                }
                catch (Exception e)
                {
                    //TODO: Change exception type and display a dialog with common solutions
                    Debug.LogError(e.Message);

                    Debug.LogError("ForgeDecomposer: Decomposition failed. Please see above message for more information.");
                    EditorUtility.DisplayDialog("ForgeDecomposer", "Decomposition failed. Please check the console for more information.", ":(");
                    return null;
                }

                if (importers != null && autoCrunch)
                {
                    for (int i = 0; i < importers.Length; i++)
                    {
                        importers[i].streamingMipmaps = true;
                        importers[i].crunchedCompression = true;
                        importers[i].SaveAndReimport();
                    }
                }

                Debug.Log("ForgeDecomposer: Success! Decomposed textures are now available in the '" + savePath.Replace(Application.dataPath, "Assets/") + "' folder, and have been applied to your material.");
                EditorUtility.DisplayDialog("ForgeDecomposer", "Success! Decomposed textures are now available in the '" + savePath.Replace(Application.dataPath, "Assets/") + "' folder, and have been applied to your material.", "Got it");
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("ForgeDecomposer: Decomposition failed because source textures (_colour 'diffuse', and _control 'cc' textures) are not the same resolution. This decomposer is intended for unmodified Halo 5 Forge texture rips.");
                EditorUtility.DisplayDialog("ForgeDecomposer", "Decomposition failed. Please check the console for more information.", ":(");
            }
            return textures;
        }
    }
}
