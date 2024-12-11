using System.Collections.Generic;
using UnityEngine;

#if NGUI
namespace UI
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
