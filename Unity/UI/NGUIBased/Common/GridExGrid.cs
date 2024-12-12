using UnityEngine;

#if NGUI
namespace GameCore.Unity.NGUIEx
{
    public class GridExGrid : GridEx
    {
        public UIGrid grid;

        protected override void Init()
        {
            base.Init();
            grid.enabled = false;
            arrangeFunc = Arrange;
        }

        private void Arrange()
        {
            grid.Reposition();
        }

        //如果数量比较少，就居中显示；数量比较多久居左显示
        public void AutoAlignCenterOrLeft()
        {
            if (scrollView == null)
            {
                Debug.LogError("AutoAlignCenterOrLeft. scrollview is required");
                return;
            }

            var region = scrollView.panel.finalClipRegion;
            var w = region.z;
            var cellWidth = this.grid.cellWidth;
            var n = w / cellWidth;
            var itemCount = DataCount;
            if (itemCount <= n)
            {
                grid.transform.localPosition = Vector3.zero;
            }
            else
            {
                grid.transform.localPosition = new Vector3((cellWidth * itemCount - w) * 0.5f, 0, 0);
            }
        }
    }
}
#endif