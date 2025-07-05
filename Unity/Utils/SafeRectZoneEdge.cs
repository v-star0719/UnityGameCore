using UnityEngine;

namespace GameCore.Unity
{
    //矩形安全区边界。目前不支持旋转
    //目标靠近边界后，边界透明度会变亮
    [ExecuteAlways]
    public class SafeRectZoneEdge : MonoBehaviour
    {
        public enum SafeSide
        {
            Inner,
            Outer,
            Both,
        }
        public GameObject leftEdge;
        public GameObject rightEdge;
        public GameObject topEdge;
        public GameObject bottomEdge;
        public float warnDistance = 1;//到边界的距离小于这个值时，边界亮起
        public string shaderKey;
        public Color color;
        public SafeSide safeSide;
        public Transform target;

        private Material leftMaterial;
        private Material rightMaterial;
        private Material topMaterial;
        private Material bottomMaterial;
        private float left;
        private float right;
        private float top;
        private float bottom;
        public float width = 1;
        public float height = 1;

        private float warnLeft;
        private float warnRight;
        private float warnTop;
        private float warnBottom;

        private float leftAlpha;
        private float rightAlpha;
        private float topAlpha;
        private float bottomAlpha;
        private int shaderKeyId;

#if UNITY_EDITOR
        private bool changed;
#endif

        public void Set(Vector3 pos, float width, float height, Transform target)
        {
            transform.position = pos;
            this.target = target;
            this.width = width;
            this.height = height;
            
            Init();
            Update();
        }

