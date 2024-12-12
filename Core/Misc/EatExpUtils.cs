using System;

namespace GameCore.Core
{
    public class EatExpUtils
    {
        //getUpgradeNeedExp: 获取当前等级升级需要的经验
        public static void Add(int expItemCount, int expPerItem, ref int lv, ref int exp, int maxLv, Func<int, int> getUpgradeNeedExp,
            out int usedItemCount)
        {
            usedItemCount = 0;
            for (int i = 0; i < expItemCount; i++)
            {
                usedItemCount++;
                exp += expPerItem;
                var needExp = getUpgradeNeedExp(lv);
                while (exp >= needExp)
                {
                    exp -= needExp;
                    lv++;

                    if (lv == maxLv)
                    {
                        return;
                    }
                    needExp = getUpgradeNeedExp(lv);
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
    }
}
