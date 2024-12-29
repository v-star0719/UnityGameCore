using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace GameCore.Unity
{
    //����һ���ؾͻ�unity�ͻᴥ��atlasRequested��������ʱ��AssetBundleϵͳ��û��ʼ�����޷�����ͼ����
    //��˵ȴ�AssetBundle��ʼ������ء�
    //�ӱ༭����������Ϸ�Ļ����༭������Ҳ���������������ˡ�
    public class SpriteAtlasLoader : MonoBehaviour
    {
        //����Unity������
        private static Dictionary<string, Action<SpriteAtlas>> requestAtlasCallbacks = new();

        private IResManager resManager;

        //AssetBundleϵͳ��ʼ����, ����ResManager
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

        //AssetBundleϵͳ��ʼ����resManager��Ϊnull
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