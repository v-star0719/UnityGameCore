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
        //数据存储机制1：直接存EditorPrefs，只支持基本类型
        public EditDataFieldBundle EditDataFields { get; private set; }
        //数据存储机制2：使用IStorage，支持复杂类型
        protected IStorage storage;

        //代码重新编译或者unity play的时候，会调用OnEnable。说明对象被清理了，重新加载配置
        protected virtual void OnEnable()
        {
            InitEditDataFields();
            InitStorage();
            LoadData();
        }

        protected virtual void InitEditDataFields()
        {
            EditDataFields ??= new EditDataFieldBundle();
            //Init edit data fields here
            //a = new EditDataFloatField("a", 0, this);
        }

        protected virtual void InitStorage()
        {
            //Init storage here
            //storage = new StorageBase<T>("MyStorage");
        }

        protected virtual void LoadData()
        {
            storage?.Load();
            EditDataFields?.Load();
        }

        protected virtual void SaveData()
        {
            storage?.Save();
            EditDataFields?.Save();
        }

        protected virtual void Update()
        {
            refreshTimer += Time.deltaTime;
            if (refreshTimer > refreshInterval)
            {
                refreshTimer = 0;
                Repaint();
            }

            if (EditDataFields != null || storage != null)
            {
                dataSaveTimer += Time.deltaTime;
                if (dataSaveTimer > dataSaveCheckIntarval)
                {
                    if (EditDataFields != null && EditDataFields.IsChanged())
                    {
                        EditDataFields.Save();
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
