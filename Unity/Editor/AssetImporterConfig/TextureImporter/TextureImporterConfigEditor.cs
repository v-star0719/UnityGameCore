using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    [CustomEditor(typeof(TextureImporterConfig))]
    public class TextureImporterConfigEditor : AssetImporterConfigEditorBase
    {
        private static string[] _textureImporterTypeOptions;
        private static string[] TextureImporterTypeOptions
        {
            get
            {
                if (_textureImporterTypeOptions == null)
                {
                    var list = new List<string>();
                    list.Add("None");
                    foreach (var fieldInfo in typeof(TextureImporterType).GetFields(BindingFlags.Static))
                    {
                        if (fieldInfo.GetCustomAttribute<ObsoleteAttribute>() == null)
                        {
                            list.Add(fieldInfo.Name);
                        }
                    }
                    _textureImporterTypeOptions = list.ToArray();
                }
                return _textureImporterTypeOptions;
            }
        }

        protected override void OnInspectorGUI_Custom()
        {
            var settings = target as TextureImporterConfig;

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

            //通用基础
            this.GUIPropertyFieldWithOverride_Enum("textureType", () => settings.TextureType);
            this.GUIPropertyFieldWithOverride_Bool("sRgbColorTexture", () => settings.SRgbColorTexture);
            this.GUIPropertyFieldWithOverride_Bool("alphaIsTransparency", () => settings.AlphaIsTransparency);
            this.GUIPropertyFieldWithOverride_Enum("powerOf2", () => settings.PowerOf2);
            this.GUIPropertyFieldWithOverride_Bool("isReadable", () => settings.IsReadable);
            this.GUIPropertyFieldWithOverride_Bool("miniMap", () => settings.MiniMap);
            this.GUIPropertyFieldWithOverride_Enum("wrapMode", () => settings.WrapMode);
            this.GUIPropertyFieldWithOverride_Enum("filterMode", () => settings.FilterMode);

            //通用贴图格式相关
            this.GUIPropertyFieldWithOverride_Enum("maxSize", () => settings.MaxSize);
            this.GUIPropertyFieldWithOverride_Enum("compression", () => settings.Compression);

            //Sprite
            this.GUIPropertyFieldWithOverride_Enum("spriteImportMode", () => settings.SpriteImportMode);
            this.GUIPropertyFieldWithOverride_Vector2("spritePivot", () => settings.SpritePivot);

            //platforms
            this.GUIPropertyFieldWithOverride_TextureImporterPlatformSettings("defaultPlatform", ()=>settings.DefaultPlatform, settings.TextureType, 0);

            //this.GUIPropertyFieldWithOverride_String("assetBundlePackage", () => settings.AssetBundlePackage);
            //this.GUIPropertyFieldWithOverride_Bool("isRelease", () => settings.IsRelease);
            //this.GUIPropertyFieldWithOverride_Bool("isDevelopment", () => settings.IsDevelopment);
            //this.GUIPropertyFieldWithOverride_StringMultiLine("scriptingDefineSymbols", () => settings.ScriptingDefineSymbols);
            //this.GUIPropertyFieldWithOverride_StringMultiLine("extraScriptingDefineSymbols", () => settings.ExtraScriptingDefineSymbols);
        }
    }
}