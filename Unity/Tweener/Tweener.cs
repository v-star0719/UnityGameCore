using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

namespace GameCore.Unity.Tweener
{
	public abstract class Tweener : MonoBehaviour
	{
		public enum Method
		{
			Curve,
			Linear,
			EaseIn,
			EaseOut,
			EaseInOut,
			BounceIn,
			BounceOut,
		}

		public enum Style
		{
			Once,
			Loop,
			PingPong,
		}

		public enum Direction
        {
            Forward,
			Backward,
        }

		public Method method = Method.Linear;
		public Style style = Style.Once;
		public AnimationCurve animationCurve;

		public bool ignoreTimeScale = false;
		public float delay = 0f;
		public float duration = 1f;
		public bool useFixedUpdate = false;

        public UnityEvent onFinished;
        public bool playOnAwake;//true表示每次enable后都自动播放。现在是从头开始播

        private Direction direction;
        private float delayTimer = 0;
        private float timer = -1;
        private Action onFinishCallback;//允许调用的时候传参，方便处理

        protected virtual void Start()
        {
            if (method == Method.Curve && animationCurve == null)
            {
				Debug.LogError("no animation curve on tweener", gameObject);
            }

            if (timer < 0 && !playOnAwake)
            {
                enabled = false;
            }
        }

        protected void Update()
        {
            if (!useFixedUpdate) DoUpdate(ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
        }

        protected void FixedUpdate()
        {
            if (useFixedUpdate) DoUpdate(ignoreTimeScale ? Time.fixedUnscaledDeltaTime : Time.fixedDeltaTime);
        }

        protected virtual void OnEnable()
        {
            if(playOnAwake && timer < 0)//如果timer>=0说明已经开始播放了，不重新播放，继续播放即可。可能是用户手动播放的
            {
                Play(true);
            }
        }

        protected void OnDisable()
        {
            if(playOnAwake)
            {
                timer = -1;
            }
        }

		protected void DoUpdate (float deltaTime)
		{
            if (delayTimer < delay)
            {
                delayTimer += deltaTime;
                return;
            }

            float f = 0;
            var isFinished = false;
            if (direction == Direction.Forward)
            {
                timer += deltaTime;
                if (timer >= duration)
                {
                    f = 1;
                    timer = duration;
                    isFinished = true;
                }
                else
                {
                    f = timer / duration;
                }
            }
            else
            {
				timer -= deltaTime;
                if(timer <= 0)
                {
                    f = 0;
                    timer = 0;
                    isFinished = true;
                }
                else
                {
                    f = timer / duration;
                }
            }
            Sample(f);

            if(isFinished)
            {
                if (style == Style.Once)
                {
					OnFinished();
                }
				else if (style == Style.Loop)
                {
                    timer = direction == Direction.Forward ? 0 : 1;
                }
                else
                {
                    direction = direction == Direction.Forward ? Direction.Backward : Direction.Forward;
                }
            }
		}

        private void OnFinished()
        {
            enabled = false;
            onFinished.Invoke();
            onFinishCallback?.Invoke();
        }

		public void Stop()
		{
            enabled = false;
        }

        public void Sample (float factor)
		{
			float val = Mathf.Clamp01(factor);

            switch (method)
            {
				case Method.EaseIn:
                    val = 1f - Mathf.Sin(0.5f * Mathf.PI * (1f - val));
                    val *= val;
                    break;
                case Method.EaseOut:
				    val = Mathf.Sin(0.5f * Mathf.PI * val);
                    val = 1f - val;
                    val = 1f - val * val;
                    break;
                case Method.EaseInOut:
                    const float pi2 = Mathf.PI * 2f;
                    val = val - Mathf.Sin(val * pi2) / pi2;

                    val = val * 2f - 1f;
                    float sign = Mathf.Sign(val);
                    val = 1f - Mathf.Abs(val);
                    val = 1f - val * val;
                    val = sign * val * 0.5f + 0.5f;
                    break;
                case Method.BounceIn:
				    val = BounceLogic(val);
                    break;
                case Method.BounceOut:
				    val = 1f - BounceLogic(1f - val);
                    break;
                case Method.Curve:
                    val = animationCurve.Evaluate(val);
                    break;
            }
			OnUpdate(val);
		}

		/// <summary>
		/// Main Bounce logic to simplify the Sample function
		/// </summary>
		
		float BounceLogic (float val)
		{
			if (val < 0.363636f) // 0.363636 = (1/ 2.75)
			{
				val = 7.5685f * val * val;
			}
			else if (val < 0.727272f) // 0.727272 = (2 / 2.75)
			{
				val = 7.5625f * (val -= 0.545454f) * val + 0.75f; // 0.545454f = (1.5 / 2.75) 
			}
			else if (val < 0.909090f) // 0.909090 = (2.5 / 2.75) 
			{
				val = 7.5625f * (val -= 0.818181f) * val + 0.9375f; // 0.818181 = (2.25 / 2.75) 
			}
			else
			{
				val = 7.5625f * (val -= 0.9545454f) * val + 0.984375f; // 0.9545454 = (2.625 / 2.75) 
			}
			return val;
		}

		public virtual void Play (bool forward, Action onFinish = null)
        {
            enabled = true;
            timer = forward ? 0 : duration;
            delayTimer = 0;
            direction = forward ? Direction.Forward : Direction.Backward;
            onFinishCallback = onFinish;
			Sample(forward ? 0 : 1);
        }

        public virtual void PlayFromCurrentPos(bool forward, Action onFinish = null)
        {
            if (!enabled)
            {
                delayTimer = 0;//说明之前已经播放完成了
            }
            enabled = true;
            direction = forward ? Direction.Forward : Direction.Backward;
            onFinishCallback = onFinish;
        }

public void ResetToBeginning ()
		{
			Sample(0);
		}

		public void ResetToEnd ()
		{
			Sample(1);
		}
		
        protected abstract void OnUpdate (float factor);
		
	//	static public T Begin<T> (GameObject go, float duration, float delay = 0f) where T : UITweener
	//	{
	//		T comp = go.GetComponent<T>();
	//#if UNITY_FLASH
	//		if ((object)comp == null) comp = (T)go.AddComponent<T>();
	//#else
	//		// Find the tween with an unset group ID (group ID of 0).
	//		if (comp != null && comp.tweenGroup != 0)
	//		{
	//			comp = null;
	//			T[] comps = go.GetComponents<T>();
	//			for (int i = 0, imax = comps.Length; i < imax; ++i)
	//			{
	//				comp = comps[i];
	//				if (comp != null && comp.tweenGroup == 0) break;
	//				comp = null;
	//			}
	//		}

	//		if (comp == null)
	//		{
	//			comp = go.AddComponent<T>();

	//			if (comp == null)
	//			{
	//				Debug.LogError("Unable to add " + typeof(T) + " to " + NGUITools.GetHierarchy(go), go);
	//				return null;
	//			}
	//		}
	//#endif
	//		comp.mStarted = false;
	//		comp.mFactor = 0f;
	//		comp.duration = duration;
	//		comp.mDuration = duration;
	//		comp.delay = delay;
	//		comp.mAmountPerDelta = duration > 0f ? Mathf.Abs(1f / duration) : 1000f;
	//		comp.style = Style.Once;
	//		comp.animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
	//		comp.eventReceiver = null;
	//		comp.callWhenFinished = null;
	//		comp.onFinished.Clear();
	//		if (comp.mTemp != null) comp.mTemp.Clear();
	//		comp.enabled = true;
	//		return comp;
	//	}
    }
}