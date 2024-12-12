using System;
using System.Collections.Generic;
using GameCore.Edit;
using GameCore.Unity;
using UnityEditor;
using UnityEngine;

//一个资源引用的其他资源，以及被哪些资源引用了
public class AssetReferenceEditor : EditorWindow
{
    private const int PAGE_ASSET_NUM = 100;
    private List<AssetReferenceData> dataList = new List<AssetReferenceData>();
    private int expandIndex = -1;
    private int curPageIndex;
    private Vector2 scrollPos = Vector2.zero;
    private string filter;
    

    [MenuItem("Tools/AssetReference", false)]
    public static void AssetUsageWindow()
    {
        GetWindow<AssetReferenceEditor>();
    }

    private void OnGUI()
    {
        GUITopBar();
        GUIDAssets();
    }

    private void GUITopBar()
    {
        using(GUIUtil.LayoutHorizontal())
        {
            using(GUIUtil.LayoutHorizontal())
            {
                if(GUILayout.Button("Build", GUILayout.ExpandWidth(false)))
                {
                    AssetReference.Instance.Clear();
                    AssetReference.Instance.BuildDatas();
                    RefreshDataList();
                }
                if(GUILayout.Button("Load", GUILayout.ExpandWidth(false)))
                {
                    AssetReference.Instance.Load();
                    RefreshDataList();
                }
                if(GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
                {
                    AssetReference.Instance.Save();
                }
            }
        }

        using(GUIUtil.LayoutHorizontal())
        {
            bool b = false;
            GUILayout.Label("总计：" + dataList.Count, GUILayout.ExpandWidth(false));

            if(GUILayout.Button("<<", GUILayout.ExpandWidth(false)))
            {
                curPageIndex--;
                if(curPageIndex < 0)
                {
                    curPageIndex = 0;
                }
            }
            curPageIndex = EditorGUIUtil.IntFieldCompact("", curPageIndex, ref b, null, GUILayout.Width(50));
            if(GUILayout.Button(">>", GUILayout.ExpandWidth(false)))
            {
                curPageIndex++;
                var max = GetMaxPageIndex();
                if(curPageIndex > max)
                {
                    curPageIndex = max;
                }
            }

            b = false;
            filter = EditorGUIUtil.TextFieldCompact("筛选：", filter, ref b);
            if(b)
            {
                RefreshDataList();
            }
        }
    }

    void GUIDAssets()
    {
        using (GUIUtil.LayoutHorizontal())
        {
            GUILayout.Space(60);
            GUILayout.Label("路径名", GUILayout.Width(500));
            GUILayout.Label("使用", GUILayout.Width(60));
            GUILayout.Label("被使用", GUILayout.Width(60));
        }
        
        using (GUIUtil.Scroll(ref scrollPos))
        {
            var starIndex = curPageIndex < 0 ? 0 : curPageIndex * PAGE_ASSET_NUM;
            var endIndex = starIndex + PAGE_ASSET_NUM - 1;
            for(int i = starIndex; i < dataList.Count && i <= endIndex; i++)
            {
                var d = dataList[i];
                using(GUIUtil.LayoutHorizontal())
                {
                    GUILayout.Label(i.ToString(), GUILayout.Width(60));
                    GUILayout.Label(d.path, GUILayout.Width(500));
                    GUILayout.Label(d.references.Count.ToString(), GUILayout.Width(60));
                    GUILayout.Label(d.referencesBy.Count.ToString(), GUILayout.Width(60));
                    GUILayout.FlexibleSpace();
                    if(GUILayout.Button(expandIndex == i ? "▼" : "◀", GUILayout.ExpandWidth(false)))
                    {
                        expandIndex = expandIndex == i ? -1 : i;
                    }
                }

                if(expandIndex == i)
                {
                    using (GUIUtil.LayoutVertical(GUI.skin.box))
                    {
                        using(GUIUtil.LayoutHorizontal())
                        {
                            GUILayout.Space(20);
                            GUILayout.Label("使用了：");
                        }
                        for(int j = 0; j < d.references.Count; j++)
                        {
                            using(GUIUtil.LayoutHorizontal())
                            {
                                GUILayout.Space(60);
                                EditorGUILayout.LabelField(d.references[j].path);
                            }
                        }

                        using(GUIUtil.LayoutHorizontal())
                        {
                            GUILayout.Space(20);
                            GUILayout.Label("被谁使用：");
                        }

                        for(int j = 0; j < d.referencesBy.Count; j++)
                        {
                            using(GUIUtil.LayoutHorizontal())
                            {
                                GUILayout.Space(60);
                                EditorGUILayout.LabelField(d.referencesBy[j].path);
                            }
                        }
                    }
                }
            }
        }
    }

    private void RefreshDataList()
    {
        dataList.Clear();
        var list = AssetReference.Instance.GetDatas();
        foreach (var data in list)
        {
            if (string.IsNullOrEmpty(filter) || data.path.Contains(filter))
            {
                dataList.Add(data);
            }
        }
        dataList.Sort((d1, d2) => String.Compare(d1.path, d2.path, StringComparison.OrdinalIgnoreCase));
        expandIndex = -1;
        curPageIndex = 0;
    }

    private int GetMaxPageIndex()
    {
        var n = dataList.Count / PAGE_ASSET_NUM;
        return n * PAGE_ASSET_NUM < dataList.Count ? n : n - 1;
    }
}