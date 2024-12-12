using System;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    [Serializable]
    public class GameConsoleFuncBase
    {
        [NonSerialized]
        protected GameConsoleWindow window;
        [NonSerialized]
        public string name;
        public void Init(GameConsoleWindow wnd)
        {
            window = wnd;
        }

        protected GameConsoleFuncBase()
        {
        }

        public virtual void OnGUI()
        {
        }

        public virtual void Update()
        {

        }

        public void Send(string method, object v1 = null, object v2 = null, object v3 = null, object v4 = null,
            object v5 = null, object v6 = null)
        {
            window.Send(method, v1, v2, v3, v4, v5, v6);
        }

        public void CallServerAPI(string api, params object[] args)
        {
            window.CallServerAPI(api, args);
        }

        public string GetAllianceShortName()
        {
            return window.GetAllianceShortName();
        }
    }
}