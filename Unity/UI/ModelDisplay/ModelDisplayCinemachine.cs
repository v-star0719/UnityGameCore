using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace Kernel.Unity
{
    public class ModelDisplayCinemachine : ModelDisplayBase
    {
        public Transform cameraRoot;
        private Dictionary<string, ModelDisplayCinemachineCamInfo> camInfoMap;
        private ModelDisplayCinemachineCamInfo curCameraInfo;
        private ModelDisplayCinemachineCamInfo cacheCameraInfo;

        public ModelDisplayCinemachineCamInfo CurCameraInfo => curCameraInfo;

        public void Init(ITouchEventSource source, ModelDisplayCinemachineCamConf[] confs, float screenFactor = -1, float scrollFactor = -1)
        {
            base.Init(source, screenFactor, scrollFactor);
            camInfoMap = new Dictionary<string, ModelDisplayCinemachineCamInfo>();

            foreach (var conf in confs)
            {
                var goName = conf.camName;
                var info = new ModelDisplayCinemachineCamInfo();
                info.key = conf.camName;
                info.camGo = cameraRoot.Find(goName).gameObject;
                info.xValue = 0;
                info.yValue = 0;
                info.zoom = 1;
                info.conf = conf;
                if (info.camGo != null)
                {
                    if (conf.type == CamTypes.Normal)
                    {
                        var cam = info.camGo.GetComponent<CinemachineVirtualCamera>();
                        info.virtualCam = cam;
                        info.lookAtTarget = cam.LookAt;
                        if (info.lookAtTarget != null)
                        {
                            info.maxDistance = Vector3.Distance(info.camGo.transform.position, info.lookAtTarget.position);
                        }
                        else
                        {
                            info.maxDistance = cam.transform.position.magnitude;
                        }
                    }
                    else if (conf.type == CamTypes.DollyTrack)
                    {
                        var cam = info.camGo.GetComponent<CinemachineVirtualCamera>();
                        info.bodyTrackedDolly =
                            cam.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineTrackedDolly;
                        info.bodyTrackGo = cameraRoot.Find(goName + "Track").gameObject;
                        info.bodyTrackOrgScale = info.bodyTrackGo.transform.localScale.x;
                        var trans = cam.LookAt;
                        if (trans)
                        {
                            info.lookTargetDollyCart = trans.GetComponent<CinemachineDollyCart>();
                        }
                        else
                        {
                            Debug.Log($"no CinemachineDollyCart for {goName}");
                        }
                    }
                    else if (conf.type == CamTypes.FreeLook)
                    {
                        info.freeLookCam = info.camGo.GetComponent<CinemachineFreeLook>();
                        info.freeLookOrgRadiusList = new float[3];
                        info.freeLookOrgRadiusList[0] = info.freeLookCam.m_Orbits[0].m_Radius;
                        info.freeLookOrgRadiusList[1] = info.freeLookCam.m_Orbits[1].m_Radius;
                        info.freeLookOrgRadiusList[2] = info.freeLookCam.m_Orbits[2].m_Radius;
                    }
                }

                camInfoMap.Add(conf.camName, info);
            }
        }

        public override void Show(string resName)
        {
            base.Show(resName);
        }

        public void SwitchCamera(string name)
        {
            if (curCameraInfo != null)
            {
                curCameraInfo.camGo.SetActive(false);
                curCameraInfo = null;
            }

            if (camInfoMap.TryGetValue(name, out var info))
            {
                curCameraInfo = info;
                info.camGo.SetActive(true);
            }
            else
            {
                Debug.LogError($"no camera: {name}");
            }
        }

        public void CacheCamera()
        {
            if (cacheCameraInfo == null)
            {
                cacheCameraInfo = new ModelDisplayCinemachineCamInfo();
            }

            cacheCameraInfo.key = curCameraInfo.key;
            cacheCameraInfo.xValue = curCameraInfo.xValue;
        }

        public void RecoverCamera()
        {
            curCameraInfo?.camGo.SetActive(false);
            curCameraInfo = camInfoMap[cacheCameraInfo.key];
            curCameraInfo.xValue = cacheCameraInfo.xValue;
            curCameraInfo.camGo.SetActive(true);
        }

        //x，z平面移动
        public void SetCameraPos(Vector3 pos)
        {
            var y = curCameraInfo.camGo.transform.position.y;
            pos.y = y;
            curCameraInfo.camGo.transform.position = pos;
        }

        //认为只转x轴了，x，z平面移动
        public void SetCameraPosByLookPos(Vector3 pos)
        {
            var trans = curCameraInfo.camGo.transform;
            var offset = Mathf.Tan((90 - trans.localEulerAngles.x) * Mathf.Deg2Rad) * trans.position.y;
            pos.z -= offset;
            SetCameraPos(pos);
        }

        public void MoveLookTarget(Vector3 delta)
        {

        }

        protected override void Update()
        {
            base.Update();
        }

        protected override void ZoomByDelta(float delta)
        {
            if (curCameraInfo == null)
            {
                return;
            }

            var info = curCameraInfo;
            var conf = info.conf;
            if (conf.zoomMin <= 0 || conf.zoomMax <= 0)
            {
                return;
            }

            info.zoom += delta * zoomFactor;

            if (info.zoom > conf.zoomMax)
            {
                info.zoom = conf.zoomMax;
            }
            if (info.zoom < conf.zoomMin)
            {
                info.zoom = conf.zoomMin;
            }

            switch (info.conf.type)
            {
                case CamTypes.Normal:
                {
                    var dir = info.camGo.transform.position - info.lookAtTarget.transform.position;
                    info.camGo.transform.position = info.lookAtTarget.transform.position + 
                                                    dir.normalized * (info.maxDistance * info.zoom);
                    break;
                }

                case CamTypes.DollyTrack:
                {
                    var s = info.bodyTrackOrgScale * info.zoom;
                    info.bodyTrackGo.transform.localScale = new Vector3(s, s, s);
                    break;
                }

                case CamTypes.FreeLook:
                {
                    var oribit = info.freeLookCam.m_Orbits[0];
                    oribit.m_Radius = info.freeLookOrgRadiusList[0] * info.zoom;
                    info.freeLookCam.m_Orbits[0] = oribit;
                    //
                    oribit = info.freeLookCam.m_Orbits[1];
                    oribit.m_Radius = info.freeLookOrgRadiusList[1] * info.zoom;
                    info.freeLookCam.m_Orbits[1] = oribit;
                    //
                    oribit = info.freeLookCam.m_Orbits[2];
                    oribit.m_Radius = info.freeLookOrgRadiusList[2] * info.zoom;
                    info.freeLookCam.m_Orbits[2] = oribit;
                        break;
                }
            }
        }

        protected override void DragByDelta(float x, float y)
        {
            x *= dragFactor;
            y *= dragFactor;
            var info = curCameraInfo;
            switch (info.conf.type)
            {
                case CamTypes.Normal:
                {
                    if (info.lookAtTarget != null)
                    {
                        var pos = info.camGo.transform.position - info.lookAtTarget.transform.position;
                        pos = info.lookAtTarget.transform.rotation * 
                              Quaternion.Euler(y, x, 0) * pos;
                        info.camGo.transform.position = info.lookAtTarget.transform.position + pos;
                    }
                    else
                    {
                        var v = info.camGo.transform.localPosition;
                        v.x -= x;
                        v.z -= y;
                        info.camGo.transform.localPosition = ClampPos(v);
                    }
                    break;
                }

                case CamTypes.DollyTrack:
                {
                    //0~1
                    var v = info.bodyTrackedDolly.m_PathPosition;
                    v = v - x * 0.002f;
                    info.bodyTrackedDolly.m_PathPosition = v;
                    if (info.lookTargetDollyCart)
                    {
                        info.lookTargetDollyCart.m_Position = v;
                    }

                    break;
                }

                case CamTypes.FreeLook:
                {
                    //0~360
                    if (x != 0)
                    {
                        var axis = info.freeLookCam.m_XAxis;
                        var v = axis.Value;
                        v = v + x * 0.8f;
                        axis.Value = v;
                        info.freeLookCam.m_XAxis = axis;
                    }

                    //0~1
                    if (y != 0)
                    {
                        var axis = info.freeLookCam.m_YAxis;
                        var v = axis.Value;
                        v = v + y * 0.003f;//x是0~360，y是0~1,所以y是1/360
                        axis.Value = v;
                        info.freeLookCam.m_YAxis = axis;
                    }

                    break;
                }
            }
        }
    }


    public class ModelDisplayCinemachineCamInfo
    {
        public ModelDisplayCinemachineCamConf conf;
        public string key;
        public GameObject camGo;
        public CinemachineTrackedDolly bodyTrackedDolly; //轨道相机的body
        public CinemachineDollyCart lookTargetDollyCart; //轨道相机看的可以在轨道上移动的目标
        public GameObject bodyTrackGo; //轨道相机的轨道。缩放轨道实现拉远拉近效果
        public CinemachineFreeLook freeLookCam; //自由观察相机
        public CinemachineVirtualCamera virtualCam; 
        public float bodyTrackOrgScale;
        public float[] freeLookOrgRadiusList; //012三个圈的半径
        public float xValue;
        public float yValue;
        public float zoom;
        public float maxDistance;//自由相机的最大距离
        public Transform lookAtTarget;
    }

    public enum CamTypes //ModelDisplayCinemachineCamType
    {
        Normal,
        DollyTrack,
        FreeLook
    }

    public class ModelDisplayCinemachineCamConf
    {
        public string camName; //相机gameObject名字
        public CamTypes type; //不同相机的旋转和缩放控制方法不一样
        public float zoomMax; //相机拉远拉近的最大值
        public float zoomMin; //相机拉远拉近的最小值

        public ModelDisplayCinemachineCamConf(string camName, CamTypes type, float zoomMin, float zoomMax)
        {
            this.camName = camName;
            this.type = type;
            this.zoomMin = zoomMin;
            this.zoomMax = zoomMax;
        }
    }
}
