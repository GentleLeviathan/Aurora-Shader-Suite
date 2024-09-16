using UnityEngine;
using UnityEditor;
using GentleShaders.Aurora.Common;

namespace GentleShaders.Aurora
{
    /// <summary>
    /// Aurora Shader About Window. Temporary implementation, upon PR2 will likely retrieve body content from a webserver.
    /// WIP!
    /// </summary>
    public class AuroraAboutWindow : EditorWindow
    {
        GUIStyle header;
        GUIStyle boldLabels;
        GUIStyle common;
        private bool setup = false;
        private int page = 0;
        private static AuroraAboutWindow window;

        public static void Init()
        {
            window = (AuroraAboutWindow)EditorWindow.GetWindow(typeof(AuroraAboutWindow));
            window.titleContent = new GUIContent("Aurora - About");
            window.minSize = new Vector2(450f, 700f);
            window.Show();
        }

        private void SetupStyles()
        {
            //header
            Color text = new Color(0.8f, 0.8f, 0.8f);
            header = new GUIStyle();
            header.alignment = TextAnchor.MiddleCenter;
            header.fontSize = 24;
            header.padding = new RectOffset(0, 0, 10, 10);
            header.normal.textColor = text;


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

        private void OnGUI()
        {
            if (!setup) { SetupStyles(); }

            DrawHeader();

            switch (page)
            {
                case 0:
                    LandingPage();
                    break;
                case 1:
                    A3Page();
                    break;
                case 2:
                    HelperPage();
                    break;
                case 3:
                    RavePage();
                    break;
                case 4:
                    page = 0;
                    LandingPage();
                    break;
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                window.Close();
            }
            if (GUILayout.Button("Next Page"))
            {
                page++;
            }
            GUILayout.EndHorizontal();
        }

        private void DrawHeader()
        {
            GUILayout.Space(5f);
            GUILayout.Label("About the Aurora Shader", header);
            GUILayout.Space(10f);
        }

        private void LandingPage()
        {
            GUILayout.Label("What is the purpose of the Aurora Shader?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("Aurora shader is designed for a personal game project in order to reduce texture memory usage.", common);
            GUILayout.Space(2f);
            GUILayout.Label("A quick disclaimer: This shader is provided to the public for free under the MIT License, and does not condone reverse-engineering or pirating.", common);
            GUILayout.Label("Aurora on GitHub: ", common);
            if (GUILayout.Button(new GUIContent("https://github.com/GentleLeviathan/Aurora-Shader-Suite", "Opens the Aurora Shader GitHub Repository.")))
            {
                AuroraCommon.OpenRepository();
            }
            GUILayout.Space(10f);

            GUILayout.Label("FAQ", header);
            GUILayout.Space(4f);
            GUILayout.Label("Aurora?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("'Aurora' is the name of the texture swizzle utilized by the shader to replace traditional Metallic, Roughness, Emission, and Occlusion textures.", common);
            GUILayout.Label("This decreases video memory usage, build file size, and provides a slightly faster workflow.", common);
            GUILayout.Space(4f);

            GUILayout.Label("Shader Features/Multi Compile?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("Local Shader Features are utilized, making the minimum supported Unity version 2019.1.x.", common);
        }

        private void A3Page()
        {
            GUILayout.Label("What's new?", header);
            GUILayout.Space(8f);
            GUILayout.Label("Aurora AR3 changes the lighting model significantly, observing energy conservation for a 'PBR' result.", common);
            GUILayout.Label("(Note: You may check the 'Boost GI?' box in the Advanced section of the shader to get a very similiar result to AR2.)", common);
            GUILayout.Space(30f);
            GUILayout.Label("AR3 introduces support for a UDIM approach to texture mapping! You may use up to 5 total texture sets per material.", common);
            GUILayout.Label("Each texture set gets a 1.0 range in the U-dimension of the UV coordinates.", common);
            GUILayout.Label("Texture Set 0 gets 0-1 on the U-dimension, 0-1 on the V-dimension.", common);
            GUILayout.Label("Texture Set 1 gets 1-2 on the U-dimension, 0-1 on the V-dimension.", common);
            GUILayout.Label("Texture Set 2 gets 2-3 on the U-dimension, 0-1 on the V-dimension.", common);
            GUILayout.Label("Texture Set 3 gets 3-4 on the U-dimension, 0-1 on the V-dimension.", common);
            GUILayout.Label("Texture Set 4 gets 4-5 on the U-dimension, 0-1 on the V-dimension.", common);
            GUILayout.Space(30f);
            GUILayout.Label("The A3 format removes Curvature information from the Aurora texture, replacing it with Roughness (which was previously stored in the Diffuse texture).", common);
            GUILayout.Label("After heavy experimentation, I no longer had a need for the mesh curvature information with a lighting model change.", common);
            GUILayout.Label("This resulted in a 30%~ reduction in texture memory usage, smaller file sizes, and the option to expand the shader to support transparency.", common);
            GUILayout.Space(30f);
        }

        private void HelperPage()
        {
            GUILayout.Label("What is the purpose of the Helper Utilities?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("The Helper Utilities are a suite of tools to assist in the conversion of existing textures into the Aurora format.", common);
            GUILayout.Label("You will find a short description of each helper utility by hovering over their buttons when inspecting a Aurora material.", common);
            GUILayout.Space(30f);
            GUILayout.Label("The most powerful helper utility Aurora A3 provides is the 'Pack Aurora Textures' helper.", boldLabels);
            GUILayout.Label("This helper will allow you to pack individual data textures (Metallic, Smoothness, Emission, and Occlusion)" +
                            " into the Aurora format by generating a new texture.", common);
            GUILayout.Label("It also includes a tool to convert OpenGL normal maps to DirectX normal maps, and vice-versa.", common);
            GUILayout.Space(30f);
            GUILayout.Label("Additional helper utilities will be implemented as the need for them arises.", common);
            GUILayout.Label("If you would like to request a helper, please do so by submitting an issue with the label 'enhancement'!", common);
        }

        private void RavePage()
        {
            GUILayout.Label("Rave? AudioLink?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("The Aurora shader affords some features for use in the virtual reality social platform 'VRChat'.", common);
            GUILayout.Label("The Rave section is not exclusive to use with VRChat or AudioLink, as it can be used as just an additional emission slot.", common);
            GUILayout.Space(30f);
            GUILayout.Label("llealloo's VRChat Udon AudioLink is a powerful tool to enable VRChat worlds and virtual avatars to synchronize their lighting effects" +
                            " to audio/music streaming to the client.", common);
            GUILayout.Label("VRC Udon AudioLink on GitHub: ", common);
            if (GUILayout.Button(new GUIContent("https://github.com/llealloo/vrc-udon-audio-link", "Opens the Aurora Shader GitHub Repository.")))
            {
                AuroraCommon.OpenAudioLinkRepository();
            }
            GUILayout.Space(30f);
            GUILayout.Label("If you wish to use AudioLink on your VRChat avatar, it is *strongly* recommended to import the AudioLink package from the link above.", common);
            GUILayout.Label("The Aurora shader includes the necessary header file to create AudioLink avatars without importing the package," +
                            " but you will not be able to preview the effects without the AudioLink prefab present.", common);
            GUILayout.Space(30f);
            GUILayout.Label("You may hover over any option in the Rave section to get a short description of it's purpose and effect on the material.", common);
            GUILayout.Space(4f);
            GUILayout.Label("Rave CC textures are 4-channel masks, with each channel being assigned an HDR color property and it's own AudioLink integration.", common);
            GUILayout.Label("Rave Mask textures 4-channel textures that are used to augment the Rave CC channels by multiplying them with itself.", common);
            GUILayout.Label("Chronotensity checkboxes augment the scroll speed by slowing it down to 10% of it's inspector value, and then speeding up to some portion" +
                " of the inspector value based on the audio intensity provided by AudioLink.", common);
        }
    }
}
