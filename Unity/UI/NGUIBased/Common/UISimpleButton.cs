#if NGUI
namespace GameCore.Unity.NGUIEx
{
    public class UISimpleButton : MonoBehaviour
    {
        public List<EventDelegate> onClick = new List<EventDelegate>();
        protected void Start()
        {
        }

        protected void OnClick()
        {
            EventDelegate.Execute(onClick);
        }
    }
}
#endif
