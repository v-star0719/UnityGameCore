using System.Collections;
using System.Collections.Generic;
using GameCore.Core;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameCore.Unity.UGUIEx
{
    //UGUI的自动导航非常准确，只需要满足几点应用需要就可以。
    //1.面板堆叠时，底部的UI导航不生效。这一点用Container实现。新Container激活时，旧的Container中的导航失活
    //2.没有选中的对象时，导航不工作。这一点在没有选中时自动选中顶层的第一个物体
    public class UINavigationContainer : MonoBehaviour
    {
        //一个container隐藏了，就显示上一个container的
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
            //等待时间过短的话，UI上的Navigation可能还没显示
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
