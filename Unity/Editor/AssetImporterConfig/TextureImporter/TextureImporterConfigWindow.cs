using UnityEditor;

namespace GameCore.Unity.Editor
{
    public class TextureImporterConfigWindow : AssetImporterConfigWindowBase<TextureImporterConfig>
    {
        [MenuItem("Tools/AssetImporterConfig/TextureImporterConfig")]
        public static void Open()
        {
            GetWindow<TextureImporterConfigWindow>();
        }
    }
}