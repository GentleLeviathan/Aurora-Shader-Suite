using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GentleShaders.Aurora.Five
{
    public static class AuroraUpdateChecker
    {
        public static async Task<bool> CheckForUpdates()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://raw.githubusercontent.com/GentleLeviathan/Aurora-Shader-Suite/main/masterVersion");
            DownloadHandler handler = www.downloadHandler;
            UnityWebRequestAsyncOperation op = www.SendWebRequest();

            while (www.downloadProgress < 1.0f)
            {
                await Task.Delay(100);
            }
            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Aurora Shader Suite - There was an error checking for an update. - " + www.error);
                return false;
            }

            return !handler.text.Contains(AuroraCommon.currentVersion);
        }

        public static async Task<string> GetNewestVersionString()
        {
            UnityWebRequest www = UnityWebRequest.Get("https://raw.githubusercontent.com/GentleLeviathan/Aurora-Shader-Suite/main/masterVersion");
            DownloadHandler handler = www.downloadHandler;
            UnityWebRequestAsyncOperation op = www.SendWebRequest();

            while (www.downloadProgress < 1.0f)
            {
                await Task.Delay(100);
            }
            if (www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("Aurora Shader Suite - There was an error checking for an update. - " + www.error);
                return AuroraCommon.currentVersion;
            }

            return handler.text.Replace("\n", "");
        }

        public static async Task<string> PerformUpdateCheck()
        {
            string updateCheckResult = await GetNewestVersionString();
            int versionCompare = AuroraCommon.CompareAuroraVersion(AuroraCommon.currentVersion, updateCheckResult);

            if(versionCompare < 0)
            {
                return "You are using an unpublished experimental version.";
            }
            if(versionCompare == 0)
            {
                return "";
            }
            if(versionCompare > 0)
            {
                return "> An update is available! (Current Version " + AuroraCommon.currentVersion + ")" + "\n(New Version " + updateCheckResult + ") ";
            }
            return "<";
        }
    }
}
