using UnityEngine;

public class AnimatorUtils
{
    public static float GetClipDuration(Animator an, string clipName)
    {
        foreach (var clip in an.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip.length;
            }
        }

        return 0;
    }
    public static AnimationClip GetClip(Animator an, string clipName)
    {
        foreach (var clip in an.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
            {
                return clip;
            }
        }

        return null;
    }
}
