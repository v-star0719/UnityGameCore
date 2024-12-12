using System;
using System.Collections.Generic;

namespace GameCore.Core
{
    public class ConfCollection<TMgr, TData> : IConfCollection<TData> where TData : ConfBase where TMgr : new()
    {
        public List<TData> List { get; private set; }
        public Dictionary<int, TData> Dict { get; private set; }
        public string FileName { get; protected set; }
        private Func<string, List<TData>> loader;
        private Action<string, List<TData>> saver;

        public ConfCollection(string fileName, Func<string, List<TData>> loader, Action<string, List<TData>> saver)
        {
            FileName = fileName;
            List = new List<TData>();
            Dict = new Dictionary<int, TData>();
            this.loader = loader;
            this.saver = saver;
            Load();
        }

        public void Load()
        {
            List = loader(FileName);
            foreach (var conf in List)
            {
                if (Dict.ContainsKey(conf.id))
                {
                    LoggerX.Error($"{FileName} 表id重复：{conf.id}");
                }
                else
                {
                    Dict.Add(conf.id, conf);
                }
            }
        }

        public void Save()
        {
            saver(FileName, List);
        }

        public void Add(TData t)
        {
            List.Add(t);
            List.Sort((a, b) => a.id.CompareTo(b.id));
            Dict.Add(t.id, t);
        }

        public void Remove(int id)
        {
            TData s;
            if (Dict.TryGetValue(id, out s))
            {
                Dict.Remove(id);
                List.Remove(s);
            }
        }

        public bool Contains(int id)
        {
            return Dict.ContainsKey(id);
        }

        public TData Get(int id)
        {
            TData rt;
            if (Dict.TryGetValue(id, out rt))
            {
                return rt;
            }

            LoggerX.Error($"{FileName} 配置找不到: {id}");
            return null;
        }

        public List<TData> GetConfs()
        {
            return List;
        }

        public virtual TData Create(int id)
        {
            var rt = Activator.CreateInstance<TData>();
            rt.id = id;
            return rt;
        }
    }

}