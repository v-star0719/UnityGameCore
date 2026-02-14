using System;
using System.Reflection;
using GameCore.Core;
using GameCore.Lang.Extension;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public enum OverrideType
    {
        Ignore,
        Customize,//自定义
        Inherit,//继承Parent的值
    }

    //fieldName是字段迷宫
    //<fieldName>Override 控制重写方式，可以是bool或者OverrideType枚举
    //parent字段 是父对象，属性从这个父对象继承
    //getParentValue 获取父对象里的值
    public class OverridePropertyDrawer
    {
        protected SerializedObject serializedObject;
        protected string fieldName;
        protected SerializedProperty controllProperty;
        protected string overrideFieldName => fieldName + "Override";

        protected void OnGUI(SerializedObject serializedObject, string fieldName)
        {
            this.serializedObject = serializedObject;
            this.fieldName = fieldName;

            controllProperty = serializedObject.FindProperty(overrideFieldName);
            switch (controllProperty.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    OnGUI_Bool();
                    break;
                case SerializedPropertyType.Enum:
                    OnGUI_OverrideType();
                    break;
                default:
                    Debug.LogError("Unknown control field type");
                    break;
            }
        }

        protected void OnGUI_Bool()
        {
            var parentProperty = serializedObject.FindProperty("parent");
            var parent = parentProperty.objectReferenceValue;
            if(parent == null)
            {
                GUISelfValue();
                return;
            }

            EditorGUIUtil.DrawSeparator();

            EditorGUILayout.PropertyField(controllProperty);
            if(controllProperty.boolValue)
            {
                GUISelfValue();
            }
            else
            {
                GUI.enabled = false;
                GUIParentValue();
                GUI.enabled = true;
            }
        }

        protected void OnGUI_OverrideType()
        {
            EditorGUIUtil.DrawSeparator();

            EditorGUILayout.PropertyField(controllProperty);

            var parentProperty = serializedObject.FindProperty("parent");
            var parent = parentProperty.objectReferenceValue;

            var overrideType = (OverrideType)controllProperty.intValue;
            if(overrideType == OverrideType.Ignore)
            {
                EditorGUILayout.LabelField(fieldName, "This option will not process, you can set it freely in the Inspector.");
            }
            else if(overrideType == OverrideType.Customize)
            {
                GUISelfValue();
            }
            else if(overrideType == OverrideType.Inherit)
            {
                GUI.enabled = false;
                if(parent == null)
                {
                    EditorGUILayout.LabelField(fieldName, "Please set parent.");
                }
                else
                {
                    var propertyName = StringUtils.FirstLetterToUpper(overrideFieldName);
                    PropertyInfo propertyInfo = serializedObject.targetObject.GetType().GetProperty(propertyName);
                    if (propertyInfo == null)
                    {
                        EditorGUILayout.LabelField(fieldName, $"Property getter {propertyName} is not found, which is used to get parent's override type.");
                    }
                    else
                    {
                        var finalOverrideType = (OverrideType)propertyInfo.GetValue(serializedObject.targetObject);
                        if(finalOverrideType == OverrideType.Ignore)
                        {
                            EditorGUILayout.LabelField(fieldName, "This option will not process, you can set it freely in the Inspector.");
                        }
                        else
                        {
                            GUIParentValue();
                        }
                    }
                }
                GUI.enabled = true;
            }
            else
            {
                EditorGUILayout.LabelField("Error", "Unknown override type");
            }
        }

        protected virtual void GUISelfValue()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
        }

        protected virtual void GUIParentValue()
        {
        }
    }

    public class OverridePropertyDrawerT<ValueT, ClassT> : OverridePropertyDrawer where ClassT : new()
    {
        public static readonly ClassT Inst = new ClassT();
        protected Func<ValueT> getParentValue;

        public virtual void OnGUI(SerializedObject serializedObject, string fieldName, Func<ValueT> parentValue)
        {
            getParentValue = parentValue;
            OnGUI(serializedObject, fieldName);
        }
    }

    public class OverridePropertyDrawerString : OverridePropertyDrawerT<string, OverridePropertyDrawerString>
    {
        protected override void GUIParentValue()
        {
            EditorGUILayout.TextField(fieldName.ToUpperFirst(), getParentValue());
        }
    }

    public class OverridePropertyDrawerBool : OverridePropertyDrawerT<bool, OverridePropertyDrawerBool>
    {
        protected override void GUIParentValue()
        {
            EditorGUILayout.Toggle(fieldName.ToUpperFirst(), getParentValue());
        }
    }

    public class OverridePropertyDrawerInt : OverridePropertyDrawerT<int, OverridePropertyDrawerInt>
    {
        protected override void GUIParentValue()
        {
            EditorGUILayout.IntField(fieldName.ToUpperFirst(), getParentValue());
        }
    }

    public class OverridePropertyDrawerPopupString : OverridePropertyDrawerT<string, OverridePropertyDrawerPopupString>
    {
        private string[] options;

        public virtual void OnGUI(SerializedObject serializedObject, string fieldName, Func<string> parentValue, string[] options)
        {
            this.options = options;
            OnGUI(serializedObject, fieldName, parentValue);
        }

        protected override void GUISelfValue()
        {
            EditorGUIUtil.PropertyField_Popup(serializedObject.FindProperty(fieldName), options);
        }

        protected override void GUIParentValue()
        {
            EditorGUILayout.Popup(fieldName.ToUpperFirst(), options.IndexOfEx(getParentValue()), options);
        }
    }

    public class OverridePropertyDrawerEnum : OverridePropertyDrawerT<Enum, OverridePropertyDrawerEnum>
    {
        protected override void GUIParentValue()
        {
            EditorGUILayout.EnumPopup(fieldName.ToUpperFirst(), getParentValue());
        }
    }

    public class OverridePropertyDrawerVector2 : OverridePropertyDrawerT<Vector2, OverridePropertyDrawerVector2>
    {
        protected override void GUIParentValue()
        {
            EditorGUILayout.Vector2Field(fieldName.ToUpperFirst(), getParentValue());
        }
    }

    public class OverridePropertyDrawerTextureImporterPlatformSettings : OverridePropertyDrawerT<TextureImporterPlatformSettings, OverridePropertyDrawerTextureImporterPlatformSettings>
    {
        private TextureImporterType importerType;
        private BuildTarget buildTarget;
        public virtual void OnGUI(SerializedObject serializedObject, string fieldName, Func<TextureImporterPlatformSettings> parentValue, TextureImporterType importerType, BuildTarget buildTarget)
        {
            this.importerType = importerType;
            this.buildTarget = buildTarget;
            OnGUI(serializedObject, fieldName, parentValue);
        }

        protected override void GUISelfValue()
        {
            EditorGUIUtil.PropertyField_TextureImporterPlatformSettings(serializedObject.FindProperty(fieldName), importerType, buildTarget);
        }

        protected override void GUIParentValue()
        {
            EditorGUIUtil.TextureImporterPlatformSettingsField(getParentValue(), importerType, buildTarget);
        }
    }
}