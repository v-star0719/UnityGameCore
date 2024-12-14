using GameCore;
using GameCore.Unity;
using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    public class PanelManager
    {
        public static PanelManagerCore Top;
        public static PanelManagerCore Normal;

        public static void InitTop(Transform root, IResManager resManager, int startDepth = 0, int depthGap = 10)
        {
            Top = new PanelManagerCore(root, resManager, startDepth, depthGap);
        }

        public static void InitNormal(Transform root, IResManager resManager, int startDepth = 0, int depthGap = 10)
        {
            Normal = new PanelManagerCore(root, resManager, startDepth, depthGap);
        }
    }
}