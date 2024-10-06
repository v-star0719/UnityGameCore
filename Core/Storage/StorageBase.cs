//#define STORAGE_DEBUG
//#define STORAGE_ENCRYPT

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using GameCore.Core;
using Kernel.Core;
using Kernel.Storage;

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
        private volatile bool changed;
        protected IStorageSerializer serializer;
        protected IStorageLoaderSaver loaderSaver;
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

        public StorageBase(IStorageSerializer serializer, IStorageLoaderSaver loaderSaver)
        {
            this.serializer = serializer;
            this.loaderSaver = loaderSaver;
        }

        public virtual void Clear()
        {
        }

        public virtual void Init()
        {
            OnInit();
        }

        public virtual void Load()
        {
            byte[] bytes = null;
            OnBeforeLoad();

            try
            {
                bytes = loaderSaver.Load();
#if !UNITY_EDITOR || STORAGE_ENCRYPT
                Decode(bytes);
#endif
            }
            catch (Exception e)
            {
                LoggerX.Error(e.ToString());
            }
            finally
            {
                data = serializer.Deserialize<T>(bytes);
                OnAfterLoad();
            }
        }

        public virtual void Save()
        {
            if (!Changed)
            {
                return;
            }

            Changed = false;
            OnBeforeSave();

            var bytes = serializer.Serialize(data);
            if (bytes == null)
            {
                return;
            }

            try
            {
#if !UNITY_EDITOR || STORAGE_ENCRYPT
                Encode(bytes);
#endif
                loaderSaver.Save(bytes);
                OnAfterSave();
            }
            catch (Exception e)
            {
                LoggerX.Error(e.ToString());
            }
        }

        protected virtual void OnInit()
        {
        }

        //读取后调用
        protected virtual void OnBeforeLoad()
        {
        }

        //读取后调用
        protected virtual void OnAfterLoad()
        {
        }

        //保存前调用
        protected virtual void OnBeforeSave()
        {
        }
        
        //保存前调用
        protected virtual void OnAfterSave()
        {
        }

        [Conditional("STORAGE_DEBUG")]
        protected void Log(string text)
        {
            LoggerX.Info(text);
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
