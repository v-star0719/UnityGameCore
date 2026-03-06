using UnityEditor;

namespace GameCore.Unity.Editor
{
    public class AudioImporterConfigWindow : AssetImporterConfigWindowBase<AudioImporterConfig>
    {
        [MenuItem("Tools/AssetImporterConfig/AudioImporterConfig")]
        public static void Open()
        {
            GetWindow<AudioImporterConfigWindow>();
        }
    }
}