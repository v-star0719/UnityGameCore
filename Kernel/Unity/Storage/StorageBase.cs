//#define STORAGE_DEBUG

using System;
using System.Diagnostics;
using System.IO;
using Kernel.Storage;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Kernel.Storage
{
    public interface IStorage
    {
        public bool Changed { get; }
        public void Save();
        public void Load();
        public void Init();
        public void Clear();
    }

    //默认是存文件，如果要存别的地方，请重载OnLoad和OnSave
    public class StorageBase<T> : IStorage where T : new()
    {
        public string FileName { get; protected set; }//请初始化
        //UnityException: get_persistentDataPath can only be called from the main thread.
        public string FilePath { get; protected set; }//
        private volatile bool changed;
        protected IStorageSerializer serializer;
        protected T data;

        public bool Changed
        {
            get => changed;
            protected set
            {
                changed = value;
                //if (value)
                //{
                //    Debug.Log("true");
                //}
            }
        }

        public StorageBase(IStorageSerializer serializer, string fileName)
        {
            this.serializer = serializer;
            FileName = fileName;
        }

        protected virtual void InitFilePath()
        {
            FilePath = $"{Application.persistentDataPath}/{FileName}";
        }

        public virtual void Clear()
        {
        }

        public virtual void Init()
        {
            OnInit();
            InitFilePath();
        }

        public virtual void Load()
        {
            var path = FilePath;
            byte[] bytes = null;

            try
            {
                if (File.Exists(path))
                {
                    bytes = File.ReadAllBytes(path);
                }
#if !UNITY_EDITOR
                Decode(bytes);
#endif
                Log($"load data from {path}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                data = serializer.Deserialize<T>(bytes);
                OnLoad();
            }
        }

        public virtual void Save()
        {
            if (!Changed)
            {
                return;
            }
            Changed = false;
            OnSave();

            var bytes = serializer.Serialize(data);
            if (bytes == null)
            {
                return;
            }

            try
            {
#if !UNITY_EDITOR
                Encode(bytes);
#endif
                File.WriteAllBytes(FilePath, bytes);
                Log($"save data to {FilePath}");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        protected virtual void OnInit()
        {
        }

        //读取后调用
        protected virtual void OnLoad()
        {
        }

        //保存前调用
        protected virtual void OnSave()
        {
        }

        [Conditional("STORAGE_DEBUG")]
        protected void Log(string text)
        {
            Debug.Log(text);
        }

        protected virtual void Encode(byte[] bytes)
        {
            var key = (byte)bytes.Length;
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= key;
            }
        }

        protected virtual void Decode(byte[] bytes)
        {
            var key = (byte)bytes.Length;
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= key;
            }
        }
    }
}
