using System;
using System.Text;
using UnityEngine;

namespace Kernel.Storage
{
    public class UnityJsonSerializer : IStorageSerializer
    {
        public byte[] Serialize<T>(T data)
        {
            try
            {
                var text = JsonUtility.ToJson(data);
                return Encoding.UTF8.GetBytes(text);
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

            var text = Encoding.UTF8.GetString(bytes);
            try
            {
                return JsonUtility.FromJson<T>(text);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return new T();
            }
        }
    }
}
