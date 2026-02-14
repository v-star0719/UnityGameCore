using System.Collections;
using Fight;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.UGUIEx
{

    [AddComponentMenu("UI/UV Transform Image (Unity 6 Final)")]
    [DisallowMultipleComponent]
    public class UIUVTransformImage : Image
    {
        public enum UVRotationType
        {
            Rotate0 = 0,
            Rotate90 = 90,
            Rotate180 = 180,
            Rotate270 = 270
        }

        [Header("UV变换设置 (Unity 6)")]
        [Tooltip("UV旋转角度（不修改RectTransform）")]
        public UVRotationType uvRotation = UVRotationType.Rotate0;

        [Tooltip("水平翻转UV")]
        public bool flipHorizontal = false;

        [Tooltip("垂直翻转UV")]
        public bool flipVertical = false;

        // 检测Sprite切换
        private Sprite _lastSprite;
        // 标记：下一帧再刷新顶点，避免重建循环
        private bool _needRefreshVerticesNextFrame = false;

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            // 核心修复1：移除OnPopulateMesh内的SetVerticesDirty()
            base.OnPopulateMesh(vh);

            if(sprite == null || type != Type.Simple || vh.currentVertCount == 0 || sprite.uv == null || sprite.uv.Length < 4)
            {
                _lastSprite = null;
                return;
            }

            // 仅记录Sprite切换，不立即刷新
            if(_lastSprite != sprite)
            {
                _lastSprite = sprite;
                _needRefreshVerticesNextFrame = true; // 标记下一帧刷新
            }

            // 正常执行UV变换逻辑
            ModifyAtlasUVs(vh);
        }

        /// <summary>
        /// 适配Unity 6的sprite.uv（Vector2[]）+ 图集UV
        /// </summary>
        private void ModifyAtlasUVs(VertexHelper vh)
        {
            Vector2[] spriteUVs = sprite.uv;
            float uvMinX = Mathf.Min(spriteUVs[0].x, spriteUVs[1].x, spriteUVs[2].x, spriteUVs[3].x);
            float uvMinY = Mathf.Min(spriteUVs[0].y, spriteUVs[1].y, spriteUVs[2].y, spriteUVs[3].y);
            float uvMaxX = Mathf.Max(spriteUVs[0].x, spriteUVs[1].x, spriteUVs[2].x, spriteUVs[3].x);
            float uvMaxY = Mathf.Max(spriteUVs[0].y, spriteUVs[1].y, spriteUVs[2].y, spriteUVs[3].y);
            float uvWidth = uvMaxX - uvMinX;
            float uvHeight = uvMaxY - uvMinY;

            if(uvWidth <= 0 || uvHeight <= 0) return;

            UIVertex vert = UIVertex.simpleVert;
            for(int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vert, i);
                Vector2 originalUV = vert.uv0;

                float normalizedX = (originalUV.x - uvMinX) / uvWidth;
                float normalizedY = (originalUV.y - uvMinY) / uvHeight;
                Vector2 normalizedUV = new Vector2(normalizedX, normalizedY);

                Vector2 transformedUV = normalizedUV;
                switch(uvRotation)
                {
                    case UVRotationType.Rotate90:
                        transformedUV = new Vector2(normalizedUV.y, 1 - normalizedUV.x);
                        break;
                    case UVRotationType.Rotate180:
                        transformedUV = new Vector2(1 - normalizedUV.x, 1 - normalizedUV.y);
                        break;
                    case UVRotationType.Rotate270:
                        transformedUV = new Vector2(1 - normalizedUV.y, normalizedUV.x);
                        break;
                }
                if(flipHorizontal) transformedUV.x = 1 - transformedUV.x;
                if(flipVertical) transformedUV.y = 1 - transformedUV.y;

                float newUVX = uvMinX + (transformedUV.x * uvWidth);
                float newUVY = uvMinY + (transformedUV.y * uvHeight);
                vert.uv0 = new Vector2(newUVX, newUVY);

                vh.SetUIVertex(vert, i);
            }
        }

        /// <summary>
        /// 核心修复2：延迟刷新顶点，避免重建循环
        /// </summary>
        public void MarkVerticesDirtyNextFrame()
        {
            _needRefreshVerticesNextFrame = true;
        }

        // 每帧检测延迟刷新标记，在重建循环外执行刷新
        private void LateUpdate()
        {
            if(_needRefreshVerticesNextFrame && gameObject.activeInHierarchy && enabled)
            {
                _needRefreshVerticesNextFrame = false;
                // 在LateUpdate中刷新，此时重建循环已结束
                SetVerticesDirty();
            }
        }

        #region 全版本兼容生命周期（延迟刷新）
        protected override void OnValidate()
        {
            base.OnValidate();

            if(type != Type.Simple)
            {
                type = Type.Simple;
                Debug.LogWarning("[UIUVTransformImage] 仅支持ImageType=Simple，已自动切换", this);
            }

            MarkVerticesDirtyNextFrame(); // 延迟刷新
        }

        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();
            MarkVerticesDirtyNextFrame(); // 延迟刷新
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            _lastSprite = null;
            MarkVerticesDirtyNextFrame(); // 延迟刷新
        }

        private void Update()
        {
            if(_lastSprite != sprite)
            {
                _lastSprite = sprite;
                MarkVerticesDirtyNextFrame(); // 延迟刷新
            }
        }
        #endregion
    }
}