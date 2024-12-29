using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace GameCore.Unity
{
    //场景一加载就会unity就会触发atlasRequested，但是这时候AssetBundle系统还没初始化，无法加载图集。
    //因此等待AssetBundle初始化后加载。
    //从编辑场景进入游戏的话，编辑场景里也挂载这个组件就行了。
    public class SpriteAtlasLoader : MonoBehaviour
    {
        //缓存Unity的请求
        private static Dictionary<string, Action<SpriteAtlas>> requestAtlasCallbacks = new();

        private IResManager resManager;

        //AssetBundle系统初始化后, 设置ResManager
        public void SetResManager(IResManager resManager)
        {
            this.resManager = resManager;
            foreach(var kv in requestAtlasCallbacks)
            {
                RequestAtlas(kv.Key, kv.Value);
            }
            requestAtlasCallbacks.Clear();
        }

        private void OnEnable()
        {
            SpriteAtlasManager.atlasRequested += RequestAtlas;
        }

        private void OnDisable()
        {
            SpriteAtlasManager.atlasRequested -= RequestAtlas;
        }

        //AssetBundle系统初始化后，resManager不为null
        protected virtual void RequestAtlas(string atlasName, Action<SpriteAtlas> callback)
        {
            if(resManager != null)
            {
                callback(ResManager.Instance.GetAsset<SpriteAtlas>(atlasName));
            }
            else
            {
                requestAtlasCallbacks.Add(atlasName, callback);
            }
        }
    }
}