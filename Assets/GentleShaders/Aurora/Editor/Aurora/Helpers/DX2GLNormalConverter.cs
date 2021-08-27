using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace GentleShaders.Aurora.Helpers
{
    /// <summary>
    /// Aurora Shader included helper. Converts between DirectX and OpenGL normal map formats.
    /// WIP!
    /// Thank you to FiyCsf for the helper suggestion.
    /// </summary>
    public static class DX2GLNormalConverter
    {
        private enum DXorGLNormal
        {
            DirectX, OpenGL
        }

        public static Texture2D ConvertToDXOGL(Texture2D normal, UnityEngine.Object asset)
        {
            DXorGLNormal toConvertTo = DXorGLNormal.OpenGL;
            string savePath = AssetDatabase.GetAssetPath(asset).Replace(asset.name, "").Replace(".png", "").Replace(".jpg", "").Replace(".bmp", "").Replace(".tif", "").Replace(".dds", "").Replace(".jpeg", "").Replace(".tga", "");
            if (normal.name.ToLower().Contains("opengl") || normal.name.ToLower().Contains("ogl") || normal.name.ToLower().Contains("open") && normal.name.ToLower().Contains("gl"))
            {
                toConvertTo = DXorGLNormal.DirectX;
            }
            TextureImporter importer = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(asset));
            importer.textureType = TextureImporterType.Default;
            if (importer.crunchedCompression)
            {
                importer.crunchedCompression = false;
            }
            if (normal.isReadable)
            {
                importer.SaveAndReimport();
                return Convert(normal, savePath, toConvertTo, importer);
            }
            else
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
                Debug.Log("DX2GLNormalConverter: Texture was not readable, changed texture import settings to enable Read/Write.");
                return Convert(normal, savePath, toConvertTo, importer);
            }
        }

        private static Texture2D Convert(Texture2D normal, string savePath, DXorGLNormal toConvertTo, TextureImporter importer = null)
        {
            Debug.Log("DX2GLNormalConverter: Beginning Conversion... Texture Name: " + normal.name);

            Texture2D newNormal = new Texture2D(normal.width, normal.height);
            Texture2D writtenNormal = null;

            if (toConvertTo == DXorGLNormal.OpenGL)
            {
                newNormal.name = normal.name.Replace("_DirectX", "") + "_OpenGL";
            }
            else
            {
                newNormal.name = normal.name.Replace("_OpenGL", "") + "_DirectX";
            }

            Debug.LogWarning("DX2GLNormalConverter: Setup complete, beginning copy process..");

            //Create and fill pixel arrays
            Color[] normalPixels = normal.GetPixels();
            Color[] convertedPixels = new Color[normal.height * normal.width];

            for (int i = 0; i < normalPixels.Length; i++)
            {
                Color normalDX = normalPixels[i];

                convertedPixels[i] = new Color(normalDX.r, Mathf.Abs(1 - normalDX.g), normalDX.b);
            }

            //Set Pixels
            newNormal.SetPixels(convertedPixels);

            //Apply to textures
            newNormal.Apply();

            Debug.Log("DX2GLNormalConverter: Conversion complete, beginning write process...");

            byte[] normalBytes = newNormal.EncodeToPNG();

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

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

                writtenNormal = (Texture2D)AssetDatabase.LoadAssetAtPath(savePath + newNormal.name + ".png", typeof(Texture2D));
            }
            catch (Exception e)
            {
                //TODO: Change exception type and display a dialog with common solutions
                Debug.LogError(e.Message);

                Debug.LogError("DX2GLNormalConverter: Conversion failed. Please see above message for more information.");
                EditorUtility.DisplayDialog("DX2GLNormalConverter", "Conversion failed. Please check the console for more information.", ":(");
                return null;
            }

            if (importer != null)
            {
                importer.textureType = TextureImporterType.NormalMap;
                importer.crunchedCompression = true;
                importer.isReadable = false;
                importer.SaveAndReimport();
            }

            Debug.Log("DX2GLNormalConverter: Success! Converted normal is now available in the '" + savePath.Replace(Application.dataPath, "Assets/") + "' folder.");
            EditorUtility.DisplayDialog("DX2GLNormalConverter", "Success! Converted normal is now available in the '" + savePath.Replace(Application.dataPath, "Assets/") + "' folder and has been applied to the material.", "Got it");

            return writtenNormal;
        }
    }

}