﻿using System.IO;
using UnityEditor;
using UnityEngine;

namespace GentleShaders.Aurora.AR2.Helpers
{
    /// <summary>
    /// Aurora Shader included helper. Bakes the material's colors, textures, and optionally environment lighting into a texture to be used elsewhere.
    /// WIP!
    /// </summary>
    public class AuroraAR2Baker
    {
        /// <summary>
        /// Bakes the passed material as a texture, placing the texture in the same folder as the material.
        /// Material's shader needs to be unlit or contain a float property named "_lightingBypass".
        /// Material's shader also needs to contain a texture property named "_MainTex".
        /// </summary>
        /// <param name="auroraMat"></param>
        /// <param name="stripLighting"></param>
        public static void BakeMaterialAsTexture(Material auroraMat, bool stripLighting = true)
        {
            UnityEngine.Object asset = auroraMat;
            Texture2D mainTex = auroraMat.GetTexture("_MainTex") as Texture2D;

            TextureImporter ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(mainTex));
            if (ti)
            {
                ti.crunchedCompression = false;
                ti.isReadable = true;
                ti.SaveAndReimport();
                AssetDatabase.Refresh();
            }
            else
            {
                Debug.LogError("AuroraBaker: Could not retrieve main texture from asset database!");
                return;
            }

            string savePath = AssetDatabase.GetAssetPath(asset).Replace(".mat", "") + (stripLighting ? "_Baked" : "_Baked_Lit") + ".png";
            Texture2D final = GenerateAndBake(auroraMat, mainTex.width, mainTex.height, stripLighting, mainTex);

            File.WriteAllBytes(savePath, final.EncodeToPNG());
            AssetDatabase.Refresh();

            ti = (TextureImporter)TextureImporter.GetAtPath(savePath);
            ti.isReadable = true;
            ti.maxTextureSize = Mathf.Max(mainTex.width, mainTex.height);
            ti.crunchedCompression = true;
            ti.streamingMipmaps = true;
            ti.SaveAndReimport();

            ti = (TextureImporter)TextureImporter.GetAtPath(AssetDatabase.GetAssetPath(mainTex));
            ti.crunchedCompression = true;
            ti.streamingMipmaps = true;
            ti.SaveAndReimport();

            AssetDatabase.Refresh();
        }

        private static Texture2D GenerateAndBake(Material auroraMat, int resX, int resY, bool stripLighting, Texture2D defaultMainTex)
        {
            if (stripLighting)
            {
                auroraMat.SetFloat("_lightingBypass", 1f);
            }

            RenderTexture rtTemp = RenderTexture.GetTemporary(resX, resY);
            Graphics.Blit(null, rtTemp, auroraMat, 0, 0);
            RenderTexture.active = rtTemp;

            Texture2D bakedTexture = new Texture2D(resX, resY, TextureFormat.RGBA32, true);
            bakedTexture.ReadPixels(new Rect(0f, 0f, resX, resY), 0, 0, false);
            bakedTexture.Apply();

            RenderTexture.active = null;

            Color[] bakedTexturePixels = bakedTexture.GetPixels();
            Color[] mainTexPixels = defaultMainTex.GetPixels();
            for(int i = 0; i < bakedTexturePixels.Length; i++)
            {
                bakedTexturePixels[i].a = mainTexPixels[i].a;
            }
            bakedTexture.SetPixels(bakedTexturePixels);
            bakedTexture.Apply();

            if (stripLighting)
            {
                auroraMat.SetFloat("_lightingBypass", 0f);
            }

            return bakedTexture;
        }
    }
}