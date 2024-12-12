using System.Collections;
using GameCore.Core;
using UnityEngine;

namespace GameCore
{
    /// 游戏主干
    public class GameBase : MonoBehaviour
    {
        public Transform topUIRoot;
        public Transform normalUIRoot;

        protected virtual void Awake()
        {
            LoggerX.AddTarget(new LogToUnity());
        }

        protected virtual void Start()
        {

        }

        protected virtual void Update()
        {
            var deltaTime = Time.deltaTime;
            EventManager.Instance.Tick(deltaTime);
            SceneManagerX.Inst.Tick(deltaTime);
        }

        protected virtual void OnDestroy()
        {
            LoggerX.Clear();
        }
    }
}