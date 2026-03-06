using UnityEngine;

namespace GameCore.Unity.Animator
{
    public class AnimatorUtils
    {
        public static float GetClipDuration(UnityEngine.Animator an, string clipName)
        {
            foreach(var clip in an.runtimeAnimatorController.animationClips)
            {
                if(clip.name == clipName)
                {
                    return clip.length;
                }
            }

            return 0;
        }

        public static AnimationClip GetClip(UnityEngine.Animator an, string clipName)
        {
            foreach(var clip in an.runtimeAnimatorController.animationClips)
            {
                if(clip.name == clipName)
                {
                    return clip;
                }
            }

            return null;
        }
    }
}