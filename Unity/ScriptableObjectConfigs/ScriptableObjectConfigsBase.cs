using System.Collections.Generic;
using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity
{
    //[CreateAssetMenu(fileName = "Tigs", menuName = "ScriptableObjects/Tigs", order = 1)]
    public class ScriptableObjectConfigsBase<T> : ScriptableObject where T : ConfBase, new()
    {
        [SerializeField]
        private List<T> list = new List<T>();

        private Dictionary<int, T> dict = new Dictionary<int, T>();

        public List<T> All => list;

        void Awake()
        {
        }

        void OnEnable()
        {
            if (dict.Count == 0)
            {
                foreach (var data in list)
                {
                    if (dict.ContainsKey(data.id))
                    {
                        Debug.LogError($"重复的配置：{data.id}");
                    }
                    else
                    {
                        dict.Add(data.id, data);
                    }
                }
            }
        }

        public T Get(int gid)
        {
            T d;
            if (dict.TryGetValue(gid, out d))
            {
                return d;
            }

            return d;
        }

        public T Create(int id)
        {
            if (dict.ContainsKey(id))
            {
                Debug.LogError($"{id} 配置已存在");
                return null;
            }

            var d = new T();
            d.id = id;
            dict.Add(id, d);
            list.Add(d);
            return d;
        }

        public void Remove(int id)
        {
            if (dict.Remove(id, out var data))
            {
                list.Remove(data);
            }
        }
    }
}