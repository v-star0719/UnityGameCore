using System;
using System.Collections;
using UnityEngine;

namespace GameCore
{
    public interface IResManager
    {
        public UnityEngine.Object GetAsset(string name, Type type);
        public T GetAsset<T>(string name) where T : UnityEngine.Object;
        public GameObject GetGameObject(string name);
        public TextAsset GetTextAsset(string name);
        public Material GetMaterial(string name);
        public Texture GetTexture(string resName);
    }
}