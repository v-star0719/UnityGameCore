using GameCore.Unity;
using UI;
using UnityEngine;

#if NGUI
namespace GameCore.Unity.NGUIEx
{

    //（1）点击弹出，点击屏幕其他任意位置关闭
    //当点击弹窗本体关闭时，不会触发底部的点击，非常安全。点击弹窗本体不关闭。
    //
    //（2）自动放置在指定的位置周围，并确保UI在屏幕内
    public class TipMessageBoxBase : UIPanelBase
    {
        //点击屏幕其他任意位置关闭
        [HideInInspector] public GameObject trigger; //点在trigger上不会触发关闭
        public TipMessageBoxBaseArg arg;
        private static int lastTipCloseFrame = 0;

        //自动放置在指定的位置周围
        private bool isPlaced; //是否处理了
        private UIPanel panel;
        private bool waitPlace; //有时候需要等界面排列完毕

        protected override void OnOpen(params object[] args)
        {
            if (args.Length <= 0)
            {
                Debug.LogError("TipMessageBoxBaseArg is required");
                return;
            }

            arg = args[0] as TipMessageBoxBaseArg;
            trigger = arg.trigger;
        }

        public virtual void Start()
        {
            if (arg.doNotAutoPlace)
            {
                SetPosition(arg.worldPos);
                isPlaced = true;
            }
            else
            {
                panel = gameObject.GetComponentInParent<UIPanel>();
                if (panel == null)
                {
                    Debug.LogError("UIPanel is required to fade in");
                    isPlaced = true;
                    return;
                }

                panel.alpha = 0;
            }

            UICamera.onPress += OnPress;
        }

        public void Update()
        {
            if (!isPlaced)
            {
                if (!waitPlace)
                {
                    panel.alpha = 1;
                    var uiGo = transform.GetChild(0).gameObject;
                    NGUIUtils.AutoPlaceUI(gameObject, uiGo, arg.worldPos);
                    NGUIUtils.ConstrainUIInScreen(uiGo);
                    isPlaced = true;
                }
            }
        }

        //-@param go GameObject
        public void OnPress(GameObject go, bool press)
        {
            if (!press)
            {
                //因为按下的时候可能要拖动，所以在按下时就处理，弹起不处理
                return;
            }

            if (lastTipCloseFrame == Time.frameCount)
            {
                return;
            }

            var trans = go.transform;
            if (this.trigger != null && NGUITools.IsChild(this.trigger.transform, trans))
            {
                return;
            }

            //if (NGUITools.IsChild(transform, trans))
            //{
            //    return;
            //}

            if (trans.GetComponent<TipMessageBoxButtonMask>() != null)
            {
                return;
            }

            var topPanel = PanelManager.Normal.GetTopPanel() as TipMessageBoxBase;
            if (topPanel == null)
            {
                return;
            }

            lastTipCloseFrame = Time.frameCount;
            topPanel.Close();
        }

        public virtual void OnDestroy()
        {
            UICamera.onPress -= OnPress;
            this.trigger = null;
        }
    }

    public class TipMessageBoxBaseArg
    {
        public GameObject trigger;
        public Vector3 worldPos; //提示位置。会自动放置在该位置周围
        public bool doNotAutoPlace; //如果不需要自动摆放位置并限制在屏幕内，设置true

        public TipMessageBoxBaseArg(GameObject trigger, Vector3 worldPos, bool doNotAutoPlace = false)
        {
            this.trigger = trigger;
            this.worldPos = worldPos;
            this.doNotAutoPlace = doNotAutoPlace;
        }
    }
}
#endif