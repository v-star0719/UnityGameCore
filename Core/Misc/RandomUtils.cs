using System;
using System.Collections.Generic;

namespace Kernel.Core.Misc
{
    public class RandomUtils
    {
        public static Random Random { get; private set;} = new Random();

        ///为减少计算，权重列表是每个元素的累积权重，构建权重列表的时候顺手算出来就行了。
        ///比如每个元素权重是1,10个元素，那么权重列表值为1,2,3,4,5,6,7,8,9,10
        public static T RandomFromListWithWeight<T>(List<T> list, List<int> weightList)
        {
            if(weightList == null)
            {
                return list[Random.Next(0, list.Count)];
            }
            else
            {
                var r = Random.Next(0, weightList[^1]);
                for(int i = 0; i < weightList.Count; i++)
                {
                    if(r < weightList[i])
                    {
                        return list[i];
                    }
                }
                throw new Exception("random list or weight list is wrong");
            }
        }
    }
}
