using System;
using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    //fieldName是字段迷宫
    //<fieldName>Override 控制重写方式，可以是bool或者OverrideType枚举
    //parent字段 是父对象，属性从这个父对象继承
    //getParentValue 获取父对象里的值
    public partial class EditorGUIUtil
    {
        public static void GUIPropertyFieldWithOverride_String(this Editor editor, string fieldName, Func<string> getParentValue)
        {
            OverridePropertyDrawerString.Inst.OnGUI(editor.serializedObject, fieldName, getParentValue);
        }

        public static void GUIPropertyFieldWithOverride_Bool(this Editor editor, string fieldName, Func<bool> getParentValue)
        {
            OverridePropertyDrawerBool.Inst.OnGUI(editor.serializedObject, fieldName, getParentValue);
        }

        public static void GUIPropertyFieldWithOverride_Int(this Editor editor, string fieldName, Func<int> getParentValue)
        {
            OverridePropertyDrawerInt.Inst.OnGUI(editor.serializedObject, fieldName, getParentValue);
        }

        public static void GUIPropertyFieldWithOverride_Popup(this Editor editor, string fieldName, string[] options, Func<string> getParentValue)
        {
            OverridePropertyDrawerPopupString.Inst.OnGUI(editor.serializedObject, fieldName, getParentValue, options);
        }

        public static void GUIPropertyFieldWithOverride_Enum(this Editor editor, string fieldName, Func<Enum> getParentValue)
        {
            OverridePropertyDrawerEnum.Inst.OnGUI(editor.serializedObject, fieldName, getParentValue);
        }

        public static void GUIPropertyFieldWithOverride_Vector2(this Editor editor, string fieldName, Func<Vector2> getParentValue)
        {
            OverridePropertyDrawerVector2.Inst.OnGUI(editor.serializedObject, fieldName, getParentValue);
        }

        public static void GUIPropertyFieldWithOverride_TextureImporterPlatformSettings(this Editor editor, string fieldName, Func<TextureImporterPlatformSettings> getParentValue, TextureImporterType importerType, BuildTarget buildTarget)
        {
            OverridePropertyDrawerTextureImporterPlatformSettings.Inst.OnGUI(editor.serializedObject, fieldName, getParentValue, importerType, buildTarget);
        }
    }
}