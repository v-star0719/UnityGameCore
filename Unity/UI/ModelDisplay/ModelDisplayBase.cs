using System;
using UKernel.Unity;
using UnityEngine;

namespace GameCore.Unity
{
    public interface ITouchEventSource
    {
        Action onDragStart { get; set; }
        Action<Vector2> onDrag { get; set; }
        Action onDragEnd { get; set; }
        Action onClick { get; set; }
        Action<float> onZoom { get; set; }
    }

    public class ModelDisplayBase : MonoBehaviour
    {
        public Camera myCamera;
        public Transform modelRoot;

        private Touch_SmoothlyDrag smoothlyDrag;
        private Touch_SmoothlyZoom smoothlyZoom;
        private GameObject modelObj;
        private string curResName;
        private RenderTexture renderTex;
        private float missTouchTime;
        private bool isDragging;
        private float lastZoomTime;
        private ITouchEventSource eventSource;
        protected bool freeDrag;//自由移动的不限制同一时间只能x或者y方向拖
        protected float dragFactor;//拖动时，屏幕位移转换成逻辑位移。
        protected float zoomFactor = 1;//双指缩放时，屏幕位移转换成逻辑位移
        private float scrollFactor;//鼠标滚轮的位移转化为屏幕位移

        public GameObject modelGo => modelObj;
        public string ResName => curResName;

        public bool FreeDrag
        {
            get => freeDrag;
            set => freeDrag = value;
        }

        public void Init(ITouchEventSource source, float dragFactor = -1, float zoomFactor = -1, float scrollFactor = -1)
        {
            eventSource = source;
            this.dragFactor = dragFactor > 0 ? dragFactor : 180f / Screen.width;//位移1屏180度
            this.zoomFactor = zoomFactor > 0 ? zoomFactor : 0.5f / Screen.width;//位移1屏缩放1半
            this.scrollFactor = scrollFactor > 0 ? scrollFactor : 100;
            smoothlyDrag = new Touch_SmoothlyDrag(OnSmoothlyDrag);
            smoothlyZoom = new Touch_SmoothlyZoom(OnSmoothlyZoom);
            missTouchTime = 0;

            if (source != null)
            {
                source.onDragStart += OnDragStart;
                source.onDrag += OnDrag;
                source.onDragEnd += OnDragEnd;
                source.onZoom += OnZoom;
            }

            //CinemachineCore.CameraUpdatedEvent.AddListener(self.CameraUpdatedEvent)
        }

        public virtual void Show(string resName)
        {
            if (curResName == resName)
            {
                return;
            }
            if (modelObj != null)
            {
                //AssetManager.RecycleAsset(modelObj)
                Destroy(modelObj);
                modelObj = null;
            }

            curResName = resName;
            modelObj = ResManager.Instance.GetGameObject(resName);
            modelObj.transform.parent = modelRoot;
            modelObj.transform.localPosition = Vector3.zero;
            modelObj.transform.localRotation = Quaternion.identity;;
			GameObjectUtils.SetLayer(modelObj.transform, gameObject.layer);
            OnModelLoaded();
        }

        protected virtual void Update()
        {
            OnCameraUpdatedEvent();

            //苹果手机摸到底部的触摸杠会导致丢失拖拽消息，UI不显示
            if (isDragging)
            {
                var isTouchMissed = false;
                //if(IsPcEditor())
                //{
                //	isTouchMissed = not Input.GetMouseButton(0);
                //}
                //else
                {
                    isTouchMissed = Input.touchCount == 0;
                }
                if (isTouchMissed)
                {
                    missTouchTime = missTouchTime + Time.deltaTime;
                    if (missTouchTime > 0.5f)
                    {
                        OnDragEnd();
                    }
                }
            }
        }

        protected virtual void ZoomByDelta(float delta)
        {
            Debug.Log("重载该函数实现拉远拉近效果");
        }

        protected virtual void DragByDelta(float x, float y)
        {
            Debug.Log("重载该函数实现移动效果");
        }

        protected bool IsZooming()
        {
            return Time.realtimeSinceStartup - lastZoomTime < 0.2f;
        }

        private void OnCameraUpdatedEvent()
        {
            if (smoothlyDrag != null)
            {
                var t = Time.deltaTime;
                smoothlyDrag.Tick(t);
                smoothlyZoom.Tick(t);
            }
        }

        public virtual void OnDragStart()
        {
            isDragging = true;
            missTouchTime = 0;
            smoothlyDrag.Stop();
        }

        public virtual void OnDrag(Vector2 dir)
        {
            //双手缩放后一定时间后才能旋转，不然双手缩放松开后，会立马进行滑动
            if (IsZooming())
            {
                return;
            }

            if (!freeDrag)
            {
                var t = dir.x * dir.x;
                var cos = t / (dir.x * dir.x + dir.y * dir.y);
                if (cos > 0.5f)
                {
                    dir.y = 0;
                }
                else
                {
                    dir.x = 0;
                }
            }

            DragByDelta(dir.x, dir.y);
            smoothlyDrag.SetLastDragDelta(dir.x, dir.y);
        }

        public virtual void OnDragEnd()
        {
            missTouchTime = 0;
            isDragging = false;
            //双手缩放后一定时间后才能旋转
            if (IsZooming())
            {
                return;
            }
            smoothlyDrag.Start();
        }

        public virtual void OnZoom(float delta)
        {
            delta = -delta;
            smoothlyZoom.SetLastZoomDelta(delta);
            ZoomByDelta(delta);
            lastZoomTime = Time.realtimeSinceStartup;
            smoothlyDrag.Stop();
        }

        public void OnScroll(GameObject go, float delta)
        {
            OnZoom(delta * scrollFactor);
        }

        public void OnSmoothlyDrag(float x, float y)
        {
            DragByDelta(x, y);
        }

        public void OnSmoothlyZoom(float f)
        {
            ZoomByDelta(f);
        }

        protected virtual void OnModelLoaded()
        {
        }

        protected virtual Vector3 ClampPos(Vector3 pos)
        {
            return pos;
        }

        private void OnDestroy()
        {
            if (renderTex != null)
            {
                renderTex.Release();
            }

            if (modelObj != null)
            {
                //AssetManager.RecycleAsset(modelObj)
                Destroy(modelObj);
                modelObj = null;
            }

            if (eventSource != null)
            {
                eventSource.onDragStart -= OnDragStart;
                eventSource.onDrag -= OnDrag;
                eventSource.onDragEnd -= OnDragEnd;
                eventSource.onZoom -= OnZoom;
            }
            //CinemachineCore.CameraUpdatedEvent.RemoveListener(CameraUpdatedEvent);
        }
    }
}
