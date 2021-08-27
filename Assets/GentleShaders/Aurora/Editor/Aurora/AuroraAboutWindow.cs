using UnityEngine;
using UnityEditor;

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
                    HelperPage();
                    break;
                case 2:
                    page = 0;
                    LandingPage();
                    break;
            }

            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Next Page"))
            {
                page++;
            }
            if (GUILayout.Button("Close"))
            {
                window.Close();
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
            GUILayout.Label("Aurora shader is designed for a personal game project that shares a texturing process with Microsoft's 'Halo' assets.", common);
            GUILayout.Label("It includes helper functions to utilize 'Halo' game textures by converting them to a common format.", common);
            GUILayout.Space(4f);
            GUILayout.Label("The primary purpose of this shader is to enable working with a diverse collection of textures and meshes created in the style of Halo game assets.", common);
            GUILayout.Space(2f);
            GUILayout.Label("A quick disclaimer: This shader is provided to the public for free under the MIT License, and does not condone reverse-engineering or pirating.", common);
            GUILayout.Label("Aurora on GitHub: ", common);
            if(GUILayout.Button(new GUIContent("https://github.com/GentleLeviathan/Aurora-Shader-Suite", "Opens the Aurora Shader GitHub Repository.")))
            {
                OpenRepository();
            }
            GUILayout.Space(10f);

            GUILayout.Label("FAQ", header);
            GUILayout.Space(4f);
            GUILayout.Label("Aurora?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("'Aurora' is the name of the texture swizzle utilized by the shader to replace traditional Metallic_Smoothness, Occlusion, and Emission textures.", common);
            GUILayout.Label("This decreases video memory usage, build file size, and provides a slightly faster workflow.", common);
            GUILayout.Space(4f);

            GUILayout.Label("Shader Features/Multi Compile?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("Local Shader Features are utilized, making the minimum supported Unity version 2019.1.x.", common);
        }

        private void HelperPage()
        {
            GUILayout.Label("What is the purpose of the Helper Utilities?", boldLabels);
            GUILayout.Space(4f);
            GUILayout.Label("The Helper Utilities are a suite of tools to convert 'Halo' textures into the format that the shader is designed for.", common);
            GUILayout.Label("You will find a short description of each helper utility by hovering over their buttons when inspecting a Aurora material.", common);
            GUILayout.Space(4f);
            GUILayout.Label("Additional helper utilities will be implemented as the need for them arises.", common);
            GUILayout.Label("If you would like to request a helper, please do so!", common);
        }

        public static void OpenRepository()
        {
            Application.OpenURL("https://github.com/GentleLeviathan/Aurora-Shader-Suite");
        }

        public static void OpenRepositoryReleases()
        {
            Application.OpenURL("https://github.com/GentleLeviathan/Aurora-Shader-Suite/releases");
        }
    }
}
