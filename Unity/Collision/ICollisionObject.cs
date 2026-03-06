using UnityEngine;

namespace GameCore.Unity.Collision
{
    public interface ICollisionObject
    {
        Vector3 Position { get; set; }
        float CollisionRadius { get;}
        int CheckedFrame { get; set; }
    }
}