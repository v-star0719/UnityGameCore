#if NGUI

using GameCore.Unity.UGUIEx;
using TMPro;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.Unity.NGUIEx
{
    public class NGUI2UGUI
    {
        private void NGUIToUGUI(Transform tran)
        {

            var go = tran.gameObject;
            var rectTrans = tran.GetComponent<RectTransform>();
            if(rectTrans == null)
            {
                rectTrans = go.AddComponent<RectTransform>();
            }

            //widget
            var widget = go.GetComponent<UIWidget>();
            if(widget != null)
            {
                if(typeof(UIWidget) == widget.GetType())
                {
                    Object.DestroyImmediate(widget);
                    //if (widget.bottomAnchor.relative)
                    //{
                    //    rectTrans.anchorMin = new Vector2(1f, 0.5f);
                    //    rectTrans.anchorMax = new Vector2(1f, 0.5f);
                    //}
                    //else if (widget.pivot == UIWidget.Pivot.Right)
                    //{
                    //    rectTrans.anchorMin = new Vector2(1f, 0.5f);
                    //    rectTrans.anchorMax = new Vector2(1f, 0.5f);
                    //}
                    //else if (widget.pivot == UIWidget.Pivot.Top)
                    //{
                    //    rectTrans.anchorMin = new Vector2(0.5f, 1f);
                    //    rectTrans.anchorMax = new Vector2(0.5f, 1f);
                    //}
                    //else if (widget.pivot == UIWidget.Pivot.Bottom)
                    //{
                    //    rectTrans.anchorMin = new Vector2(0.5f, 0f);
                    //    rectTrans.anchorMax = new Vector2(0.5f, 0f);
                    //}
                    //else if (widget.pivot == UIWidget.Pivot.TopLeft)
                    //{
                    //    rectTrans.anchorMin = new Vector2(0f, 1f);
                    //    rectTrans.anchorMax = new Vector2(0f, 1f);
                    //}
                    //else if (widget.pivot == UIWidget.Pivot.TopRight)
                    //{
                    //    rectTrans.anchorMin = new Vector2(1f, 1f);
                    //    rectTrans.anchorMax = new Vector2(1f, 1f);
                    //}
                    //else if(widget.pivot == UIWidget.Pivot.BottomLeft)
                    //{
                    //    rectTrans.anchorMin = new Vector2(0f, 0f);
                    //    rectTrans.anchorMax = new Vector2(0f, 0f);
                    //}
                    //else if(widget.pivot == UIWidget.Pivot.BottomRight)
                    //{
                    //    rectTrans.anchorMin = new Vector2(1f, 0f);
                    //    rectTrans.anchorMax = new Vector2(1f, 0f);
                    //}
                }

                rectTrans.sizeDelta = new Vector2(widget.width, widget.height);
            }

            //sprite
            var uiSprite = go.GetComponent<UISprite>();
            if(uiSprite != null)
            {
                var image = go.AddComponent<Image>();
                var spritePath = $"Assets/Atlas/{(uiSprite.atlas as NGUIAtlas)?.name}/{uiSprite.spriteName}.png";
                image.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
                if(image.sprite == null)
                {
                    Debug.Log($"Sprite not found: {spritePath}");
                }
                if(uiSprite.type == UIBasicSprite.Type.Sliced)
                {
                    image.type = Image.Type.Sliced;
                    image.fillCenter = uiSprite.centerType != UIBasicSprite.AdvancedType.Invisible;
                }
                image.color = uiSprite.color;
                Object.DestroyImmediate(uiSprite);
            }

            //label
            var uiLabel = go.GetComponent<UILabel>();
            if(uiLabel != null)
            {
                var text = go.AddComponent<TextMeshProUGUI>();
                var fontPath = $"Assets/BundleRes/UI/Font/NotoSansSC-Regular SDF.asset";
                text.font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontPath);
                if(uiLabel.alignment == NGUIText.Alignment.Automatic)
                {
                    if(uiLabel.pivot == UIWidget.Pivot.Center)
                    {
                        text.alignment = TextAlignmentOptions.Center;
                    }
                    else if(uiLabel.pivot == UIWidget.Pivot.Left)
                    {
                        text.alignment = TextAlignmentOptions.Left;
                    }
                    else if(uiLabel.pivot == UIWidget.Pivot.Right)
                    {
                        text.alignment = TextAlignmentOptions.Right;
                    }
                }
                text.color = uiLabel.color;
                text.fontSize = uiLabel.fontSize;
                text.text = uiLabel.text;
                text.textWrappingMode = uiLabel.overflowMethod == UILabel.Overflow.ResizeFreely ? TextWrappingModes.NoWrap : TextWrappingModes.Normal;
                var nguiL10n = go.GetComponent<UIL10n>();
                if(nguiL10n != null)
                {
                    var uguiL10n = go.AddComponent<GameCore.Unity.UGUIEx.UIL10n>();
                    uguiL10n.key = nguiL10n.key;
                    Object.DestroyImmediate(nguiL10n);
                }
                Object.DestroyImmediate(uiLabel);
            }

            //texture
            var uiTexture = go.GetComponent<UITexture>();
            if(uiTexture != null)
            {
                var rawImage = go.AddComponent<RawImage>();
                rawImage.texture = uiTexture.mainTexture;
                rawImage.color = uiTexture.color;
                Object.DestroyImmediate(uiTexture);
            }

            //button
            var uiButton = go.GetComponent<UIButton>();
            var simpleButton = go.GetComponent<UISimpleButton>();
            var boxCollider = go.GetComponent<BoxCollider>();
            if(uiButton != null || simpleButton != null || boxCollider != null)
            {
                var button = go.AddComponent<Button>();
                button.transition = Selectable.Transition.Animation;
                var animator = go.AddComponent<Animator>();
                animator.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/BundleRes/UI/timeLine/Button.controller");
                if(simpleButton != null)
                {
                    Object.DestroyImmediate(simpleButton);
                }
                if(uiButton != null)
                {
                    Object.DestroyImmediate(uiButton);
                }
                if(boxCollider != null)
                {
                    Object.DestroyImmediate(boxCollider);
                }
                Debug.Log($"find Button or click ui", go);
            }
            var buttonSale = go.GetComponent<UIButtonScale>();
            if(buttonSale != null)
            {
                Object.DestroyImmediate(buttonSale);
            }
            var keyNavigation = go.GetComponent<UIKeyNavigation>();
            if(keyNavigation != null)
            {
                var uiNavigation = go.AddComponent<UINavigation>();
                uiNavigation.startsSelected = keyNavigation.startsSelected;
                Object.DestroyImmediate(keyNavigation);
            }
            var keyNavigationContainer = go.GetComponent<UIKeyNavigationContainer>();
            if(keyNavigationContainer != null)
            {
                go.AddComponent<UINavigationContainer>();
                Object.DestroyImmediate(keyNavigationContainer);
            }

            //Grid/Table
            var uiGrid = go.GetComponent<UIGrid>();
            var uiTable = go.GetComponent<UITable>();
            if(uiGrid != null || uiTable != null)
            {
                if(uiGrid != null && uiGrid.maxPerLine > 1)
                {
                    go.AddComponent<GridLayoutGroup>();
                }
                else if(uiGrid != null && uiGrid.arrangement == UIGrid.Arrangement.Vertical ||
                        uiTable != null && uiTable.columns != 0)
                {
                    var layoutGroup = go.AddComponent<VerticalLayoutGroup>();
                    layoutGroup.childForceExpandHeight = false;
                    layoutGroup.childForceExpandWidth = false;
                }
                else
                {
                    var layoutGroup = go.AddComponent<HorizontalLayoutGroup>();
                    layoutGroup.childForceExpandHeight = false;
                    layoutGroup.childForceExpandWidth = false;
                }

                if(uiGrid != null)
                {
                    Object.DestroyImmediate(uiGrid);
                }
                if(uiTable != null)
                {
                    Object.DestroyImmediate(uiTable);
                }
                Debug.Log($"find UIGrid or UITable", go);
            }

            //Progress
            var uiProgressBar = go.GetComponent<UIProgressBar>();
            if(uiProgressBar)
            {
                Object.DestroyImmediate(uiProgressBar);//需要手动设置引用的图
                Debug.Log($"find UIProgressBar", go);
            }

            //others
            var scrollView = go.GetComponent<UIScrollView>();
            if(scrollView != null)
            {
                var scrollRect = go.AddComponent<ScrollRect>();
                scrollRect.horizontal = scrollView.canMoveHorizontally;
                scrollRect.vertical = scrollView.canMoveVertically;
                Object.DestroyImmediate(scrollView);
                Debug.Log($"find UIScrollView", go);
            }
            var panel = go.GetComponent<UIPanel>();
            if(panel != null)
            {
                if(panel.hasClipping)
                {
                    rectTrans.sizeDelta = new Vector2(panel.baseClipRegion.z, panel.baseClipRegion.w);
                }
                Object.DestroyImmediate(panel);
                rectTrans.anchorMax = new Vector2(1, 1);
                rectTrans.anchorMin = new Vector2(0, 0);
            }
            var rigidbody = go.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                Object.DestroyImmediate(rigidbody);
            }
            if(widget != null)
            {
                Debug.Log($"Unsupported widget: {widget.name}", widget.gameObject);
            }
            var dragScrollView = go.GetComponent<UIDragScrollView>();
            if(dragScrollView != null)
            {
                Object.DestroyImmediate(dragScrollView);
            }
            if(go.GetComponent<UITweener>())
            {
                Debug.Log($"find UITweener", go);
            }

            for(int i = 0; i < go.transform.childCount; i++)
            {
                NGUIToUGUI(go.transform.GetChild(i));
            }
        }
    }
}

#endif
