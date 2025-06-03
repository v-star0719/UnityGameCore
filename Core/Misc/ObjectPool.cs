using System;
using System.Collections.Generic;

namespace GameCore.Core
{
    public class ObjectPool<T>
    {
        private Func<T> createFunc;
        private Action<T> onGet;
        private Action<T> onRecycle;
        private Action<T> onClear;
        private List<T> caches;

        public ObjectPool(Func<T> createFunc, Action<T> onGet, Action<T> onRecycle, Action<T> onClear)
        {
            this.createFunc = createFunc;
            this.onGet = onGet;
            this.onRecycle = onRecycle;
            this.onClear = onClear;
            caches = new List<T>();
        }

        public T Get()
        {
            T rt;
            if (caches.Count > 0)
            {
                rt = caches[^1];
                caches.RemoveAt(caches.Count - 1);
            }
            else
            {
                rt = createFunc();
            }
            onGet?.Invoke(rt);
            return rt;
        }

        public void Recycle(T t)
        {
            if (caches.Contains(t))
            {
                LoggerX.Error("object is recycled");
                return;
            }

            caches.Add(t);
            onRecycle?.Invoke(t);
        }

        public void Clear()
        {
            if (onClear != null)
            {
                foreach(var c in caches)
                {
                    onClear(c);
                }
            }
            caches.Clear();
        }
    }
}