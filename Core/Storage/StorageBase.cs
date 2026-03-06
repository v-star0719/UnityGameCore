//打印存储相关的日志
//Print logs
//#define GAME_CORE_STORAGE_DEBUG

//加密存储数据
//Encrypt data
//#define GAME_CORE_STORAGE_ENCRYPT

using System;
using System.Diagnostics;
using GameCore.Core.Logger;

namespace GameCore.Core.Storage
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
    //File storage is used by default. If you need to store data elsewhere, please override the OnLoad and OnSave methods.
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
#if GAME_CORE_STORAGE_ENCRYPT
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
#if GAME_CORE_STORAGE_ENCRYPT
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

        //加载数据前调用
        //Call before loading data.
        protected virtual void OnBeforeLoad()
        {
        }

        //加载数据后调用
        //Call after loading data.
        protected virtual void OnAfterLoad()
        {
        }

        //保存数据前调用
        //Call before saving data.
        protected virtual void OnBeforeSave()
        {
        }

        //保存数据后调用
        //Call after saving data.
        protected virtual void OnAfterSave()
        {
        }

        [Conditional("GAME_CORE_STORAGE_DEBUG")]
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
            if (bytes == null)
            {
                return;
            }
            var key = (byte)bytes.Length;
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] ^= key;
            }
        }
    }
}
