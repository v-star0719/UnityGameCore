using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Kernel.Unity.UI
{
    //UGUI的自动导航非常准确，只需要满足几点应用需要就可以。
    //1.面板堆叠时，底部的UI导航不生效。这一点用Container实现。新Container激活时，旧的Container中的导航失活
    //2.没有选中的对象时，导航不工作。这一点在没有选中时自动选中顶层的第一个物体
    public class UINavigationContainer : MonoBehaviour
    {
        //一个container隐藏了，就显示上一个container的
        public static List<UINavigationContainer> containers = new();

        public Dictionary<GameObject, UINavigation> dict = new();
        private GameObject lastObj;
        private bool isWorking = true;

        public void OnEnable()
        {
            foreach (var c in containers)
            {
                c.SetWorking(false);
            }
            containers.Add(this);
        }

        public void OnDisable()
        {
            var i = containers.IndexOf(this);
            containers.RemoveAt(i);
            if(containers.Count > 0 && i > 0 && Game.Inst != null)
            {
                Game.Inst.StartCoroutine(DelaySelect(0.1f));//底部的面板可能被关闭了，需要等底部的面板打开后才能进行选中
            }
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
                EventSystem.current.SetSelectedGameObject(lastObj); 
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
                    EventSystem.current.SetSelectedGameObject(p1 ?? p2);
                }
                else
                {
                    //Debug.LogError("no ui navigation to select");
                }
            }
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
