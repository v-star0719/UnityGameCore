using System.Collections;
using System.Collections.Generic;
using GameCore;
using GameCore.Unity;
using GameCore.Core;
using UnityEngine;
#if NGUI
namespace GameCore.Unity.NGUIEx
{
    public class UIKeyNavigationContainer : MonoBehaviour
    {
        //一个container隐藏了，就显示上一个container的
        public static List<UIKeyNavigationContainer> containers = new();

        public Dictionary<GameObject, UIKeyNavigation> dict = new();
        private GameObject lastObj;

        public void OnEnable()
        {
            containers.Add(this);
        }

        public void OnDisable()
        {
            var i = containers.IndexOf(this);
            containers.RemoveAt(i);
            if (containers.Count > 0 && i > 0 && Game.Inst != null)
            {
                Game.Inst.StartCoroutine(DelaySelect(0.1f));//底部的面板可能被关闭了，需要等底部的面板打开后才能进行选中
            }
        }

        public void Update()
        {
            var obj = UICamera.hoveredObject;
            if (obj != null)
            {
                if (dict.ContainsKey(obj))
                {
                    lastObj = obj;
                }
            }
        }

        public void SelectLastObj()
        {
            if (lastObj != null && lastObj.activeInHierarchy)
            {
                UICamera.hoveredObject = lastObj;
                UICamera.selectedObject = lastObj;
            }
            else
            {
                GameObject p1 = null;
                GameObject p2 = null;
                foreach (var v in dict.Values)
                {
                    if (v.gameObject.activeInHierarchy)
                    {
                        if (v.startsSelected)
                        {
                            p1 = v.gameObject;
                        }
                        else
                        {
                            p2 = v.gameObject;
                        }
                    }
                }

                if (p1 != null)
                {
                    UICamera.hoveredObject = p1;
                    UICamera.selectedObject = p1;
                }
                else if (p2 != null)
                {
                    UICamera.hoveredObject = p2;
                    UICamera.selectedObject = p2;
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
            if (containers.Count > 0)
            {
                containers[^1].SelectLastObj();
            }
        }
    }
}
#endif