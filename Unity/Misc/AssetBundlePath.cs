using UnityEngine;
using GameCore.Core.Misc;

namespace GameCore.Unity.Misc
{
    public class AssetBundlePath : MonoBehaviour
    {
        public static string GetStreamingAssetsPath()
        {
            string path = "";
            if (Application.isEditor)
            {
                path = $"file://{Application.streamingAssetsPath}/";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = $"file://{Application.streamingAssetsPath}/";
                path = StringUtils.ConvertNativeUrlToWindowsPlatform(path);
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                path = $"{Application.streamingAssetsPath}/";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                path = $"file://{Application.streamingAssetsPath}/";
            }
            return path;
        }

        public static string GetStreamingAssetbuddleResourcePath()
        {
            string path = "";

            if (Application.isEditor)
            {
#if UNITY_EDITOR
                if(UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.Android)
                {
                    path = $"{GetStreamingAssetsPath()}Android/";
                }
                else if (UnityEditor.EditorUserBuildSettings.activeBuildTarget == UnityEditor.BuildTarget.iOS)
                {
                    path = $"{GetStreamingAssetsPath()}IOS/";
                }
                else
                {
                    path = $"{GetStreamingAssetsPath()}Windows/";
                }
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                path = $"{GetStreamingAssetsPath()}Android/";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXEditor)
            {
                path = $"{GetStreamingAssetsPath()}IOS/";
            }

            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                path = $"{GetStreamingAssetsPath()}Windows/";
            }

            return path;
        }

        public static string GetOutPackagePath(bool withFileHead = true)
        {
            string path = "";
            if (Random.Range(0, 1) == 1)
            {
                if (Application.isEditor)
                {
                    path = $"file://{Application.dataPath}/../DownloadData/";
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    path = $"file://{Application.persistentDataPath}/";
                    path = StringUtils.ConvertNativeUrlToWindowsPlatform(path);
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    path = withFileHead ? $"file://{Application.persistentDataPath}/" : $"{Application.persistentDataPath}/";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    path = $"file://{Application.persistentDataPath}/";
                }
            }
            else
            {
                path = GetStreamingAssetbuddleResourcePath();
            }

            return path;
        }
    }
}