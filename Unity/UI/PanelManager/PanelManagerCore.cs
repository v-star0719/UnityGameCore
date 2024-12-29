using System;
using GameCore;
using System.Collections.Generic;
using GameCore.Core;
using UnityEngine;

namespace GameCore.Unity
{
    public class PanelManagerCore
    {
        public Transform UIRoot { get; private set; }
        public Camera Camera { get; private set; }
        public int DepthGap { get; private set; } = 10;
        public int StartDepth { get; private set; } = 0;

        protected IResManager resManager;
        protected List<PanelBaseCore> panels = new List<PanelBaseCore>();
        public Action<PanelBaseCore> onPanelOpen;
        public Action<PanelBaseCore> onPanelClose;

        public PanelManagerCore(Transform root, IResManager resManager, Camera cam, int startDepth = 0, int depthGap = 10)
        {
            StartDepth = startDepth;
            DepthGap = depthGap;
            UIRoot = root;
            this.resManager = resManager;
            Camera = cam;
        }

        public virtual PanelBaseCore LoadPanel(string name)
        {
            var go = resManager.GetGameObject(name);
            if(go != null)
            {
                return go.GetComponent<PanelBaseCore>();
            }
            LoggerX.Error($"Panel is not found {name}");
            return null;
        }

        public virtual PanelContainerCore LoadContainer()
        {
            return resManager.GetGameObject("PanelContainer").GetComponent<PanelContainerCore>();
        }

        public virtual PanelBaseCore OpenPanel(string name, params object[] args)
        {
            var panel = LoadPanel(name);
            var container = LoadContainer();
            panel.PanelName = name;
            container.AddPanel(panel);
            container.transform.SetParent(UIRoot, false);
            GameObjectUtils.SetLayer(container.transform, UIRoot.gameObject.layer);

            panel.Open(this, args);
            panel.Depth = StartDepth + panels.Count * DepthGap;

            if (panel.settings.isFullPanel)
            {
                for (int i = panels.Count - 1; i >= 0; i--)
                {
                    var p = panels[i];
                    p.Hide();
                    if (p.settings.isFullPanel)
                    {
                        break; //全面界面下面的UI都已经隐藏了。
                    }
                }
            }

            panels.Add(panel);
            onPanelOpen?.Invoke(panel);
            return panel;
        }

        public virtual void ClosePanel(PanelBaseCore pl)
        {
            for (var i = 0; i < panels.Count; i++)
            {
                var p = panels[i];
                if (p == pl)
                {
                    panels.RemoveAt(i);
                    p.Close(true);
                    if (p.settings.isFullPanel)
                    {
                        //如果上面没有全屏界面，则把下面的UI打开
                        bool hasFullPanel = false;
                        for (int j = i; j < panels.Count; j++)
                        {
                            if (panels[j].settings.isFullPanel)
                            {
                                hasFullPanel = true;
                                break;
                            }
                        }

                        if (!hasFullPanel)
                        {
                            for (int j = i - 1; j >= 0; j--)
                            {
                                panels[j].Show();
                                if (panels[j].settings.isFullPanel)
                                {
                                    break;
                                }
                            }
                        }

                    }

                    break;
                }
            }
        }

        public virtual void ClosePanel(string name)
        {
            for (var i = panels.Count - 1; i >= 0; i--)
            {
                var p = panels[i];
                if (p.PanelName == name)
                {
                    ClosePanel(p);
                    break;
                }
            }
        }

        public virtual void CloseTopPanel()
        {
            if (panels.Count > 0)
            {
                ClosePanel(panels[panels.Count - 1]);
            }
        }

        public virtual void CloseAllPanel(bool includeFirst = false)
        {
            var e = includeFirst ? 0 : 1;
            for (int i = panels.Count - 1; i >= e; i--)
            {
                var p = panels[i];
                p.Close(true);
                panels.RemoveAt(i);
            }
        }

        public virtual PanelBaseCore GetPanel(string name)
        {
            for (var i = 0; i < panels.Count; i++)
            {
                if (panels[i].PanelName == name)
                {
                    return panels[i];
                }
            }

            return null;
        }

        public virtual PanelBaseCore GetTopPanel()
        {
            if (panels.Count > 0)
            {
                return panels[panels.Count - 1];
            }

            return null;
        }
    }
}