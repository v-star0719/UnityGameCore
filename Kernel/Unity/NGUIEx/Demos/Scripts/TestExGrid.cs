using UnityEngine;
using System.Collections;

namespace Kernel.Unity.NguiEx
{
    public class TestExGrid : MonoBehaviour
    {
#if NGUI
        public UIExGrid horzExGrid;
        public int horzItemCount = 5;
        public UIExGrid vertExGrid;

        public int vertItemCount = 5;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        public bool testBtn;

        void Update()
        {
            if (testBtn)
            {
                testBtn = false;
                vertExGrid.SetGrid(vertItemCount, null);
                horzExGrid.SetGrid(horzItemCount, null);
            }
        }
#endif
    }
}
