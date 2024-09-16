using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GentleShaders.Aurora.Helpers
{
    /// <summary>
    /// Aurora Shader included helper. "Decomposes" Halo 4 'Storm' textures into a format that is more like the Halo 5 Forge textures.
    /// WIP!
    /// </summary>
    public static class StormDecomposer
    {
        public static void DecomposeStormTexture(Texture2D stormTexture, UnityEngine.Object asset)
        {
            string savePath = AssetDatabase.GetAssetPath(asset).Replace(asset.name, "").Replace(".png", "").Replace(".jpg", "").Replace(".bmp", "").Replace(".tif", "").Replace(".dds", "").Replace(".jpeg", "").Replace(".tga", "") + "Decomposed";
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(asset));
            importer.isReadable = true;
            if (importer.crunchedCompression)
            {
                importer.crunchedCompression = false;
                importer.SaveAndReimport();
                Debug.Log("StormDecomposer: Texture was crunched, passing importer");
                Decompose(stormTexture, savePath, importer);
            }
            else
            {
                importer.SaveAndReimport();
                Decompose(stormTexture, savePath);
            }
        }

        private static void Decompose(Texture2D stormTexture, string savePath, TextureImporter importer = null)
        {
            Debug.Log("StormDecomposer: Beginning Decomposition... Texture Name: " + stormTexture.name);

            Texture2D diffuse = new Texture2D(stormTexture.width, stormTexture.height);
            Texture2D cc = new Texture2D(stormTexture.width, stormTexture.height);
            Texture2D specularMetallic = new Texture2D(stormTexture.width, stormTexture.height);

            diffuse.name = stormTexture.name + "_Diffuse_RGB_Smoothness_A";
            specularMetallic.name = stormTexture.name + "Specular_RGB_Roughness_A";
            cc.name = stormTexture.name + "_CC_RG_Specular_B_Roughness_A";

            Debug.LogWarning("StormDecomposer: Setup complete, beginning copy process..");

            //Get and create pixel array's
            Color[] stormPixels = stormTexture.GetPixels();
            Color[] diffusePixels = new Color[stormTexture.height * stormTexture.width];
            Color[] ccPixels = new Color[stormTexture.height * stormTexture.width];
            Color[] specularPixels = new Color[stormTexture.height * stormTexture.width];

            for (int i = 0; i < stormPixels.Length; i++)
            {
                Color stormColor = stormPixels[i];
                Color texture = new Color(stormColor.g, stormColor.g, stormColor.g, Mathf.Abs(1 - stormColor.r));

                diffusePixels[i] = texture;

                //Red channel
                if (stormColor.a > 0 && stormColor.a < 0.99)
                {
                    ccPixels[i] = Color.green;
                }
                //Green channel
                else if (stormColor.a > 0.99)
                {
                    ccPixels[i] = Color.red;
                }
                //Passthrough (black)
                else
                {
                    ccPixels[i] = Color.black;
                }

                //halo 5 shader compatibility
                ccPixels[i].b = stormColor.b;
                ccPixels[i].a = stormColor.r;

                specularPixels[i].r = stormColor.b; specularPixels[i].g = stormColor.b; specularPixels[i].b = stormColor.b;
                specularPixels[i].a = stormColor.r;
            }

            //Set Pixels
            diffuse.SetPixels(diffusePixels);
            cc.SetPixels(ccPixels);
            specularMetallic.SetPixels(specularPixels);

            //Apply to textures
            diffuse.Apply();
            cc.Apply();
            specularMetallic.Apply();

            Debug.Log("StormDecomposer: Copy complete, beginning write process...");

            byte[] diffuseBytes = diffuse.EncodeToPNG();
            byte[] ccBytes = cc.EncodeToPNG();
            byte[] srBytes = specularMetallic.EncodeToPNG();

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
                //SpecularRoughness
                FileStream srStream = new FileStream(savePath + "/" + specularMetallic.name + ".png", FileMode.Create);
                BinaryWriter srWriter = new BinaryWriter(srStream);
                srWriter.Write(srBytes);
                srWriter.Close();


                AssetDatabase.Refresh();

                //diffuse
                TextureImporter importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + diffuse.name + ".png");
                importedTextureCorrector.streamingMipmaps = true;
                importedTextureCorrector.crunchedCompression = true;
                importedTextureCorrector.SaveAndReimport();
                //cc
                importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + cc.name + ".png");
                importedTextureCorrector.streamingMipmaps = true;
                importedTextureCorrector.crunchedCompression = true;
                importedTextureCorrector.SaveAndReimport();
                //specular
                importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + specularMetallic.name + ".png");
                importedTextureCorrector.streamingMipmaps = true;
                importedTextureCorrector.crunchedCompression = true;
                importedTextureCorrector.SaveAndReimport();
            }
            catch (Exception e)
            {
                //TODO: Change exception type and display a dialog with common solutions
                Debug.LogError(e.Message);

                Debug.LogError("StormDecomposer: Decomposition failed. Please see above message for more information.");
                EditorUtility.DisplayDialog("StormDecomposer", "Decomposition failed. Please check the console for more information.", ":(");
                return;
            }

            if (importer != null)
            {
                importer.crunchedCompression = true;
                importer.isReadable = false;
                importer.SaveAndReimport();
            }

            Debug.Log("StormDecomposer: Success! Decomposed textures are now available in the '" + savePath.Replace(Application.dataPath, "Assets/" + "' folder."));
            EditorUtility.DisplayDialog("StormDecomposer", "Success! Decomposed textures are now available in the '" + savePath.Replace(Application.dataPath, "Assets/") + "' folder.", "Got it");
            AssetDatabase.Refresh();
        }
    }
}
