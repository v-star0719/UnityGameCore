using System;
using System.Collections;
using System.Collections.Generic;
using Kernel.Edit;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    public class SelectFromListWnd : EditorWindow
    {
        private Action<int> chooseCallback;
        private Action<int> delCallback;
        private Func<int, ISelectFromListData> createCallback;
        private List<ISelectFromListData> dataList;
        private Vector2 scrollPos;
        private int selectId;
        private int filerId;
        private int createId;

        private const int COLUMN = 4;
        private List<ISelectFromListData> delList = new List<ISelectFromListData>();

        public static void Show(int selectId, List<ISelectFromListData> datas, Action<int> chooseCallback, Action<int> delCallabck, Func<int, ISelectFromListData> createCallback)
        {
            var wnd = GetWindow<SelectFromListWnd>("选择");
            wnd.Init(selectId, datas, chooseCallback, delCallabck, createCallback);
        }

        private void Init(int selectId, List<ISelectFromListData> datas, Action<int> chooseCallback, Action<int> delCallabck, Func<int, ISelectFromListData> createCallback)
        {
            dataList = datas;
            this.selectId = selectId;
            this.chooseCallback = chooseCallback;
            this.delCallback = delCallabck;
            this.createCallback = createCallback;
        }

        private void OnGUI()
        {
            //toolbar
            using (GUIUtil.LayoutHorizontal("box"))
            {
                OnGUIToolBar();
            }

            using (GUIUtil.Scroll(ref scrollPos, GUILayout.ExpandHeight(true)))
            {
                OnGUIContent();
            }
        }

        private void OnGUIToolBar()
        {
            GUILayout.Label("筛选：", GUILayout.ExpandWidth(false));
            filerId = EditorGUILayout.IntField(filerId);
            GUILayout.Space(30);

            createId = EditorGUILayout.IntField(createId);
            if (GUILayout.Button("Create", GUILayout.ExpandWidth(false)))
            {
                var d = createCallback?.Invoke((createId));
                if (d != null)
                {
                    dataList.Add(d);
                    dataList.Sort((a, b) => a.Id.CompareTo(b.Id));
                }
            }
        }

        private void OnGUIContent()
        {
            List<ISelectFromListData> list;
            if (filerId <= 0)
            {
                list = dataList;
            }
            else
            {
                list = new List<ISelectFromListData>();
                foreach (var data in dataList)
                {
                    var id = data.Id;
                    while (id > 0 && id != filerId)
                    {
                        id /= 10;
                    }

                    if (id != 0)
                    {
                        list.Add(data);
                    }
                }
            }

            //划分成n列
            var lines = list.Count / COLUMN;
            if (list.Count % COLUMN > 0)
            {
                lines++;
            }

            for (int i = 0; i < lines; i++)
            {
                using (GUIUtil.LayoutHorizontal())
                {
                    OnGUIList(list, i * COLUMN, (i + 1) * COLUMN - 1);
                }
            }
        }

        private void OnGUIList(List<ISelectFromListData> list, int start, int end)
        {
            for (int i = start; i <= end && i < list.Count; i++)
            {
                var data = list[i];
                IDisposable dis = null;
                if (data.Id == selectId)
                {
                    dis = GUIUtil.Color(Color.green);
                }

                var align = GUI.skin.button.alignment;
                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                if (GUILayout.Button(data.ToString(), GUILayout.Width(150)))
                {
                    selectId = list[i].Id;
                    chooseCallback(selectId);
                    Close();
                }

                GUI.skin.button.alignment = align;

                dis?.Dispose();

                using (GUIUtil.Color(Color.red))
                {
                    if (GUILayout.Button("x", GUILayout.ExpandWidth(false)))
                    {
                        if (EditorUtility.DisplayDialog("警告", "确定要删除吗?", "确定", "取消"))
                        {
                            delList.Add(data);
                            delCallback?.Invoke(data.Id);
                        }
                    }
                }
            }

            foreach (var d in delList)
            {
                dataList.Remove(d);
            }

            delList.Clear();
        }
    }
}