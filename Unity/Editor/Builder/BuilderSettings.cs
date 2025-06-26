using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity
{
    public class BuilderSettings : ScriptableObject
    {
        [SerializeField] private string desc = "BuilderSetting";
        public string Desc => desc;

        [SerializeField] private BuilderSettings parent;
        public BuilderSettings Parent => parent;

        [SerializeField] private bool builderOverride;
        [SerializeField] private string builder; //类的命名空间，不如Namespace1.Builder.Win64，会用反射创建打包器
        public string Builder => builderOverride || Parent == null ? builder : Parent.Builder;

        [SerializeField] private bool channelOverride;
        [SerializeField] private string channel; //渠道
        public string Channel => channelOverride || Parent == null ? channel : Parent.Channel;

        [SerializeField] private bool assetBundlePackageOverride;
        [SerializeField] private string assetBundlePackage;
        public string AssetBundlePackage => assetBundlePackageOverride || Parent == null ? assetBundlePackage : Parent.AssetBundlePackage;

        [SerializeField] private bool isReleaseOverride;
        [SerializeField] private bool isRelease;
        public bool IsRelease => isReleaseOverride || Parent == null ? isRelease : Parent.IsRelease;

        [SerializeField] private bool isDevelopmentOverride;
        [SerializeField] private bool isDevelopment;
        public bool IsDevelopment => isDevelopmentOverride || Parent == null ? isDevelopment : Parent.IsDevelopment;

        [SerializeField] private bool scriptingDefineSymbolsOverride;
        [SerializeField] [TextArea(5, 10)] private string scriptingDefineSymbols;
        public string ScriptingDefineSymbols => scriptingDefineSymbolsOverride || Parent == null ? scriptingDefineSymbols : Parent.ScriptingDefineSymbols;

        [SerializeField] private bool extraScriptingDefineSymbolsOverride;
        [SerializeField] [TextArea(5, 10)] private string extraScriptingDefineSymbols;
        public string ExtraScriptingDefineSymbols => extraScriptingDefineSymbolsOverride || Parent == null ? extraScriptingDefineSymbols : Parent.ExtraScriptingDefineSymbols;

        public bool CheckParentLoop()
        {
            //检查一下嵌套
            var pSlow = parent;
            var pFast = parent;
            while (pFast != null && pFast.parent != null)
            {
                pSlow = pFast.parent;
                pFast = pFast.parent.parent;
                if (pSlow == pFast)
                {
                    return true;
                }
            }

            return false;
        }
    }
}