using System.Collections.Generic;
using UnityEngine;

namespace GentleShaders.Aurora.Common
{
    public enum Workflow
    {
        SpecularRoughness, SpecularGloss, MetallicRoughness, MetallicSmoothness
    }

    public enum SurfaceFinishWorkflow
    {
        Roughness, Smoothness
    }

    public enum TextureChannels
    {
        Red = 1 << 0, Green = 1 << 1, Blue = 1 << 2, Alpha = 1 << 3
    }

    public enum AuroraA3SwizzleTextures
    {
        Metallic = 0, Roughness = 1, Illumination = 2, Occlusion = 3
    }

    public enum NormalMapFormat
    {
        OpenGL = 0, DirectX = 1
    }

    public enum Aurora_Transparency_Setting
    {
        Opaque, Transparent, Cutout
    }
    public enum Aurora_Lighting_Setting
    {
        Lit, Unlit
    }

    public enum Aurora_BLSH_Setting
    {
        AccountForBLSH, Ignore
    }

    public enum Aurora_GI_Setting
    {
        Normal, Boosted
    }
    public enum Aurora_UVLayout_Setting
    {
        FiveX, ND5
    }

    public static class AuroraCommon
    {
        public static string currentVersion = "AR4.1";

        public static void OpenRepository()
        {
            Application.OpenURL("https://github.com/GentleLeviathan/Aurora-Shader-Suite");
        }

        public static void OpenRepositoryReleases()
        {
            Application.OpenURL("https://github.com/GentleLeviathan/Aurora-Shader-Suite/releases");
        }

        public static void OpenAudioLinkRepository()
        {
            Application.OpenURL("https://github.com/llealloo/audiolink");
        }

        static Dictionary<TextureFormat, float> TextureFormatBitsPerPixel = new Dictionary<TextureFormat, float>()
        {
            { TextureFormat.Alpha8, 9 },
            { TextureFormat.ARGB4444, 16 },
            { TextureFormat.RGB24, 24 },
            { TextureFormat.RGBA32, 32 },
            { TextureFormat.ARGB32, 32 },
            { TextureFormat.RGB565, 16 },
            { TextureFormat.R16, 16 },
            { TextureFormat.DXT1, 4 },
            { TextureFormat.DXT5, 8 },
            { TextureFormat.RGBA4444, 16 },
            { TextureFormat.BGRA32, 32 },
            { TextureFormat.RHalf, 16 },
            { TextureFormat.RGHalf, 32 },
            { TextureFormat.RGBAHalf, 64 },
            { TextureFormat.RFloat, 32 },
            { TextureFormat.RGFloat, 64 },
            { TextureFormat.RGBAFloat, 128 },
            { TextureFormat.YUY2, 16 },
            { TextureFormat.RGB9e5Float, 32 },
            { TextureFormat.BC6H, 8 },
            { TextureFormat.BC7, 8 },
            { TextureFormat.BC4, 4 },
            { TextureFormat.BC5, 8 },
            { TextureFormat.DXT1Crunched, 4 },
            { TextureFormat.DXT5Crunched, 8 },
            { TextureFormat.PVRTC_RGB2, 6 },
            { TextureFormat.PVRTC_RGBA2, 8 },
            { TextureFormat.PVRTC_RGB4, 12 },
            { TextureFormat.PVRTC_RGBA4, 16 },
            { TextureFormat.ETC_RGB4, 12 },
            { TextureFormat.EAC_R, 4 },
            { TextureFormat.EAC_R_SIGNED, 4 },
            { TextureFormat.EAC_RG, 8 },
            { TextureFormat.EAC_RG_SIGNED, 8 },
            { TextureFormat.ETC2_RGB, 12 },
            { TextureFormat.ETC2_RGBA1, 12 },
            { TextureFormat.ETC2_RGBA8, 32 },
            { TextureFormat.ASTC_4x4, 8 },
            { TextureFormat.ASTC_5x5, 5.12f },
            { TextureFormat.ASTC_6x6, 3.55f },
            { TextureFormat.ASTC_8x8, 2 },
            { TextureFormat.ASTC_10x10, 1.28f },
            { TextureFormat.ASTC_12x12, 1 },
            { TextureFormat.RG16, 16 },
            { TextureFormat.R8, 8 },
            { TextureFormat.ETC_RGB4Crunched, 12 },
            { TextureFormat.ETC2_RGBA8Crunched, 32 },
            { TextureFormat.ASTC_HDR_4x4, 8 },
            { TextureFormat.ASTC_HDR_5x5, 5.12f },
            { TextureFormat.ASTC_HDR_6x6, 3.55f },
            { TextureFormat.ASTC_HDR_8x8, 2 },
            { TextureFormat.ASTC_HDR_10x10, 1.28f },
            { TextureFormat.ASTC_HDR_12x12, 1 },
            { TextureFormat.RG32, 32 },
            { TextureFormat.RGB48, 48 },
            { TextureFormat.RGBA64, 64 }
        };

        public static long GetUncompressedTexture2DByteCount(Texture2D tex)
        {
            long byteCount = 0;
            float bitsPerPixel = TextureFormatBitsPerPixel[tex.format];
            for (int i = 0; i < tex.mipmapCount; i++)
            {
                byteCount += (long)Mathf.RoundToInt((float)((tex.width * tex.height) >> 2 * i) * bitsPerPixel / 8);
            }

            return byteCount;
        }

        public static string GetUncompressedTexture2DSizeString(Texture2D tex)
        {
            long byteCount = GetUncompressedTexture2DByteCount(tex);

            string sizeMB = ((byteCount / 1000000f) * 0.95367431640625f).ToString("n2") + "MB";

            return sizeMB;
        }


        public static string GetUncompressedTexture2DSizeString(long byteCount)
        {
            string sizeMB = ((byteCount / 1000000f) * 0.95367431640625f).ToString("n2") + "MB";

            return sizeMB;
        }
    }
}