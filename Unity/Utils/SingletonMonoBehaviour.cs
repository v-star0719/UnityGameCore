using System;
using UnityEngine;

namespace GameCore.Unity
{
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : class
    {
        public static T Inst { get; private set;}

        protected virtual void Awake()
        {
            if (Inst != null)
            {
                throw new Exception($"{name} is running!");
            }
            Inst = this as T;
        }

        protected virtual void OnDestroy()
        {
            Inst = null;
        }
    }
}