        public void Update()
        {
            if (leftEdge == null || rightEdge == null || topEdge == null || bottomEdge == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (changed)
            {
                Init();
            }
#endif

            if (target == null)
            {
                return;
            }

            var targetPos = target.position;
            switch (safeSide)
            {
                case SafeSide.Inner:
                    CheckInner(left, targetPos.x, leftMaterial, ref leftAlpha);
                    CheckInner(targetPos.x, right, rightMaterial, ref rightAlpha);
                    CheckInner(targetPos.z, top, topMaterial, ref topAlpha);
                    CheckInner(bottom, targetPos.z, bottomMaterial, ref bottomAlpha);
                    break;
                case SafeSide.Outer:
                    var isInside = left <= targetPos.x && targetPos.x <= right && bottom <= targetPos.z && targetPos.z <= top;
                    CheckOuter(targetPos.x, left, width, isInside, leftMaterial, ref leftAlpha);
                    CheckOuter(right, targetPos.x, width, isInside, rightMaterial, ref rightAlpha);
                    CheckOuter(top, targetPos.z, height, isInside, topMaterial, ref topAlpha);
                    CheckOuter(targetPos.z, bottom, height, isInside, bottomMaterial, ref bottomAlpha);
                    break;
                case SafeSide.Both:
                    var isInWarnArea = left - warnDistance <= targetPos.x &&  targetPos.x <= right + warnDistance && 
                                       bottom - warnDistance <= targetPos.z && targetPos.z <= top + warnDistance &&
                                       (left + warnDistance > targetPos.x || targetPos.x > right - warnDistance ||
                                        bottom + warnDistance > targetPos.z || targetPos.z > top - warnDistance);
                    CheckTwoSide(left, targetPos.x, isInWarnArea, leftMaterial, ref leftAlpha);
                    CheckTwoSide(right, targetPos.x, isInWarnArea, rightMaterial, ref rightAlpha);
                    CheckTwoSide(top, targetPos.z, isInWarnArea, topMaterial, ref topAlpha);
                    CheckTwoSide(bottom, targetPos.z, isInWarnArea, bottomMaterial, ref bottomAlpha);
                    break;
            }
        }

        //a是目标位置，b是边界。a < b
        //为方便处理，都按这种情形处理：安全区在小于b的那一侧，这样b-a得到是正数说明在安全的那一侧。负数说明越过边界。
        private void CheckInner(float pos, float edge, Material mat, ref float lastAlpha)
        {
            var d = edge - pos;
            float f;
            if (d < 0)
            {
                f = 1;
            }
            else if (d > warnDistance)
            {
                f = 0;
            }
            else
            {
                f = 1 - d / warnDistance;
            }

            if (lastAlpha != f)
            {
                lastAlpha = f;
                color.a = f;
                mat.SetColor(shaderKeyId, color);
            }
        }

        //a是目标位置，b是边界。a < b
        //为方便处理，都按这种情形处理：安全区在大于b的那一侧。b-a得到的是正数，说明在安全的那一侧。负数说明越过边界。
        //如果越过边界较远，超出了另一个边界，则又是在外面了
        //如果越过边界不是很远，卡在两个边界中间，但是不在区域内，也不能亮起
        private void CheckOuter(float pos, float edge, float size, bool isInside, Material m, ref float lastAlpha)
        {
            var d = edge - pos;
            float f;
            if (d < -size)
            {
                f = 0;
            }
            else if (d < 0)
            {
                f = isInside ? 1 : 0;
            }
            else if (d > warnDistance)
            {
                f = 0;
            }
            else
            {
                f = 1 - d / warnDistance;
            }

            if (lastAlpha != f)
            {
                lastAlpha = f;
                color.a = f;
                m.SetColor(shaderKeyId, color);
            }
        }

        private void CheckTwoSide(float pos, float edge, bool isInWarnArea, Material mat, ref float lastAlpha)
        {
            var f = 0f;
            if (isInWarnArea)
            {
                var d = Mathf.Abs(edge - pos);
                f = d < warnDistance ? d / warnDistance : 0;
            }
            
            if (lastAlpha != f)
            {
                lastAlpha = f;
                color.a = f;
                mat.SetColor(shaderKeyId, color);
            }
        }

        private void Init()
        {
            shaderKeyId = Shader.PropertyToID(shaderKey);

            var pos = transform.position;
            var halfWidth = width * 0.5f;
            var halfHeight = height * 0.5f;
            left = pos.x - halfWidth;
            right = pos.x + halfWidth;
            top = pos.y + halfHeight;
            bottom = pos.y - halfHeight;

            leftEdge.transform.localPosition = new Vector3(-halfWidth, 0, 0);
            rightEdge.transform.localPosition = new Vector3(halfWidth, 0, 0);
            topEdge.transform.localPosition = new Vector3(0, 0, halfHeight);
            bottomEdge.transform.localPosition = new Vector3(0, 0, -halfHeight);
            leftEdge.transform.localScale = new Vector3(height, 1, 1);
            rightEdge.transform.localScale = new Vector3(height, 1, 1);
            topEdge.transform.localScale = new Vector3(width, 1, 1);
            bottomEdge.transform.localScale = new Vector3(width, 1, 1);

            leftAlpha = -1;
            rightAlpha = -1;
            topAlpha = -1;
            bottomAlpha = -1;

            if (!Application.isEditor || !Application.isPlaying || GameObjectUtils.IsEditingInPrefabMode(gameObject))
            {
                leftMaterial = leftEdge.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                rightMaterial = rightEdge.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                topMaterial = topEdge.GetComponentInChildren<MeshRenderer>().sharedMaterial;
                bottomMaterial = bottomEdge.GetComponentInChildren<MeshRenderer>().sharedMaterial;
            }
            else
            {
                leftMaterial = leftEdge.GetComponentInChildren<MeshRenderer>().material;
                rightMaterial = rightEdge.GetComponentInChildren<MeshRenderer>().material;
                topMaterial = topEdge.GetComponentInChildren<MeshRenderer>().material;
                bottomMaterial = bottomEdge.GetComponentInChildren<MeshRenderer>().material;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, new Vector3(width, 0f, height));
            Gizmos.color = Color.green;
            if(safeSide is SafeSide.Inner or SafeSide.Both)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(width - warnDistance * 2, 0f, height - warnDistance * 2));
            }
            if(safeSide is SafeSide.Outer or SafeSide.Both)
            {
                Gizmos.DrawWireCube(transform.position, new Vector3(width + warnDistance * 2, 0f, height + warnDistance * 2));
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            changed = true;
        }
#endif
    }
}
