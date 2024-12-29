using System;
using UnityEditor;
using UnityEditor.Sprites;
using UnityEngine;

namespace GameCore.Unity.UGUIEx
{
    [CustomEditor(typeof(SpriteAnimation))]
    public class SpriteAnimationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var sa = target as SpriteAnimation;
            if (GUILayout.Button("AddSelectedSprites"))
            {
                foreach (var o in Selection.objects)
                {
                    if (o is Texture2D texture)
                    {
                        var array = new Sprite[sa.frames.Length + 1];
                        Array.Copy(sa.frames, array, sa.frames.Length);
                        array[sa.frames.Length] = AssetDatabase.LoadAssetAtPath<Sprite>(AssetDatabase.GetAssetPath(o));
                        sa.frames = array;
                    }
                }
            }

            if (GUILayout.Button("SortSprites"))
            {
                Array.Sort(sa.frames, (a, b) => String.Compare(a.name, b.name, StringComparison.Ordinal));
            }
        }
    }
}
