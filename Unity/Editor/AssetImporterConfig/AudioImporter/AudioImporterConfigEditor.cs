using GpuEcsAnimationBaker.Engine.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameCore.Unity
{
    [CustomEditor(typeof(AudioImporterConfig))]
    public class AudioImporterConfigEditor : AssetImporterConfigEditorBase
    {
        protected override void OnInspectorGUI_Custom()
        {
            var settings = target as AudioImporterConfig;
            this.GUIPropertyFieldWithOverride_Bool("forceToMono", () => settings.ForceToMono);
            this.GUIPropertyFieldWithOverride_Bool("loadInBackground", () => settings.LoadInBackground);
            this.GUIPropertyFieldWithOverride_Bool("ambisonic", () => settings.Ambisonic);
        }
    }
}