using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kernel.Unity
{
    [Serializable]
    public class ScriptingDefineSymbolItem
    {
        public string value;
        public bool enabled;
    }

    public class ScriptingDefineSymbolSetting : ScriptableObject
    {
        public List<ScriptingDefineSymbolItem> symbols = new List<ScriptingDefineSymbolItem>();
    }
}