using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace GameCore.Storage
{
    public class StorageXmlSerializer : IStorageSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            try
            {
                using var ms = new MemoryStream();
                var xz = new System.Xml.Serialization.XmlSerializer(typeof(T));
                xz.Serialize(ms, data);
                return ms.ToArray();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public T Deserialize<T>(byte[] bytes) where T : new()
        {
            if(bytes == null)
            {
                return new T();
            }

            try
            {
                using var fs = new MemoryStream(bytes);
                using var sr = new StreamReader(fs, Encoding.UTF8);
                var xz = new System.Xml.Serialization.XmlSerializer(typeof(T));
                return (T)xz.Deserialize(sr);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new T();
            }
        }
    }
}
