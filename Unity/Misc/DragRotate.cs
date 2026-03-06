using UnityEngine;

namespace GameCore.Unity.Misc
{
    public class DragRotate : MonoBehaviour
    {
        public Transform rotateObject;
        public float dragRotateFactor = 1f;

        private float curAngle;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnDrag(Vector2 delta)
        {
            curAngle += delta.x * dragRotateFactor;
            rotateObject.localEulerAngles = new Vector3(0, curAngle, 0);
        }
    }
}
