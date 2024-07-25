using GameCore;
using Kernel.Unity;
using System.Collections;
using UnityEngine;

namespace GameCore.Unity
{
    public class PanelManager
    {
        public static PanelManagerCore Top;
        public static PanelManagerCore Normal;

        public static void InitTop(Transform root, IResManager resManager)
        {
            Top = new PanelManagerCore(root, resManager);
        }

        public static void InitNormal(Transform root, IResManager resManager)
        {
            Normal = new PanelManagerCore(root, resManager);
        }
    }
}