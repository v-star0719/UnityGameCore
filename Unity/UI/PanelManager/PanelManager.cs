using GameCore.Unity.Main;
using UnityEngine;

namespace GameCore.Unity.UI
{
    public class PanelManager
    {
        public static PanelManagerCore Top { get; private set; }
        public static PanelManagerCore Normal { get; private set; }

        public static void InitTop(Transform root, IResManager resManager, int startDepth = 0, int depthGap = 10)
        {
            Top = new PanelManagerCore(root, resManager, Camera.main, startDepth, depthGap);
        }

        public static void InitNormal(Transform root, IResManager resManager, int startDepth = 0, int depthGap = 10)
        {
            Normal = new PanelManagerCore(root, resManager, Camera.main, startDepth, depthGap);
        }
    }
}