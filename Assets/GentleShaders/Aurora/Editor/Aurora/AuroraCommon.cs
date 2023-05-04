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

    public static class AuroraCommon
    {
        public static string currentVersion = "AR3.0";

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
            Application.OpenURL("https://github.com/llealloo/vrc-udon-audio-link");
        }
    }
}