using UnityEngine;

namespace GameCore.Unity
{
    public static class SkinnedMeshRendererUtils
    {
        public static Bounds GetBoundsByBones(SkinnedMeshRenderer render)
        {
            var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            foreach (var bone in render.bones)
            {
                var pos = bone.position;
                if (min.x > pos.x)
                {
                    min.x = pos.x;
                }
                if (min.y > pos.y)
                {
                    min.y = pos.y;
                }
                if (min.z > pos.z)
                {
                    min.z = pos.z;
                }
                if (max.x < pos.x)
                {
                    max.x = pos.x; 
                }
                if (max.y < pos.y)
                {
                    max.y = pos.y;
                }
                if (max.z < pos.z)
                {
                    max.z = pos.z;
                }
            }

            var size = max - min;
            return new Bounds(min + size * 0.5f, size);
        }
    }
}
