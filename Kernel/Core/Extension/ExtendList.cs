using System;
using System.Collections.Generic;
using Kernel.Core.Misc;

namespace Kernel.Lang.Extension
{
	public static class ExtendList
	{
		public static T BackEx<T>(this List<T> list)
		{
			if(null != list)
			{
				var count = list.Count;
				if(count > 0)
				{
					var idxLast = count - 1;
					var back = list[idxLast];

					return back;
				}
			}

			return default(T);
		}

		public static int ClampIndexEx<T>(this List<T> list, int index)
		{
			if(list == null || list.Count == 0 || index < 0)
			{
				return 0;
			}
			if(index >= list.Count)
			{
				return list.Count - 1;
			}
			return index;
		}

		public static bool EmptyEx<T>(this List<T> list)
		{
			return null != list && list.Count == 0;
		}

		public static T GetItemEx<T>(this List<T> list, int index)
		{
			if(list != null && index >= 0 && index < list.Count)
			{
				return list[index];
			}
			return default(T);
		}

		public static T LastOrDefaultEx<T>(this List<T> list)
		{
			if(list == null || list.Count == 0)
			{
				return default(T);
			}
			return list[list.Count - 1];
		}

		public static T PopBackEx<T>(this List<T> list)
		{
			if(null != list)
			{
				var count = list.Count;
				if(count > 0)
				{
					var idxLast = count - 1;
					var back = list[idxLast];
					list.RemoveAt(idxLast);

					return back;
				}
			}

			return default(T);
		}

		public static void ReserveEx<T>(this List<T> list, int minCapacity)
		{
			if(null != list)
			{
				var capacity = list.Capacity;

				if(minCapacity > capacity)
				{
					list.Capacity = Math.Max(Math.Max(capacity * 2, 4), minCapacity);
				}
			}
		}

		public static void Resize<T>(this List<T> list, int size, bool createDefaultElem = false)
		{
			if(list == null)
			{
				return;
			}

			var oldLen = list.Count;
			ResizeImpl(list, size, default(T));

			if(createDefaultElem)
			{
				for(var i = oldLen; i < list.Count; i++)
				{
					list[i] = (T) Activator.CreateInstance(typeof(T));
				}
			}
		}

		private static void ResizeImpl<T>(List<T> list, int size, T defaultValue)
		{
			var count = list.Count;
			if(size > count)
			{
				if(list.Capacity < size)
				{
					list.Capacity = size;
				}

				for(var i = count; i < size; ++i)
				{
					list.Add(defaultValue);
				}
			}

			if(size < count)
			{
				list.RemoveRange(size, count - size);
			}
		}

        public static void FastRemove<T>(this IList<T> list, int index)
        {
            var last = list.Count - 1;
            if (index < 0 || index > last)
            {
                return;
            }

            if (last == index)
            {
                list.RemoveAt(index);
            }
            else
            {
                list[index] = list[last];
                list.RemoveAt(last);
            }
        }

        public static T RandomOne<T>(this IList<T> list, T def)
        {
            var n = list.Count;
            if (n == 0)
            {
                return def;
            }

            if (n == 1)
            {
                return list[0];
            }
            else
            {
                return list[RandomUtils.Random.Next(0, n)];
            }
        }

        public static T SafeGet<T>(this IList<T> list, int index, T def)
        {
            if (index < 0 || index >= list.Count)
            {
                return def;
            }
            return list[index];
        }

        /// <summary>
        /// 一个列表里的项移上移下
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">列表</param>
        /// <param name="item">项</param>
        /// <param name="isMoveUp">true = 上移，false = 下移</param>
        public static void MoveUpDown<T>(this List<T> list, T item, bool isMoveUp)
        {
            if (list.Count <= 1)
            {
                return;
            }

            int index = list.IndexOf(item);

            if (index < 0 || index >= list.Count)
            {
                return;
            }

            T t = list[index];
            int targetIndex = 0;
            if (index == 0)
            {
                if (isMoveUp)
                {
                    //不能进行交换，直接插入到最后
                    list.RemoveAt(index);
                    list.Add(t);
                    return;
                }

                targetIndex = 1;
            }
            else if (index == list.Count - 1)
            {

                if (!isMoveUp)
                {
                    //不能进行交换，直接插入到最前
                    list.RemoveAt(index);
                    list.Insert(0, t);
                    return;
                }

                targetIndex = list.Count - 2;
            }
            else
            {
                targetIndex = index + (isMoveUp ? -1 : 1);
            }

            list[index] = list[targetIndex];
            list[targetIndex] = t;
        }

        // randomize the order of elements of a list
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                int r = UnityEngine.Random.Range(0, list.Count);
                T temp = list[i];
                list[i] = list[r];
                list[r] = temp;
            }
        }
	}


}