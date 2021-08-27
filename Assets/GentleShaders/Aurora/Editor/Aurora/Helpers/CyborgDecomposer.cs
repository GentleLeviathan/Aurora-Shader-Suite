using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GentleShaders.Aurora.Helpers
{
    /// <summary>
    /// Aurora Shader included helper. "Decomposes" Halo CE 'Multipurpose' textures into a format that is more like the Bungie-era textures.
    /// WIP!
    /// </summary>
    public static class CyborgDecomposer
    {
        public static void DecomposeMultipurposeTexture(Texture2D mpTexture, UnityEngine.Object asset)
        {
            string savePath = AssetDatabase.GetAssetPath(asset).Replace(asset.name, "").Replace(".png", "").Replace(".jpg", "").Replace(".bmp", "").Replace(".tif", "").Replace(".dds", "").Replace(".jpeg", "").Replace(".tga", "") + "Decomposed";
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(asset));
            importer.isReadable = true;
            if (importer.crunchedCompression)
            {
                importer.crunchedCompression = false;
                importer.SaveAndReimport();
                Debug.Log("CyborgDecomposer: Texture was crunched, passing importer");
                Decompose(mpTexture, savePath, importer);
            }
            else
            {
                importer.SaveAndReimport();
                Decompose(mpTexture, savePath);
            }
        }

        private static void Decompose(Texture2D mpTexture, string savePath, TextureImporter importer = null)
        {
            Debug.Log("CyborgDecomposer: Beginning Decomposition... Texture Name: " + mpTexture.name);
            string textureName = mpTexture.name.Replace(" multipurpose", "");

            Texture2D illumination = new Texture2D(mpTexture.width, mpTexture.height);
            Texture2D cc = new Texture2D(mpTexture.width, mpTexture.height);
            Texture2D metallic = new Texture2D(mpTexture.width, mpTexture.height);
            Texture2D roughness = new Texture2D(mpTexture.width, mpTexture.height);

            illumination.name = textureName + "_Illumination";
            metallic.name = textureName + "_MetallicSmoothness";
            cc.name = textureName + "_CC";
            roughness.name = textureName + "_Roughness";

            Debug.LogWarning("CyborgDecomposer: Setup complete, beginning copy process..");

            //Get and create pixel array's
            Color[] mpPixels = mpTexture.GetPixels();
            Color[] illuminationPixels = new Color[mpTexture.height * mpTexture.width];
            Color[] ccPixels = new Color[mpTexture.height * mpTexture.width];
            Color[] metallicPixels = new Color[mpTexture.height * mpTexture.width];
            Color[] roughnessPixels = new Color[mpTexture.height * mpTexture.width];

            for (int i = 0; i < mpPixels.Length; i++)
            {
                Color mpColor = mpPixels[i];

                illuminationPixels[i] = new Color(mpColor.g, mpColor.g, mpColor.g, 1f);

                //Red channel
                if (mpColor.a > 0.005f)
                {
                    ccPixels[i] = Color.red;
                }
                //Passthrough (black)
                else
                {
                    ccPixels[i] = Color.black;
                }

                metallicPixels[i].r = mpColor.a;
                metallicPixels[i].g = mpColor.a;
                metallicPixels[i].b = mpColor.a;
                metallicPixels[i].a = mpColor.b;

                roughnessPixels[i] = new Color(1.0f - mpColor.b, 1.0f - mpColor.b, 1.0f - mpColor.b, 1);
            }

            //Set Pixels
            illumination.SetPixels(illuminationPixels);
            cc.SetPixels(ccPixels);
            metallic.SetPixels(metallicPixels);
            roughness.SetPixels(roughnessPixels);

            //Apply to textures
            illumination.Apply();
            cc.Apply();
            metallic.Apply();
            roughness.Apply();

            Debug.Log("CyborgDecomposer: Copy complete, beginning write process...");

            byte[] diffuseBytes = illumination.EncodeToPNG();
            byte[] ccBytes = cc.EncodeToPNG();
            byte[] msBytes = metallic.EncodeToPNG();
            byte[] rBytes = roughness.EncodeToPNG();

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            try
            {
                //Diffuse
                FileStream diffuseStream = new FileStream(savePath + "/" + illumination.name + ".png", FileMode.Create);
                BinaryWriter diffuseWriter = new BinaryWriter(diffuseStream);
                diffuseWriter.Write(diffuseBytes);
                diffuseWriter.Close();
                //CC
                FileStream ccStream = new FileStream(savePath + "/" + cc.name + ".png", FileMode.Create);
                BinaryWriter ccWriter = new BinaryWriter(ccStream);
                ccWriter.Write(ccBytes);
                ccWriter.Close();
                //MetallicSmoothness
                FileStream msStream = new FileStream(savePath + "/" + metallic.name + ".png", FileMode.Create);
                BinaryWriter msWriter = new BinaryWriter(msStream);
                msWriter.Write(msBytes);
                msWriter.Close();
                //Roughness
                FileStream rStream = new FileStream(savePath + "/" + roughness.name + ".png", FileMode.Create);
                BinaryWriter rWriter = new BinaryWriter(rStream);
                rWriter.Write(rBytes);
                rWriter.Close();


                AssetDatabase.Refresh();

                //Diffuse
                TextureImporter importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + illumination.name + ".png");
                importedTextureCorrector.streamingMipmaps = true;
                importedTextureCorrector.crunchedCompression = true;
                importedTextureCorrector.SaveAndReimport();
                //CC
                importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + cc.name + ".png");
                importedTextureCorrector.streamingMipmaps = true;
                importedTextureCorrector.crunchedCompression = true;
                importedTextureCorrector.SaveAndReimport();
                //MetallicSmoothness
                importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + metallic.name + ".png");
                importedTextureCorrector.streamingMipmaps = true;
                importedTextureCorrector.crunchedCompression = true;
                importedTextureCorrector.SaveAndReimport();
                //Roughness
                importedTextureCorrector = (TextureImporter)AssetImporter.GetAtPath(savePath + "/" + roughness.name + ".png");
                importedTextureCorrector.streamingMipmaps = true;
                importedTextureCorrector.crunchedCompression = true;
                importedTextureCorrector.SaveAndReimport();
            }
            catch (Exception e)
            {
                //TODO: Change exception type and display a dialog with common solutions
                Debug.LogError(e.Message);

                Debug.LogError("CyborgDecomposer: Decomposition failed. Please see above message for more information.");
                EditorUtility.DisplayDialog("CyborgDecomposer", "Decomposition failed. Please check the console for more information.", ":(");
                return;
            }

            if (importer != null)
            {
                importer.crunchedCompression = true;
                importer.isReadable = false;
                importer.SaveAndReimport();
            }

            Debug.Log("CyborgDecomposer: Success! Decomposed textures are now available in the '" + savePath.Replace(Application.dataPath, "Assets/" + "' folder."));
            EditorUtility.DisplayDialog("CyborgDecomposer", "Success! Decomposed textures are now available in the '" + savePath.Replace(Application.dataPath, "Assets/") + "' folder.", "Got it");
            AssetDatabase.Refresh();
        }
    }
}
