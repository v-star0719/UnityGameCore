using System;
using UKernel.Unity;
using UnityEngine;

namespace Kernel.Unity
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
        public Camera camera;
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
        protected bool freeDrag;//�����ƶ��Ĳ�����ͬһʱ��ֻ��x����y������
        protected float dragFactor;//�϶�ʱ����Ļλ��ת�����߼�λ�ơ�
        protected float zoomFactor = 1;//˫ָ����ʱ����Ļλ��ת�����߼�λ��
        private float scrollFactor;//�����ֵ�λ��ת��Ϊ��Ļλ��

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
            this.dragFactor = dragFactor > 0 ? dragFactor : 180f / Screen.width;//λ��1��180��
            this.zoomFactor = zoomFactor > 0 ? zoomFactor : 0.5f / Screen.width;//λ��1������1��
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

            //ƻ���ֻ������ײ��Ĵ����ܻᵼ�¶�ʧ��ק��Ϣ��UI����ʾ
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
            Debug.Log("���ظú���ʵ����Զ����Ч��");
        }

        protected virtual void DragByDelta(float x, float y)
        {
            Debug.Log("���ظú���ʵ���ƶ�Ч��");
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

        public void OnDragStart()
        {
            isDragging = true;
            missTouchTime = 0;
            smoothlyDrag.Stop();
        }

        public void OnDrag(Vector2 dir)
        {
            //˫�����ź�һ��ʱ��������ת����Ȼ˫�������ɿ��󣬻�������л���
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

        public void OnDragEnd()
        {
            missTouchTime = 0;
            isDragging = false;
            //˫�����ź�һ��ʱ��������ת
            if (IsZooming())
            {
                return;
            }
            smoothlyDrag.Start();
        }

        public void OnZoom(float delta)
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

            if (modelObj != null) ;
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
