using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCore.Core;
using GameCore.Edit;
using GameCore.Unity;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameCore.Unity
{
    public partial class BatchRenameWnd : EditorWindowBase
    {
        public class RenameInfo
        {
            public string path;
            public string name;
            public string newName;
        }

        public EditDataStringField replaceSource;
        public EditDataStringField replaceTarget;
        public EditDataStringField format;

        private Vector2 scrollPos;
        private List<RenameInfo> infos = new();

        [MenuItem("Tools/Misc/BatchRenameWnd")]
        public static void Open()
        {
            GetWindow<BatchRenameWnd>("BatchRenameWnd");
        }

        protected override void InitEditDataFields()
        {
            base.InitEditDataFields();
            replaceSource = new("BatchRenameWnd.replaceSource", "", this);
            replaceTarget = new("BatchRenameWnd.replaceTarget", "", this);
        }

        public void OnGUI()
        {
            using (GUIUtil.LayoutHorizontal())
            {
                replaceSource.Value = EditorGUILayout.TextField(replaceSource.Value);
                GUILayout.Label("==>", GUILayout.ExpandWidth(false));
                replaceTarget.Value = EditorGUILayout.TextField(replaceTarget.Value);
            }

            using (GUIUtil.LayoutHorizontal())
            {
                if (GUILayout.Button("Preview", GUILayout.ExpandWidth(false)))
                {
                    infos.Clear();
                    foreach(var guid in Selection.assetGUIDs)
                    {
                        var info = new RenameInfo();
                        info.path = AssetDatabase.GUIDToAssetPath(guid);
                        info.name = Path.GetFileNameWithoutExtension(info.path);
                        info.newName = info.name.Replace(replaceSource.Value, replaceTarget.Value);
                        infos.Add(info);
                    }
                }
                if (GUILayout.Button("Confirm", GUILayout.ExpandWidth(false)))
                {
                    foreach (var info in infos)
                    {
                        AssetDatabase.RenameAsset(info.path, info.newName);
                    }
                }
            }

            using (GUIUtil.Scroll(ref scrollPos))
            {
                for (var i = 0; i < infos.Count; i++)
                {
                    var info = infos[i];
                    GUILayout.Label($"[{i+1}] {info.name} ==> {info.newName}");
                }
            }
        }
    }
}