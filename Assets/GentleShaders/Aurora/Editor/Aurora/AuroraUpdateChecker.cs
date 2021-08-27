using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GentleShaders.Aurora
{
    public static class AuroraUpdateChecker
    {
        public static async Task<bool> CheckForUpdates()
        {
            UnityWebRequest www = UnityWebRequest.Get("http://raw.githubusercontent.com/GentleLeviathan/Aurora-Shader-Suite/main/masterVersion");
            DownloadHandler handler = www.downloadHandler;
            UnityWebRequestAsyncOperation op = www.SendWebRequest();

            while (www.downloadProgress < 1.0f)
            {
                await Task.Delay(100);
            }
            if (www.isHttpError)
            {
                Debug.Log("Aurora Shader Suite - There was an error checking for an update. - " + www.error);
                return false;
            }

            return handler.text != AuroraEditor.currentVersion;
        }
    }
}
