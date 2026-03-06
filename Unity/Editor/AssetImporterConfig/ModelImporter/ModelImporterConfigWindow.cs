using UnityEditor;

namespace GameCore.Unity.Editor
{
    public class ModelImporterConfigWindow : AssetImporterConfigWindowBase<TextureImporterConfig>
    {
        [MenuItem("Tools/AssetImporterConfig/ModelImporterConfig")]
        public static void Open()
        {
            GetWindow<ModelImporterConfigWindow>();
        }
    }
}