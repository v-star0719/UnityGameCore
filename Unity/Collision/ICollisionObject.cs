using UnityEngine;

namespace GameCore.Unity
{
    public interface ICollisionObject
    {
        Vector3 Position { get; set; }
        float CollisionRadius { get;}
        int CheckedFrame { get; set; }
    }
}