#if NGUI
namespace GameCore.Unity.NGUIEx
{
    public class GridExTable : GridEx
    {
        public UITable table;

        protected override void Init()
        {
            base.Init();
            table.enabled = false;
            arrangeFunc = Arrange;
        }

        private void Arrange()
        {
            table.Reposition();
        }
    }
}
#endif