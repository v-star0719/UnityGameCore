using System.Collections.Generic;
using UnityEngine;

public class UIManagerCore
{
    private const int DEPTH_GAP = 20;
    private List<UIPanelBaseCore> panels = new List<UIPanelBaseCore>();

    public Transform UIRoot { get; private set; }

    public UIManagerCore(Transform root)
    {
        UIRoot = root;
    }

    public virtual UIPanelBaseCore LoadPanel(string name)
    {
        return null;
    }

    public virtual UIPanelContainer LoadContainer()
    {
        return null;
    }

    public virtual UIPanelBaseCore OpenPanel(string name, params object[] args)
    {
        var panel = LoadPanel(name);
        var container = LoadContainer();
        panel.PanelName = name;
        container.AddPanel(panel);
        container.transform.SetParent(UIRoot, false);
        GameObjectUtils.SetLayer(container.transform, UIRoot.gameObject.layer);

        panel.Open(this, args);
        panel.Depth = panels.Count * DEPTH_GAP;

        if (panel.isFullPanel)
        {
            for (int i = panels.Count - 1; i >= 0; i--)
            {
                var p = panels[i];
                p.Hide();
                if (p.isFullPanel)
                {
                    break;//全面界面下面的UI都已经隐藏了。
                }
            }
        }
        panels.Add(panel);
        return panel;
    }

    public virtual void ClosePanel(UIPanelBaseCore pl)
    {
        for (var i = 0; i < panels.Count; i++)
        {
            var p = panels[i];
            if (p == pl)
            {
                panels.RemoveAt(i);
                p.Close(true);
                if (p.isFullPanel)
                {
                    //如果上面没有全屏界面，则把下面的UI打开
                    bool hasFullPanel = false;
                    for (int j = i; j < panels.Count; j++)
                    {
                        if (panels[j].isFullPanel)
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
                            if (panels[j].isFullPanel)
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
            var p = panels[panels.Count - 1];
            panels.RemoveAt(panels.Count - 1);
            p.Close(true);
        }
    }

    public virtual void CloseAllPanel(bool includeFirst = false)
    {
        var e = includeFirst ? 0 : 1;
        for(int i = panels.Count - 1; i >= e; i--)
        {
            var p = panels[i];
            p.Close(true);
            panels.RemoveAt(i);
        }
    }

    public virtual UIPanelBaseCore GetPanel(string name)
    {
        for(var i = 0; i < panels.Count; i++)
        {
            if(panels[i].PanelName == name)
            {
                return panels[i];
            }
        }

        return null;
    }

    public virtual UIPanelBaseCore GetTopPanel()
    {
        if (panels.Count > 0)
        {
            return panels[panels.Count - 1];
        }
        return null;
    }
}
