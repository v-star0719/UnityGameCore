using System;
using UnityEditor;
using UnityEngine;

#if NGUI
namespace GameCore.Unity
{
    public class SpriteUtilsBaseNgui
    {
        public static void SetSprite(UISprite sprite, string name, Func<string, NGUIAtlas> loadAtlas)
        {
            if(string.IsNullOrEmpty(name))
            {
                Debug.LogError("sprite name is null or empty");
                return;
            }
            //atlasName_spriteName
            var index = name.IndexOf('_');
            if(index < 0)
            {
                Debug.LogError("sprite name is not right. name = atlasName_spriteName. this is: " + name);
                return;
            }

            var atlasName = name.Substring(0, index);
            if(sprite.atlasName != atlasName)
            {
                sprite.atlas = loadAtlas(atlasName);
            }
            sprite.spriteName = name;
        }
        
        public static void SetSpriteGray(UISprite sprite, bool gray)
        {
            sprite.color = gray ? Color.black : Color.white;
        }

        public static void SetTextureGray(UITexture texture, bool gray)
        {
            texture.color = gray ? Color.black : Color.white;
        }

    }
}
#endif