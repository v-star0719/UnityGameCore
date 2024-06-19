using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Unity
{
    //将碰撞体按球处理，当有重合时互相挤开。
    //抖动：
    //碰撞挤开处理时的抖动问题。移动后A和B重合，A和B挤开，这样A和B的位置就发生了变化。
    //当A和B都围绕贴着目标时，每帧都会进行这种位置变化，看起来就是在细微的抖。如果挤开速度比较快的话，抖动就会很明显。
    //重叠：
    //当前的处理机制是非发生重叠的，因为挤开速度比重叠速度慢，挤开速度增大时，重叠会变少，但是会出现抖动。
    //如何避免重叠是一个可以优化的问题。但是用在怪物追踪敌人时，敌人重叠到一起也没有太大关系。敌人最终会围绕目标慢慢散开。
    //完全重叠：
    //当对象完全重叠时，因为方向算出来是(0,0,0)，会导致原地不动
    public class SimpleCollision
    {
        public float leaveSpeed = 1;
        public float moveSpeed;
        public List<ICollisionObject> objList;
        private Action<ICollisionObject, List<ICollisionObject>> getCollisionFunc;
        private int frameCount;
        private List<ICollisionObject> collisions = new List<ICollisionObject>();

        public SimpleCollision(float leaveSpeed, List<ICollisionObject> objList, Action<ICollisionObject, List<ICollisionObject>> getCollisionFunc)
        {
            this.leaveSpeed = leaveSpeed;
            this.objList = objList;
            this.getCollisionFunc = getCollisionFunc ?? GetCollisionsByDefault;
        }

        public void Tick(float deltaTime)
        {
            frameCount++;
            for (int i = 0; i < objList.Count; i++)
            {
                var a = objList[i];
                a.CheckedFrame = frameCount;//这样可以跳过检测a

                getCollisionFunc(a, collisions);
                foreach (var b in collisions)
                {
                    var dir = a.Position - b.Position;
                    dir.y = 0;
                    var intersection = a.CollisionRadius + b.CollisionRadius - dir.magnitude;
                    var delta = leaveSpeed * deltaTime;
                    if (delta * 2 > intersection)
                    {
                        delta = intersection * 0.5f;
                    }
                    var deltaV3 = delta * dir.normalized;
                    a.Position += deltaV3;
                    b.Position -= deltaV3;
                }
            }
        }

        private void GetCollisionsByDefault(ICollisionObject obj, List<ICollisionObject> outList)
        {
            outList.Clear();
            foreach (var target in objList)
            {
                if (target.CheckedFrame == frameCount)
                {
                    continue;;
                }

                var dir = obj.Position - target.Position;
                dir.y = 0;
                var t = obj.CollisionRadius + target.CollisionRadius;
                if (dir.sqrMagnitude < t * t)
                {
                    outList.Add(target);
                }
            }
        }
    }
}