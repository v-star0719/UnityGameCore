using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{
    //（1）点击弹出，点击屏幕其他任意位置关闭
    //当点击弹窗本体关闭时，不会触发底部的点击，非常安全。点击弹窗本体不关闭。
    //
    //（2）自动放置在指定的位置周围，并确保UI在屏幕内
    public class TipMessageBoxBase : UIPanelBase, IPointerClickHandler
    {
        //点击屏幕其他任意位置关闭
        [HideInInspector]
        public GameObject trigger;//点在trigger上不会触发关闭
        public Argument arg;
        public RectTransform content;
        private static int lastTipCloseFrame = 0;

        //自动放置在指定的位置周围
        private bool isPlaced;//是否处理了
        private CanvasGroup canvasGroup;
        private bool waitPlace;//有时候需要等界面排列完毕

        protected override void OnOpen(params object[] args)
        {
            if(args.Length <= 0)
            {
                Debug.LogError("TipMessageBoxBaseArg is required");
                return;
            }

            arg = args[0] as Argument;
            trigger = arg.trigger;
        }

        public virtual void Start()
        {
            if(arg.doNotAutoPlace)
            {
                SetPosition(arg.worldPos);
                isPlaced = true;
            }
            else
            {
                canvasGroup = content.GetComponent<CanvasGroup>();
                if(canvasGroup == null)
                {
                    Debug.LogError("CanvasGroup is required to fade in");
                    isPlaced = true;
                    return;
                }
                canvasGroup.alpha = 0;
            }
        }

        public void Update()
        {
            if(!isPlaced)
            {
                if(!waitPlace)
                {
                    canvasGroup.alpha = 1;
                    UGUIUtils.AutoPlaceUI(content, arg.worldPos, arg.PreferDir);
                    UGUIUtils.ConstrainUIInScreen(content);
                    isPlaced = true;
                }
            }
        }

        //-@param go GameObject
        public void OnPress(GameObject go, bool press)
        {
            if(!press)
            { //因为按下的时候可能要拖动，所以在按下时就处理，弹起不处理
                return;
            }

            if(lastTipCloseFrame == Time.frameCount)
            {
                return;
            }

            var trans = go.transform;
            if(this.trigger != null && trigger.transform.IsChildOf(trans))
            {
                return;
            }

            if(trans.GetComponent<TipMessageBoxButtonMask>() != null)
            {
                return;
            }

            var topPanel = PanelManager.Normal.GetTopPanel() as TipMessageBoxBase;
            if(topPanel == null)
            {
                return;
            }

            lastTipCloseFrame = Time.frameCount;
            topPanel.Close();
        }

        public virtual void OnDestroy()
        {
            this.trigger = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log(eventData.pointerClick);
        }

        public class Argument
        {
            public GameObject trigger;
            public Vector3 worldPos; //提示位置。会自动放置在该位置周围
            public bool doNotAutoPlace;//如果不需要自动摆放位置并限制在屏幕内，设置true

            public UGUIUtils.AutoPlaceUIPreferDir PreferDir
            {
                get
                {
                    if (trigger == null)
                    {
                        return UGUIUtils.AutoPlaceUIPreferDir.None;
                    }
                    var sr = trigger.GetComponentInParent<ScrollRect>();
                    if(sr != null)
                    {
                        return sr.horizontal ? UGUIUtils.AutoPlaceUIPreferDir.Vertical : UGUIUtils.AutoPlaceUIPreferDir.Horizontal;
                    }
                    var hg = trigger.GetComponentInParent<HorizontalLayoutGroup>();
                    if (hg != null)
                    {
                        return UGUIUtils.AutoPlaceUIPreferDir.Vertical;
                    }
                    var vg = trigger.GetComponentInParent<VerticalLayoutGroup>();
                    if(vg != null)
                    {
                        return UGUIUtils.AutoPlaceUIPreferDir.Horizontal;
                    }
                    return UGUIUtils.AutoPlaceUIPreferDir.None;
                }
            }

            public Argument(GameObject trigger, Vector3 worldPos, bool doNotAutoPlace = false)
            {
                this.trigger = trigger;
                this.worldPos = worldPos;
                this.doNotAutoPlace = doNotAutoPlace;
            }
        }
    }
}
