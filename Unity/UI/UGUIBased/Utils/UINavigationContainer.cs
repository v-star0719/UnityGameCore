using System.Collections;
using System.Collections.Generic;
using GameCore.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore.Unity.UGUIEx
{
    //UGUI���Զ������ǳ�׼ȷ��ֻ��Ҫ���㼸��Ӧ����Ҫ�Ϳ��ԡ�
    //1.���ѵ�ʱ���ײ���UI��������Ч����һ����Containerʵ�֡���Container����ʱ���ɵ�Container�еĵ���ʧ��
    //2.û��ѡ�еĶ���ʱ����������������һ����û��ѡ��ʱ�Զ�ѡ�ж���ĵ�һ������
    public class UINavigationContainer : MonoBehaviour
    {
        //һ��container�����ˣ�����ʾ��һ��container��
        public static List<UINavigationContainer> containers = new();
        private static int refreshTimer;

        public Dictionary<GameObject, UINavigation> dict = new();
        private GameObject lastObj;
        private bool isWorking = true;

        public void OnEnable()
        {
            if (containers.Count > 0)
            {
                containers[^1].SetWorking(false);
            }
            containers.Add(this);
            BeginRefresh();
        }

        public void OnDisable()
        {
            var i = containers.IndexOf(this);
            containers.RemoveAt(i);
            BeginRefresh();
        }

        public void Update()
        {
            var obj = EventSystem.current.currentSelectedGameObject;
            if(obj != null)
            {
                if(dict.ContainsKey(obj))
                {
                    lastObj = obj;
                }
            }
        }

        public void SetWorking(bool b)
        {
            isWorking = b;
            foreach (var n in dict.Values)
            {
                n.SetWorking(b);
            }
        }

        public void AddNavigation(UINavigation n)
        {
            dict.Add(n.gameObject, n);
            n.SetWorking(isWorking);
        }

        public void SelectLastObj()
        {
            if(lastObj != null && lastObj.activeInHierarchy)
            {
                SetEventSystemSelect(lastObj);
            }
            else
            {
                GameObject p1 = null;
                GameObject p2 = null;
                foreach(var v in dict.Values)
                {
                    if (!v.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    if(v.startsSelected)
                    {
                        p1 = v.gameObject;
                        break;
                    }

                    if(p2 == null)
                    {
                        p2 = v.gameObject;
                    }
                }
                
                if(p1 != null || p2 != null)
                {
                    SetEventSystemSelect(p1 ?? p2);
                }
                else
                {
                    //Debug.LogError("no ui navigation to select");
                }
            }
        }

        public static void BeginRefresh()
        {
            if (refreshTimer > 0)
            {
                TimerManager.Inst.Del(refreshTimer);
            }
            //�ȴ�ʱ����̵Ļ���UI�ϵ�Navigation���ܻ�û��ʾ
            refreshTimer = TimerManager.Inst.Add(0.7f, () =>
            {
                if (containers.Count > 0)
                {
                    var c = containers[^1];
                    c.SetWorking(true);
                    c.SelectLastObj();
                }
                refreshTimer = 0;
            });
        }

        public static void SetEventSystemSelect(GameObject go)
        {
            EventSystem.current.SetSelectedGameObject(go);
            EventSystem.current.firstSelectedGameObject = go;
        }

        public static IEnumerator DelaySelect(float t)
        {
            yield return new WaitForSeconds(t);
            if(containers.Count > 0)
            {
                containers[^1].SelectLastObj();
            }
        }
    }
}
