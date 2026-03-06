using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameCore.Unity.Editor
{
    [Serializable]
    public class ScriptingDefineSymbolItem
    {
        public string value;
        public string desc;
        public bool enabled;
    }

    public class ScriptingDefineSymbolSetting : ScriptableObject
    {
        public List<ScriptingDefineSymbolItem> symbols = new List<ScriptingDefineSymbolItem>();
    }
}