using GpuEcsAnimationBaker.Engine.Data;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEditor.SpeedTreeImporter;

namespace GameCore.Unity
{
    [CustomEditor(typeof(ModelImporterConfig))]
    public class ModelImporterConfigEditor : AssetImporterConfigEditorBase
    {
        private static int tab = 0;
        private static string[] tabNames = { "Model", "Rig", "Animation", "Materials" };

        protected override void OnInspectorGUI_Custom()
        {
            var settings = target as ModelImporterConfig;

            EditorGUILayout.ObjectField(serializedObject.FindProperty("parent"));
            if (settings.CheckParentLoop())
            {
                var clr = GUI.color;
                GUI.color = Color.red;
                GUILayout.Label("parent loop reference");
                GUI.color = clr;
                serializedObject.ApplyModifiedProperties();
                return;
            }

            EditorGUILayout.PropertyField(serializedObject.FindProperty("desc"));
            if (settings.Parent == null)
            {
                TextureImporterConfigWindow.RootConfig = null;
            }
            else
            {
                //EditorGUIUtil.DrawSeparator();
            }

            tab = GUILayout.SelectionGrid(tab, tabNames, tabNames.Length);

            switch (tab)
            {
                case 0:
                    //Model--Scene
                    this.GUIPropertyFieldWithOverride_Bool("convertUnits", () => settings.ConvertUnits);
                    this.GUIPropertyFieldWithOverride_Bool("bakeAxisConversion", () => settings.BakeAxisConversion);
                    this.GUIPropertyFieldWithOverride_Bool("importBlendShapes", () => settings.ImportBlendShapes);
                    this.GUIPropertyFieldWithOverride_Bool("importDeformPercent", () => settings.ImportDeformPercent);
                    this.GUIPropertyFieldWithOverride_Bool("importVisibility", () => settings.ImportVisibility);
                    this.GUIPropertyFieldWithOverride_Bool("importCamera", () => settings.ImportCamera);
                    this.GUIPropertyFieldWithOverride_Bool("importLights", () => settings.ImportLights);
                    this.GUIPropertyFieldWithOverride_Bool("preserveHierarchy", () => settings.PreserveHierarchy);

                    //Model--Meshes
                    this.GUIPropertyFieldWithOverride_Enum("meshCompression", () => settings.MeshCompression);
                    this.GUIPropertyFieldWithOverride_Bool("readWrite", () => settings.ReadWrite);
                    this.GUIPropertyFieldWithOverride_Enum("optimizeMesh", () => settings.OptimizeMesh);

                    //Model--Geometry
                    this.GUIPropertyFieldWithOverride_Enum("indexFormat", () => settings.IndexFormat);
                    this.GUIPropertyFieldWithOverride_Bool("generateLightmapUvs", () => settings.GenerateLightmapUvs);
                    break;

                case 1:
                    //Rig
                    this.GUIPropertyFieldWithOverride_Enum("animationType", () => settings.AnimationType);
                    this.GUIPropertyFieldWithOverride_Enum("generateAnimations", () => settings.GenerateAnimations);
                    this.GUIPropertyFieldWithOverride_Bool("stripBones", () => settings.StripBones);
                    break;

                case 2:
                    //Animation
                    this.GUIPropertyFieldWithOverride_Bool("importAnimation", () => settings.ImportAnimation);
                    this.GUIPropertyFieldWithOverride_Bool("importConstraints", () => settings.ImportConstraints);
                    break;

                case 3:
                    //Materials
                    this.GUIPropertyFieldWithOverride_Enum("materialImportMode", () => settings.MaterialImportMode);
                    this.GUIPropertyFieldWithOverride_Bool("useSRGBMaterialColor", () => settings.UseSRGBMaterialColor);
                    this.GUIPropertyFieldWithOverride_Enum("materialLocation", () => settings.MaterialLocation);
                    this.GUIPropertyFieldWithOverride_Enum("materialName", () => settings.MaterialName);
                    this.GUIPropertyFieldWithOverride_Enum("materialSearch", () => settings.MaterialSearch);
                    break;
            }
        }
    }
}