using UnityEditor;

namespace GameCore.Unity
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