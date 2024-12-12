using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    public class SimpleCollisionTestObj : MonoBehaviour, ICollisionObject
    {
        public SimpleCollisionTest collisionTest;
        public Transform trackingTarget;
        public float moveSpeed = 1;
        public float collisionRadius = 0.5f;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public float CollisionRadius
        {
            get => collisionRadius;
            set => collisionRadius = value;
        }

        public int CheckedFrame { get; set; }

        // Use this for initialization
        void Start()
        {
            collisionTest.objList.Add(this);
        }

        // Update is called once per frame
        void Update()
        {
            var dir = trackingTarget.position - transform.position;
            var delta = moveSpeed * Time.deltaTime;
            var dist = dir.magnitude - CollisionRadius - collisionTest.trackingTargetRadius;
            if (delta >= dist)
            {
                delta = dist;
            }
            transform.position += dir.normalized * delta;
        }

        void OnDestroy()
        {
            collisionTest.objList.Remove(this);
        }
    }
}