using Kernel.Storage;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    public class EditorWindowBase : EditorWindow
    {
        protected float refreshInterval = 0.2f;
        protected float refreshTimer;

        //编辑数据
        protected float dataSaveCheckIntarval = 5f;
        private float dataSaveTimer;
        protected internal EditDataFieldBundle editDataFields;//直接存取EditorPrefs，只支持基本类型
        protected IStorage storage;

        //代码重新编译或者unity play的时候，会调用OnEnable。说明对象被清理了，重新加载配置
        protected virtual void OnEnable()
        {
            if (editDataFields == null)
            {
                LoadData();
            }
        }

        protected virtual void LoadData()
        {
            storage?.Load();
            editDataFields?.Load();
        }

        protected virtual void SaveData()
        {
            storage?.Save();
            editDataFields?.Save();
        }

        protected virtual void Update()
        {
            refreshTimer += Time.deltaTime;
            if (refreshTimer > refreshInterval)
            {
                refreshTimer = 0;
                Repaint();
            }

            if (editDataFields != null || storage != null)
            {
                dataSaveTimer += Time.deltaTime;
                if (dataSaveTimer > dataSaveCheckIntarval)
                {
                    if (editDataFields != null && editDataFields.IsChanged())
                    {
                        editDataFields.Save();
                    }
                    if (storage != null && storage.Changed)
                    {
                        storage.Save();
                    }
                    dataSaveTimer -= dataSaveCheckIntarval;
                }
            }
        }
    }
}
