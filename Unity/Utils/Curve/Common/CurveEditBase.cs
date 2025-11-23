using System.Collections.Generic;
using UnityEngine;

namespace Curves
{
    public class CurveEditBase<T> : MonoBehaviour where T : CurveData
    {
        public T data;
        public T workingData;
        public List<Vector3> posList = new List<Vector3>();
        public int smooth = 1;
        public float markSize = 0.3f;

        void Start()
        {
        }

        void Update()
        {
            if (workingData == null)
            {
                if (data != null)
                {
                    Load();
                }
                return;
            }

            if (workingData != data)
            {
                Clear();
                return;
            }

            //更新点列表
            posList.Clear();
            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i);
                posList.Add(t.localPosition);
                var mark = t.GetComponent<CurvePointMark>();
                if (mark == null)
                {
                    mark = t.gameObject.AddComponent<CurvePointMark>();
                }

                mark.size = markSize;
                OnSetMark(i, mark);
            }
            OnUpdate();
        }

        public void Load()
        {
            if (data == null)
            {
                return;
            }
            workingData = data;
            posList.AddRange(data.points);
            smooth = data.smooth;
            InitPointMarks();
            OnLoad();
        }

        public void Save()
        {
            if (data == null)
            {
                return;
            }

            data.points.Clear();
            data.points.AddRange(posList);
            data.smooth = smooth;
            OnSave();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(data);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        protected virtual void OnDrawGizmos()
        {
            data?.DrawGizmos(Color.white, transform.localToWorldMatrix, posList, smooth);
        }

        protected virtual void OnUpdate()
        {
        }

        protected virtual void OnSetMark(int index, CurvePointMark mark)
        {
            mark.name = index.ToString();
        }

        protected virtual void OnLoad()
        {
        }

        protected virtual void OnSave()
        {
        }

        public void InitPointMarks()
        {
            for (int i = 0; i < posList.Count; i++)
            {
                Transform trans;
                GameObject go;
                var m = i >> 1;
                if (i < transform.childCount)
                {
                    trans = transform.GetChild(i);
                    go = trans.gameObject;
                }
                else
                {
                    go = new GameObject();
                    trans = go.transform;
                    trans.parent = transform;
                    trans.localScale = Vector3.one;
                }

                go.transform.localPosition = posList[i];
                go.SetActive(true);
            }

            for (int i = transform.childCount - 1; i >= posList.Count; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(transform.GetChild(i).gameObject);
#else
                Destroy(transform.GetChild(i).gameObject);
#endif
            }
        }

        public void Clear()
        {
            workingData = null;
            data = null;
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(transform.GetChild(i).gameObject);
#else
                Destroy(transform.GetChild(i).gameObject);
#endif
            }
        }
    }
}