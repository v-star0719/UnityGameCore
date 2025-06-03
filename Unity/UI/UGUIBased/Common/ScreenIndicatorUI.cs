using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    public class ScreenIndicatorUI : MonoBehaviour
    {
        public Camera worldCamera;
        public Camera uiCamera; //Screen Space - Overlay模式下用null
        public Vector3 worldPos;
        public Transform worldObj;
        public RectTransform viewArea;
        public GameObject uiContent;
        public float angle;//朝向x+轴是0度
        public bool hideIfNotInScreen = true;

        protected bool isShow;
        protected float screenW; //屏幕宽
        protected float screenH; //屏幕高
        protected float viewW; //视图宽，指示器显示区域，用来控制离屏幕边缘的距离
        protected float viewH; //视图高，指示器显示区域，用来控制离屏幕边缘的距离
        protected bool isInited;
        protected Vector3 lastWorldCameraPos = new Vector3(float.MaxValue, 0, 0);
        protected float lastWorldCameraFov;
        protected float lastWorldCameraOrth;
        protected Vector3 lastWorldPos = new Vector3(float.MaxValue, 0, 0);

        public virtual void Start()
        {
            if (!isInited)
            {
                Init();
            }
        }

        public virtual void Init()
        {
            isInited = true;
            screenW = Screen.width;
            screenH = Screen.height;
            if (viewArea != null)
            {
                viewW = viewArea.rect.width;
                viewH = viewArea.rect.height;
            }
            else
            {
                viewW = screenW;
                viewH = screenH;
            }
        }

        public void SetPos(Vector3 worldPos)
        {
            this.worldPos = worldPos;
            worldObj = null;
        }

        public void SetPos(Transform worldObj)
        {
            this.worldPos = worldObj.position;
            this.worldObj = worldObj;
        }

        protected virtual void Update()
        {
            if (worldObj != null && worldPos != worldObj.position)
            {
                worldPos = worldObj.position;
            }

            if (lastWorldPos == worldPos && lastWorldCameraPos == worldCamera.transform.position &&
                lastWorldCameraFov == worldCamera.fieldOfView && lastWorldCameraOrth == worldCamera.orthographicSize)
            {
                return;
            }

            lastWorldPos = worldPos;
            lastWorldCameraPos = worldCamera.transform.position;
            lastWorldCameraFov = worldCamera.fieldOfView;
            lastWorldCameraOrth = worldCamera.orthographicSize;

            var screenPos = worldCamera.WorldToScreenPoint(worldPos);

            //是否在屏幕内
            if(0 <= screenPos.x && screenPos.x <= screenW && 0 <= screenPos.y && screenPos.y <= screenH)
            {
                if(hideIfNotInScreen && uiContent != null && uiContent.activeSelf)
                {
                    uiContent.SetActive(false);
                }

                if(!hideIfNotInScreen)
                {
                    //转换为视图内坐标
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(viewArea, screenPos, uiCamera, out var viewPos);
                    //如果不在ViewArea内，计算和和ViewArea边界的交点
                    if(!viewArea.rect.Contains(viewPos))
                    {
                        viewPos = GameCore.Core.MathUtils.CalculateRayScreenIntersection(new Vector2(viewW, viewH), viewPos);
                    }
                    var ag = Mathf.Atan2(viewPos.y, viewPos.x) * Mathf.Rad2Deg;
                    transform.localEulerAngles = new Vector3(0, 0, ag + angle);
                    transform.localPosition = viewPos;
                }
            }
            else
            {
                //在屏幕外
                if(uiContent != null && !uiContent.activeSelf)
                {
                    uiContent.SetActive(true);
                }

                //转换为视图内坐标
                RectTransformUtility.ScreenPointToLocalPointInRectangle(viewArea, screenPos, uiCamera, out var viewPos);
                //交点计算
                var intersectPos = GameCore.Core.MathUtils.CalculateRayScreenIntersection(new Vector2(viewW, viewH), viewPos);
                var ag = Mathf.Atan2(intersectPos.y, intersectPos.x) * Mathf.Rad2Deg;
                transform.localEulerAngles = new Vector3(0, 0, ag + angle);
                transform.localPosition = intersectPos;
            }
        }
    }
}