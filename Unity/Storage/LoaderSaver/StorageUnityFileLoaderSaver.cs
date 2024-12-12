using System;
using System.Collections;
using System.IO;
using System.Threading;
using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity
{
    public class StorageUnityFileLoaderSaver : IStorageLoaderSaver
    {
        private string filePath;
        private bool useSaveThread;
        private Mutex saveMutex = new Mutex();

        public StorageUnityFileLoaderSaver(long userId, string fileName, bool useSaveThread)
        {
            filePath = $"{Application.persistentDataPath}/{userId}_{fileName}";
            this.useSaveThread = useSaveThread;
        }

        public byte[] Load()
        {
            if(File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            return null;
        }

        public void Save(byte[] data)
        {
            if (useSaveThread)
            {
                var thread = new Thread(SaveThread);
                thread.Start(data);
            }
            else
            {
                File.WriteAllBytes(filePath, data);
            }
        }

        private void SaveThread(object obj)
        {
            saveMutex.WaitOne();
            try
            {
                File.WriteAllBytes(filePath, obj as byte[]);
            }
            catch (Exception e)
            {
                LoggerX.Error(e.Message);
            }
            saveMutex.ReleaseMutex();
        }
    }
}