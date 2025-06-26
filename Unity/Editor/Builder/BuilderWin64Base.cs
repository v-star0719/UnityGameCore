using UnityEditor;

namespace GameCore.Unity
{
    public class BuilderWin64Base : BuilderBase
    {
        public BuilderWin64Base(object settings) : base(settings)
        {
            buildTarget = BuildTarget.StandaloneWindows64;
        }

        protected override string GetLocationPathName()
        {
            var release = Settings.IsRelease ? "_release" : "";
            var trial = "";// Settings.isTrial ? "_trial" : "";
            var dev = Settings.IsDevelopment ? "_dev" : "";
            var ch = string.IsNullOrEmpty(Settings.Channel) ? string.Empty : "_" + Settings.Channel;
            return $"{outputDir2}/Win64{ch}{trial}{release}{dev}_{GetVersion()}/{PlayerSettings.productName}.exe";
        }
    }
}