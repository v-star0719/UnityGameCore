using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class AssetImporterConfigBase : ScriptableObject, ITreeNode
    {
        public class PlatformNames
        {
            public static string defPlatform = "default";
            public static string standalone = "Standalone";
            public static string web = "Web";
            public static string iPhone = "iPhone";
            public static string android = "Android";
            public static string webGL = "WebGL";
            public static string windowsStoreApps = "Windows Store Apps";
            public static string ps4 = "PS4";
            public static string xboxOne = "XboxOne";
            public static string nintendoSwitch = "Nintendo Switch";
            public static string tvOS = "tvOS";
            public static string [] All = new[]
            {
                standalone, iPhone, android, ps4, xboxOne, nintendoSwitch, windowsStoreApps, web, webGL, tvOS
            };
        }

        //ITreeNode
        public string Name => desc;
        public Rect Rect { get; set; }
        public List<ITreeNode> ChildNodes { get; protected set; } = new List<ITreeNode>();
        public virtual ITreeNode ParentNode => null;
        public bool IsDragging { get; set; }
        public float TotalWidth { get; set; }
        //-------------------------------------------

        [SerializeField] protected string desc = "AssetImporterConfig";
        public string Desc => desc;

        [SerializeField] protected List<string> regList = new();
        [SerializeField] protected List<UnityEngine.Object> fileAndDirList = new();

        //导入一个资源的时候，对应的对象会被销毁，fileAndDirList里是null，导入结束后会再次赋值。因此使用的时候转为路径使用。
        public List<string> fileAndDirPathList { get; protected set; } = new();
        [NonSerialized] public bool isParentChanged; //parent改变了的话RootConfig需要重新判断

        public virtual void Apply(AssetImporter importer)
        {
        }

        public bool CheckParentLoop()
        {
            //检查一下是否存在死循环
            var pSlow = ParentNode;
            var pFast = ParentNode;
            while(pFast != null && pFast.ParentNode != null)
            {
                pSlow = pFast.ParentNode;
                pFast = pFast.ParentNode.ParentNode;
                if(pSlow == pFast)
                {
                    return true;
                }
            }
            return false;
        }

        public virtual bool IsMatch(string path)
        {
            foreach (var p in fileAndDirPathList)
            {
                if (Directory.Exists(p))
                {
                    if (path.StartsWith(p))
                    {
                        return true;
                    }
                }
                else if (path == p)
                {
                    return true;
                }
            }

            foreach (var reg in regList)
            {
                if (Regex.IsMatch(path, reg))
                {
                    return true;
                }
            }
            return false;
        }

        public AssetImporterConfigBase GetMatched(string path)
        {
            foreach (var c in ChildNodes)
            {
                var child = c as AssetImporterConfigBase;
                var rt = child.GetMatched(path);
                if (rt != null)
                {
                    return rt;
                }
            }

            if (IsMatch(path))
            {
                return this;
            }

            return null;
        }

        protected void OnValidate()
        {
            fileAndDirPathList.Clear();
            foreach (var o in fileAndDirList)
            {
                fileAndDirPathList.Add(AssetDatabase.GetAssetPath(o));
            }
        }
    }
}