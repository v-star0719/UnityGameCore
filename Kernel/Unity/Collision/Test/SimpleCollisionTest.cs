using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Unity
{
    public class SimpleCollisionTest : MonoBehaviour
    {
        public float leaveSpeed = 1;
        public float trackingTargetRadius = 0.5f;
        public SimpleCollision simpleCollision;
        public List<ICollisionObject> objList = new List<ICollisionObject>();

        // Use this for initialization
        void Start()
        {
            simpleCollision = new SimpleCollision(leaveSpeed, objList, null);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            simpleCollision.Tick(Time.deltaTime);
        }
    }
}