using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;

static class MaterialCleanerTools
{
    [UnityEditor.MenuItem("Assets/Tools/清理材质无用贴图")]
    public static void ClearMaterial()
    {
        UnityEngine.Object[] objects = Selection.GetFiltered(typeof(object), SelectionMode.Assets);
        if (objects.Length == 0)
        {
            Debug.LogWarning("请选择资源目录后再进行操作");
            return;
        }

        foreach (var obj in objects)
        {
            float index = 0;
            string assetPath = AssetDatabase.GetAssetPath(obj);
            string projectPath = Application.dataPath.Replace("Assets", "");
            string fullPath = (projectPath + assetPath);
            if (fullPath.Contains(".mat"))
            {
                // 单个文件
                string fileAssetPath = fullPath.Replace(Application.dataPath, "Assets");
                ClearMaterialAsset(fileAssetPath);
            }
            else
            {
                //文件夹
                List<string> filePaths = CollectFilesByEnd(fullPath ,".mat");
                foreach (string filePath in filePaths)
                {
                    string fileAssetPath = filePath.Replace(Application.dataPath, "Assets");
                    ClearMaterialAsset(fileAssetPath);
                    index++;
                    EditorUtility.DisplayProgressBar("清理材质无用的缓存属性", fileAssetPath, index / (float)filePaths.Count);
                }
            }
           
        }

        EditorUtility.ClearProgressBar();
    }
    
    private static List<string> CollectFilesByEnd(string path, string end)
    {
        List<string> filePaths = new List<string>();
        string[] dirs = Directory.GetFiles(path, "*" + end);
        for (int j = 0; j < dirs.Length; j++)
        {
            filePaths.Add(dirs[j]);
        }

        return filePaths;
    }
    
    private static string GetProjectPath(string path)
    {
        string projectPath = "";
        projectPath = path.Split("Assets")[1];
        return "Assets/" + projectPath;
    }
    public static void ClearMaterialAsset(string path)
    {
        if (String.IsNullOrEmpty(path))
        {
            return;
        }

        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
            return;

        var deps = AssetDatabase.GetDependencies(new String[] { path });
        var deps_textures = deps.Where(s => IsTextureAsset(s)).ToList();
        var used_textures = new HashSet<String>();
        var shader = m.shader;
        var newMat = new Material(shader);
        var c = ShaderUtil.GetPropertyCount(shader);

        for (int i = 0; i < c; ++i)
        {
            var type = ShaderUtil.GetPropertyType(shader, i);
            var name = ShaderUtil.GetPropertyName(shader, i);
            var value = m.GetProperty(i);
            switch (type)
            {
                case ShaderUtil.ShaderPropertyType.Color:
                    {
                        newMat.SetColor(name, m.GetColor(name));
                    }
                    break;
                case ShaderUtil.ShaderPropertyType.Float:
                    {
                        newMat.SetFloat(name, m.GetFloat(name));
                    }
                    break;
                case ShaderUtil.ShaderPropertyType.Range:
                    {
                        newMat.SetFloat(name, (float)value);
                    }
                    break;
                case ShaderUtil.ShaderPropertyType.TexEnv:
                    {
                        newMat.SetTexture(name, (Texture)value);
                        newMat.SetTextureOffset(name, m.GetTextureOffset(name));
                        newMat.SetTextureScale(name, m.GetTextureScale(name));
                        var tpath = AssetDatabase.GetAssetPath((Texture)value);
                        if (!String.IsNullOrEmpty(tpath))
                        {
                            used_textures.Add(tpath);
                        }
                    }
                    break;
                case ShaderUtil.ShaderPropertyType.Vector:
                    {
                        newMat.SetVector(name, (Vector4)value);
                    }
                    break;
            }
        }
        bool rebuild = false;
        if (used_textures.Count != deps_textures.Count)
        {
            for (int i = 0; i < deps_textures.Count; ++i)
            {
                var _fn = deps_textures[i];
                if (!used_textures.Contains(_fn))
                {
                    rebuild = true;
                    UnityEngine.Debug.LogWarning(String.Format("unused texture: {0}", _fn));
                }
            }
        }
        if (!rebuild)
        {
            if (newMat != null)
            {
                UnityEngine.Object.DestroyImmediate(newMat);
            }
            return;
        }

        string basePath = Path.GetFullPath(path + "/../").Replace(Path.GetFullPath(Application.dataPath), "Assets");
        string fn = Path.GetFileNameWithoutExtension(path);
        string ext = Path.GetExtension(path);

        //SplitFullFilename(path, out fn, out ext, out basePath);
        var tempAssetPath = String.Format("{0}{1}{2}", basePath, fn, ext);
        var _test = AssetDatabase.LoadAllAssetsAtPath(tempAssetPath);

        if (_test != null)
        {
            AssetDatabase.DeleteAsset(tempAssetPath);
        }

        // create a new material to replace it latter
        AssetDatabase.CreateAsset(newMat, tempAssetPath);
        Resources.UnloadAsset(newMat);

        var tempAssetDataPath = String.Format("{0}{1}_datatemp.bytes", basePath, fn, ext);
        if (File.Exists(tempAssetPath))
        {
            // rename it to .bytes
            File.Copy(tempAssetPath, tempAssetDataPath, true);
            // delete temp material
            AssetDatabase.DeleteAsset(tempAssetPath);
            if (File.Exists(tempAssetDataPath))
            {
                // delete original material
                File.Delete(path);
                // replace original material with .bytes file
                File.Copy(tempAssetDataPath, path, true);
                // remove bytes file
                File.Delete(tempAssetDataPath);

                AssetDatabase.Refresh();
            }
        }
        
        return;
    }

    static object GetProperty(this Material material, int index)
    {
        var name = ShaderUtil.GetPropertyName(material.shader, index);
        var type = ShaderUtil.GetPropertyType(material.shader, index);
        switch (type)
        {
            case ShaderUtil.ShaderPropertyType.Color:
                return material.GetColor(name);
            case ShaderUtil.ShaderPropertyType.Vector:
                return material.GetVector(name);
            case ShaderUtil.ShaderPropertyType.Range:
            case ShaderUtil.ShaderPropertyType.Float:
                return material.GetFloat(name);
            case ShaderUtil.ShaderPropertyType.TexEnv:
                return material.GetTexture(name);
        }
        return null;
    }

    static bool IsTextureAsset(String assetPath)
    {
        var ext = Path.GetExtension(assetPath).ToLower();
        return ext == ".png" ||
            ext == ".tga" ||
            ext == ".jpg" ||
            ext == ".bmp" ||
            ext == ".psd" ||
            ext == ".dds" ||
            ext == ".exr";
    }
}
