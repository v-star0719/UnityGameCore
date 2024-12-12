using System;
using System.IO;
using System.Reflection;
using GameCore.Core;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace GameCore.Unity
{
    public class EditorUtils
    {
        public static void TestIterateFolderAssets()
        {
            IterateAssetsInFolder(GetSelectedFolder(), (filePath) => { Debug.Log(filePath); });
        }

        /// <summary>
        /// return Assets/XXX...
        /// </summary>
        public static string GetSelectedFolder()
        {
            if (Selection.assetGUIDs.Length == 0)
            {
                return "Assets/";
            }

            //Selection.activeObject是上次选择的asset，和inspector里看到的一样
            //Selection.GetFiltered同上
            var path = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);
            if (System.IO.Directory.Exists(path))
            {
                return path;
            }
            else
            {
                return Path.GetDirectoryName(path);
            }
        }

        /// <summary>
        /// 遍历目录下的文件。也可以使用AssetDatabase.FindAssets来获取目录下的资源
        /// </summary>
        /// <param name="path">示例：Assets/Test/</param>
        /// <param name="action">传入的路径包含Assets/。比如Assets/Test/Test.txt</param>
        /// <param name="opt"></param>
        public static void IterateAssetsInFolder(string path, Action<string> action, SearchOption opt = SearchOption.AllDirectories)
        {
            path = path.Substring("Assets".Length);
            var paths = Directory.GetFiles(Application.dataPath + path, "*", opt);
            float f = 0;
            for(int i = 0; i<paths.Length; i++)
            {
                f++;
                string s = paths[i].Replace(@"\", @"/");
                if(s.EndsWith(".meta"))
                {
                    continue;
                }

                EditorUtility.DisplayProgressBar("", i + "/" + paths.Length, f/paths.Length);
                var index = s.IndexOf("Assets/");
                action(s.Substring(index));
            }
            EditorUtility.ClearProgressBar();
        }

        public static NamedBuildTarget CurrentNamedBuildTarget
        {
            get
            {
                BuildTarget buildTarget = EditorUserBuildSettings.activeBuildTarget;
                BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
                NamedBuildTarget namedBuildTarget = NamedBuildTarget.FromBuildTargetGroup(targetGroup);
                return namedBuildTarget;
            }
        }
    }
}
