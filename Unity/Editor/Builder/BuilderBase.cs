using Codice.Client.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Kernel.Unity
{
    public abstract class BuilderBase
    {
        public BuildTarget buildTarget;
        public string outputDir;//存放打出来的包的目录，如果是../开头，则放进当前工程目录里
        public List<string> scriptingDefineSymbols = new List<string>();

        //对于Windows来说，打包出来是一个文件夹
        protected string packageName;

        //如果需要增加步骤，在这里面加
        public void Build()
        {
            if(EditorUserBuildSettings.activeBuildTarget != buildTarget)
            {
                Debug.LogError($"current active build target is not {buildTarget}, it is {EditorUserBuildSettings.activeBuildTarget}");
                return;
            }
            SetPackageName();
            //SetScriptingDefineSymbols();
            DoBuild();
        }

        protected virtual void SetPackageName()
        {
            packageName = $"{PlayerSettings.productName}_{buildTarget}_version_{DateTime.Now:yy-MM-dd,HH-mm-ss}";
        }

        protected virtual void SetScriptingDefineSymbols()
        {
            PlayerSettings.SetScriptingDefineSymbols(EditorUtils.CurrentNamedBuildTarget, GetExtraScriptingDefineSymbols().ToArray());
        }

        //PlayerSettings里的还会存在，这是额外附加的
        protected virtual List<string> GetExtraScriptingDefineSymbols()
        {
            return new List<string>();
        }

        protected virtual List<string> GetScenes()
        {
            var rt = new List<string>();
            rt.AddRange(EditorBuildSettings.scenes);
            return rt;
        }

        //前缀还是PlayerSettings里的BundleVersion,自动增加日期后缀
        protected virtual string GetVersion()
        {
            return $"{PlayerSettings.bundleVersion}.{DateTime.Now:yymmddhhmm}";
        }

        ///就是buildPlayerOptions.locationPathName这个字段的值
        ///存放build出来的结果的目录。
        ///对于Windows，是存放可执行文件的路径。比如D:/123/4.exe。打包出来的文件都放在123目录，目录没有回自动新建。
        ///如果用D:/123这样的路径，就会把exe生成为123名字的文件，不带扩展名。所有文件都放在D:/目录下。
        protected virtual string GetLocationPathName()
        {
            return "";
        }

        protected virtual BuildOptions GetOptions()
        {
            return BuildOptions.None;
        }
        
        protected virtual void DoBuild()
        {
            //var sceneArray = new string[scenes.Count];
            //for (var i = 0; i < scenes.Count; i++)
            //{
            //    sceneArray[i] = AssetDatabase.GetAssetPath(scenes[i]);
            //}

            //如果不指定目录，会自动创建在根目录下。如果目录已经有一个包，会更新修改的文件，但是是否会删除用不到的就不知道了
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetScenes().ToArray();
            buildPlayerOptions.locationPathName = GetLocationPathName();
            buildPlayerOptions.target = buildTarget;
            buildPlayerOptions.options = GetOptions();
            buildPlayerOptions.extraScriptingDefines = GetExtraScriptingDefineSymbols().ToArray();

            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;
            if(summary.result == BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                EditorUtility.RevealInFinder(summary.outputPath);
            }
            else if(summary.result == BuildResult.Failed)
            {
                Debug.Log("Build failed");
            }
        }
    }
}