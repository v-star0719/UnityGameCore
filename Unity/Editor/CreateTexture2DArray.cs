using UnityEditor;
using UnityEngine;

public class CreateTexture2DArray : EditorWindow
{
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CreateTexture2DArray));
    }

    public string _TexArrayName;
    public string _TexArrayTarget;
    public int ArrayCount;

    public Texture2D[] textures;

    private FilterMode _filterMode = FilterMode.Bilinear;
    private TextureWrapMode _textureWrapMode = TextureWrapMode.Repeat;

    private int oriCount = -1;
    public void OnGUI()
    {
        ArrayCount = EditorGUILayout.IntField("Array count", ArrayCount);

        if (ArrayCount != oriCount)
        {
            textures = new Texture2D[ArrayCount];
            oriCount = ArrayCount;
        }
        
        if( null == textures || ArrayCount != textures.Length)
            return;
      
        for (int i = 0; i < ArrayCount; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i.ToString());
            if ( textures[i] != null)
            {
                EditorGUILayout.LabelField( textures[i].name);
            }
            textures[i] =(Texture2D) EditorGUILayout.ObjectField(textures[i], typeof(Texture2D), false);
            EditorGUILayout.EndHorizontal();
        }
        
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("OutPutSetting");
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.BeginHorizontal();
        _TexArrayTarget = EditorGUILayout.TextField("输出路径", _TexArrayTarget);
        if (GUILayout.Button("浏览", GUILayout.Width(50f)))
        {
            _TexArrayTarget = EditorUtility.OpenFolderPanel("输出路径", Application.dataPath + "/HotRes", "");
        }
        EditorGUILayout.EndHorizontal();
        _TexArrayName = EditorGUILayout.TextField("OutPutName", _TexArrayName);
        _textureWrapMode = (TextureWrapMode)EditorGUILayout.EnumPopup("TextureWarpMode", _textureWrapMode);
        _filterMode = (FilterMode)EditorGUILayout.EnumPopup("TextureWarpMode", _filterMode);
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create Texture2D Array"))
        {
            CreateTextureArray();
        }
        EditorGUILayout.EndHorizontal();
    }

    private void CreateTextureArray()
    {
        if (_TexArrayName == null || _TexArrayTarget == null)
        {
            Debug.Log("输出的名字或者路径为空，重新输入");
            return;
        }
        Texture2DArray texArr = new Texture2DArray(textures[0].width, textures[0].width, textures.Length, textures[0].format, false, false);
        for (int i = 0; i < textures.Length; i++)
        {
            Graphics.CopyTexture(textures[i], 0, 0, texArr, i, 0);
        }

        texArr.wrapMode = _textureWrapMode;
        texArr.filterMode = _filterMode;
        string outPutFolder = _TexArrayTarget.Split("Assets")[1];
        AssetDatabase.CreateAsset(texArr, "Assets/"+outPutFolder+"/"+ _TexArrayName + ".asset");
        Debug.Log("输出路径："+"Assets/"+outPutFolder+"/"+ _TexArrayName + ".asset");
    }

}
