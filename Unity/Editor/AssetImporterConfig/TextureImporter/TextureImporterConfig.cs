using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameCore.Unity.EditorGUIUtil;

namespace GameCore.Unity
{
    public class TextureImporterConfig : AssetImporterConfigBase
    {
        //ITreeNode
        public override ITreeNode ParentNode => parent;
        //-------------------------------------------

        [SerializeField] private TextureImporterConfig parent;
        public TextureImporterConfig Parent => parent;

        //通用基础
        [SerializeField] private OverrideType textureTypeOverride;
        [SerializeField] private TextureImporterType textureType = TextureImporterType.Default;
        public TextureImporterType TextureType => textureTypeOverride == OverrideType.Override && Parent != null ? Parent.TextureType : textureType;
        //--
        [SerializeField] private OverrideType alphaIsTransparencyOverride;
        [SerializeField] private bool alphaIsTransparency;
        public bool AlphaIsTransparency => alphaIsTransparencyOverride == OverrideType.Override && Parent != null ? Parent.AlphaIsTransparency : alphaIsTransparency;
        //--
        [SerializeField] private OverrideType sRgbColorTextureOverride;
        [SerializeField] private bool sRgbColorTexture;
        public bool SRgbColorTexture => sRgbColorTextureOverride == OverrideType.Override && Parent != null ? Parent.SRgbColorTexture : sRgbColorTexture;
        //--
        [SerializeField] private OverrideType powerOf2Override;
        [SerializeField] private TextureImporterNPOTScale powerOf2 = TextureImporterNPOTScale.ToNearest;
        public TextureImporterNPOTScale PowerOf2 => powerOf2Override == OverrideType.Override && Parent != null ? Parent.PowerOf2 : powerOf2;
        //--
        [SerializeField] private OverrideType isReadableOverride;
        [SerializeField] private bool isReadable;
        public bool IsReadable => isReadableOverride == OverrideType.Override && Parent != null ? Parent.IsReadable : isReadable;
        //--
        [SerializeField] private OverrideType miniMapOverride;
        [SerializeField] private bool miniMap;
        public bool MiniMap => miniMapOverride == OverrideType.Override && Parent != null ? Parent.MiniMap : miniMap;
        //--
        [SerializeField] private OverrideType wrapModeOverride;
        [SerializeField] private TextureWrapMode wrapMode;
        public TextureWrapMode WrapMode => wrapModeOverride == OverrideType.Override && Parent != null ? Parent.WrapMode : wrapMode;
        //--
        [SerializeField] private OverrideType filterModeOverride;
        [SerializeField] private FilterMode filterMode;
        public FilterMode FilterMode => filterModeOverride == OverrideType.Override && Parent != null ? Parent.FilterMode: filterMode;

        //通用贴图格式相关
        [SerializeField] private OverrideType maxSizeOverride;
        [SerializeField] private TextureMaxSizeType maxSize = TextureMaxSizeType._2048;
        public TextureMaxSizeType MaxSize => maxSizeOverride == OverrideType.Override && Parent != null ? Parent.MaxSize : maxSize;
        //--
        [SerializeField] private OverrideType compressionOverride;
        [SerializeField] private TextureImporterCompression compression = TextureImporterCompression.Compressed;
        public TextureImporterCompression Compression => compressionOverride == OverrideType.Override && Parent != null ? Parent.Compression : compression;

        //Sprite
        [SerializeField] private OverrideType spriteImportModeOverride;
        [SerializeField] private SpriteImportMode spriteImportMode;
        public SpriteImportMode SpriteImportMode => spriteImportModeOverride == OverrideType.Override && Parent != null ? Parent.SpriteImportMode : spriteImportMode;
        //--
        [SerializeField] private OverrideType spriteMeshTypeOverride;
        [SerializeField] private SpriteMeshType spriteMeshType = SpriteMeshType.Tight;
        public SpriteMeshType SpriteMeshType => spriteMeshTypeOverride == OverrideType.Override && Parent != null ? Parent.SpriteMeshType : spriteMeshType;
        //--
        [SerializeField] private OverrideType spritePivotOverride;
        [SerializeField] private Vector2 spritePivot = new Vector2(0.5f, 0.5f);
        public Vector2 SpritePivot => spritePivotOverride == OverrideType.Override && Parent != null ? Parent.SpritePivot : spritePivot;

        [SerializeField] private OverrideType defaultPlatformOverride;
        [SerializeField] private TextureImporterPlatformSettings defaultPlatform;
        public TextureImporterPlatformSettings DefaultPlatform => defaultPlatformOverride == OverrideType.Override && Parent != null ? Parent.DefaultPlatform : defaultPlatform;

        //[SerializeField] private OverrideType standaloneSettingsOverride;
        //[SerializeField] private TextureImporterPlatformSettings standaloneSettings;
        //public TextureImporterPlatformSettings StandaloneSettings => standaloneSettingsOverride == OverrideType.Override && Parent != null ? Parent.StandaloneSettings : standaloneSettings;

        public override void Apply(AssetImporter im)
        {
            var importer = im as TextureImporter;
            //通用基础
            if(textureTypeOverride != OverrideType.Ignore)
            {
                importer.textureType = TextureType;
            }
            if(sRgbColorTextureOverride != OverrideType.Ignore)
            {
                importer.sRGBTexture = SRgbColorTexture;
            }
            if(alphaIsTransparencyOverride != OverrideType.Ignore)
            {
                importer.alphaIsTransparency = AlphaIsTransparency;
            }
            if(powerOf2Override != OverrideType.Ignore)
            {
                importer.npotScale = PowerOf2;
            }
            if(isReadableOverride != OverrideType.Ignore)
            {
                importer.isReadable = IsReadable;
            }
            if(miniMapOverride != OverrideType.Ignore)
            {
                importer.mipmapEnabled = MiniMap;
            }
            if(wrapModeOverride != OverrideType.Ignore)
            {
                importer.wrapMode = WrapMode;
            }
            if(filterModeOverride != OverrideType.Ignore)
            {
                importer.filterMode = filterMode;
            }

            //通用贴图格式相关
            if(maxSizeOverride != OverrideType.Specified && importer.maxTextureSize != (int)MaxSize)
            {
                importer.maxTextureSize = (int)MaxSize;
            }
            if(compressionOverride != OverrideType.Ignore)
            {
                importer.textureCompression = Compression;
            }

            //Sprite
            if (importer.textureType == TextureImporterType.Sprite)
            {
                if(spriteImportModeOverride != OverrideType.Ignore)
                {
                    importer.spriteImportMode = SpriteImportMode;
                }
                if(spritePivotOverride != OverrideType.Ignore)
                {
                    importer.spritePivot = SpritePivot;
                }
            }
        }
    }
}