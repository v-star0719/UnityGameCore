using System;
using System.Collections.Generic;
using Kernel.Core;
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

        ///a和b比较结果，小于0=a在左边，b在右边
        public static void BinarySearchInsert<T>(List<T> list, T n, Func<T, T, int> compare)
        {
            var left = 0;
            var right = list.Count - 1;
            var mid = 0;
            while(left <= right)
            {
                mid = (left + right) >> 1;
                var midValue = list[mid];
                var c = compare(n, midValue);
                if(c > 0)
                {
                    left = mid + 1;//目标在中间值的右边
                }
                else if(c < 0)
                {
                    right = mid - 1;//目标在中间值的左边
                }
                else
                {
                    break;//命中目标
                }
            }

            if(left <= right)
            {
                list.Insert(mid, n);//会把mid往后顶，但是两个值都一样，先后应该无所谓吧
            }
            else if(left < list.Count)
            {
                //这种情况下最后结束一定是right < left。插入值位于这两个值之间。不管排列方式怎么样，是否位于right或者left，把值插入到这两个中间准没错了。
                list.Insert(left, n);
            }
            else
            {
                //无脑加最后。为什么呢请看下面解答。
                list.Add(n);
            }

            //详细分析
            //哐哐循环没有命中，最后都会到left=right这一步（证明就不需要了吧），如果还没命中：
            //   如果目标在左边，right-1。right < left结束循环。目标在[right] [left]这两个之间
            //   如果目标在右边，left+1。right < left结束循环。目标在[right] [left]这两个之间

            //如果list是空的，left=0，right=-1，循环不会进行。插入到left
            //如果list只有一个元素，left=right=0。
            //  目标等于它，命中插入到mid(0)
            //  目标小于它，也就是位于中间值的左边，right-1=-1，left=0。还是插入到left，会把原来的大的顶到后面。
            //  目标大于它，也就是位于中间值的右边，left+1=1，right=0。【【这时候left超过了数组大小，向后追加。】】
            //如果list元素超过2个，正常执行循环搜索

            //如果目标应插入到列表最左边，最后left=0，right=-1.插入到left就行
            //如果目标应插入到列表最右边，最后left=ListCount，right=ListCount-1。【【left超过了数组大小，向后追加。】】

            //List<int> array = new List<int>(100);
            ////int[] testNumbers = new int[] { 55, 37, 35, 26 };
            //for(int i = 0; i < 100; i++)
            //{
            //    BinarySearchInsert(array, Random.Range(0, 100), (a, b) => a.CompareTo(b));
            //    bool r = true;
            //    for(int i = 0; i < array.Count; i++)
            //    {
            //        for(int j = i + 1; j < array.Count; j++)
            //        {
            //            if(array[i] > array[j])
            //            {
            //                r = false;
            //            }
            //        }
            //    }
            //    Debug.Log(StringUtils.CollectionToString(array) + " " + r);
            //}
        }
    }
}