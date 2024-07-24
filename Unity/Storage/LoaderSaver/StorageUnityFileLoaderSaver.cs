using System.Collections;
using System.IO;
using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity
{
    public class StorageUnityFileLoaderSaver : IStorageLoaderSaver
    {
        private string filePath;

        public StorageUnityFileLoaderSaver(long userId, string fileName)
        {
            filePath = $"{Application.persistentDataPath}/{userId}_{fileName}";
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
            File.WriteAllBytes(filePath, data);
        }
    }
}