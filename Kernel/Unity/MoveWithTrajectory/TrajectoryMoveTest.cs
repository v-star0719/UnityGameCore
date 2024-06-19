using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kernel.Unity
{
    public class TrajectoryMoveTest : MonoBehaviour
    {
        public Transform start;
        public Transform target;
        public TrajectoryMove trajectoryMove;
        public float speed;

        void Start()
        {
        }

        void Update()
        {
            if (trajectoryMove.IsArrived)
            {
                trajectoryMove.StartFly(start.position, target, speed);
            }
        }

        void OnGUI()
        {
            if (GUILayout.Button("Start", GUILayout.Width(100)))
            {
                trajectoryMove.StartFly(start.position, target, speed);
            }
            if (GUILayout.Button("Stop", GUILayout.Width(100)))
            {
                trajectoryMove.enabled = false;
            }
        }
    }
}
