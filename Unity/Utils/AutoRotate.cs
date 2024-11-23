using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    public class AutoRotate : MonoBehaviour
    {
        public float angle;
        public Vector3 axis;
        public Space space;
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(axis, angle * Time.deltaTime, space);
        }
    }
}