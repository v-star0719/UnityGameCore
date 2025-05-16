using System;

namespace GameCore.Core
{
    public class EatExpUtils
    {
        //getUpgradeNeedExp: 获取当前等级升级需要的经验
        public static void Add(int expItemCount, int expPerItem, ref int curLv, ref int curExp, int maxLv, Func<int, int> getUpgradeNeedExp,
            out int usedItemCount)
        {
            usedItemCount = 0;
            for (int i = 0; i < expItemCount; i++)
            {
                usedItemCount++;
                curExp += expPerItem;
                var needExp = getUpgradeNeedExp(curLv);
                while (curExp >= needExp)
                {
                    curExp -= needExp;
                    curLv++;

                    if (curLv == maxLv)
                    {
                        return;
                    }
                    needExp = getUpgradeNeedExp(curLv);
                }
            }
        }

        //减到道具使用数量为0
        //getUpgradeNeedExp: 获取当前等级升级需要的经验
        public static void Sub(int expItemCount, int expPerItem, ref int lv, ref int exp, Func<int, int> getUpgradeNeedExp,
            out int returnItemCount)
        {
            returnItemCount = 0;
            for (int i = 0; i < expItemCount; i++)
            {
                returnItemCount++;
                exp -= expPerItem;
                while (exp < 0)//如果exp等于0，则刚好停留在当前这一级
                {
                    lv--;
                    exp += getUpgradeNeedExp(lv);
                }
            }
        }

        public static int GetExpPropCount(int exp, int expPerProp)
        {
            var n = exp / expPerProp;
            if (exp > n * expPerProp)
            {
                n++;
            }
            return n;
        }
    }
}
