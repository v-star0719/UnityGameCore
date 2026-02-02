using UnityEditor;

namespace GameCore.Unity
{
    public class AssetPostprocessorApplyConfig : AssetPostprocessor
    {
        private void Apply(AssetImporterConfigBase rootConfig)
        {
            if (rootConfig == null)
            {
                return;
            }
            var config = rootConfig.GetMatched(assetPath);
            if(config != null)
            {
                config.Apply(assetImporter);
            }
        }

        private void OnPreprocessTexture()
        {
            Apply(TextureImporterConfigWindow.RootConfig);
        }

        private void OnPreprocessModel()
        {
            Apply(ModelImporterConfigWindow.RootConfig);
        }

        private void OnPreprocessAudio()
        {
            Apply(AudioImporterConfigWindow.RootConfig);
        }
    }
}

