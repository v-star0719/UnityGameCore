using System.Collections.Generic;

namespace GameCore.Core
{
    public interface IConfCollection<T> where T : ConfBase
    {
        void Load();
        void Save();
        void Add(T t);
        void Remove(int id);
        bool Contains(int id);
        T Get(int id);
        T Create(int id);
        List<T> GetConfs();
    }
}
