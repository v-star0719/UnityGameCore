using UnityEngine;

namespace GameCore.Unity.NGUIEx
{
    public class TestCollapsibleList : MonoBehaviour
    {
#if NGUI
        public UICollapsibleListCtrl listCtrl;

        public int itemCount = 3;

        // Use this for initialization
        void Start()
        {
            listCtrl.Set(itemCount, null, 1000);
        }

        // Update is called once per frame
        void Update()
        {

        }
#endif
    }
}