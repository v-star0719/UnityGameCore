using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Kernel.Unity;
using UnityEditor;
using UnityEngine;

public class AssetReference
{
    public const string ASSET_DIR = "Assets/";//根目录
    private const string DATA_PATH = "/../Temp/AssetReference.data";

    private static AssetReference instance;
    public static AssetReference Instance => instance ??= new AssetReference();
    private Dictionary<string, AssetReferenceData> assetDict = new Dictionary<string, AssetReferenceData>();
    private bool hasLoaded = false;

    public void Clear()
    {
        assetDict.Clear();
        hasLoaded = false;
    }

    public void Load()
    {
        if (hasLoaded)
        {
            return;
        }

        var path = Application.dataPath + DATA_PATH;
        if (!File.Exists(path))
        {
            Debug.Log("AssetReference: no file to load");
            return;
        }

        hasLoaded = true;
        BinaryFormatter formatter = new BinaryFormatter();
        using(FileStream stream = new FileStream(path, FileMode.OpenOrCreate))
        {
            assetDict = formatter.Deserialize(stream) as Dictionary<string, AssetReferenceData>;
            Debug.Log("AssetReference: load " + assetDict.Count);
        }
    }

    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        using(FileStream stream = new FileStream(Application.dataPath + DATA_PATH, FileMode.OpenOrCreate))
        {
            formatter.Serialize(stream, assetDict);
            Debug.Log("AssetReference: save " + assetDict.Count);
        }
    }
    
    public void BuildDatas()
    {
        assetDict.Clear();
        var time = System.DateTime.Now.Ticks;
        EditorUtils.IterateAssetsInFolder(ASSET_DIR, path =>
        {
            if(IsSkipped(path))
            {
                return;
            }

            var deps = AssetDatabase.GetDependencies(path);
            var rootAsset = GetReferenceData(path);
            foreach (string s in deps)
            {
                if (path == s)
                {
                    continue;//会包含自己，跳过
                }
                var dependedAsset = GetReferenceData(s);
                dependedAsset.referencesBy.Add(rootAsset);
                rootAsset.references.Add(dependedAsset);
            }
        });
        Debug.Log("AssetReference build data finished " + ((System.DateTime.Now.Ticks - time) * 0.0000001f).ToString("f3"));
    }

    public AssetReferenceData GetReferenceData(string path, bool createNew = true)
    {
        AssetReferenceData ad;
        if (!assetDict.TryGetValue(path, out ad) && createNew)
        {
            ad = new AssetReferenceData();
            ad.path = path;
            ad.dependencyHash = AssetDatabase.GetAssetDependencyHash(path);
            assetDict.Add(path, ad);
        }

        return ad;
    }

    public IEnumerable<AssetReferenceData> GetDatas()
    {
        return assetDict.Values;
    }

    //不处理的文件
    public static bool IsSkipped(string s)
    {
        if(s.EndsWith(".meta"))
        {
            return true;
        }
        return false;
    }
}

[Serializable]
public class AssetReferenceData
{
    public string path; //"Assets/xxxx"
    public Hash128 dependencyHash;
    public List<AssetReferenceData> references = new List<AssetReferenceData>();//依赖了哪些资源
    public List<AssetReferenceData> referencesBy = new List<AssetReferenceData>();//被哪些资源依赖了

    public bool IsReferencedByAnyOne
    {
        get { return referencesBy.Count > 0 || path.EndsWith(".unity"); }
    }
}
