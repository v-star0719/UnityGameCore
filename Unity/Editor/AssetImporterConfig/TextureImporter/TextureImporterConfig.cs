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
        public OverrideType TextureTypeOverride => textureTypeOverride  == OverrideType.Inherit && Parent != null ? Parent.textureTypeOverride : textureTypeOverride;
        public TextureImporterType TextureType => textureTypeOverride == OverrideType.Inherit && Parent != null ? Parent.TextureType : textureType;
        //--
        [SerializeField] private OverrideType alphaIsTransparencyOverride;
        [SerializeField] private bool alphaIsTransparency;
        public OverrideType AlphaIsTransparencyOverride => alphaIsTransparencyOverride == OverrideType.Inherit && Parent != null ? Parent.AlphaIsTransparencyOverride : alphaIsTransparencyOverride;
        public bool AlphaIsTransparency => alphaIsTransparencyOverride == OverrideType.Inherit && Parent != null ? Parent.AlphaIsTransparency : alphaIsTransparency;
        //--
        [SerializeField] private OverrideType sRgbColorTextureOverride;
        [SerializeField] private bool sRgbColorTexture;
        public OverrideType SRgbColorTextureOverride => sRgbColorTextureOverride == OverrideType.Inherit && Parent != null ? Parent.SRgbColorTextureOverride : sRgbColorTextureOverride;
        public bool SRgbColorTexture => sRgbColorTextureOverride == OverrideType.Inherit && Parent != null ? Parent.SRgbColorTexture : sRgbColorTexture;
        //--
        [SerializeField] private OverrideType powerOf2Override;
        [SerializeField] private TextureImporterNPOTScale powerOf2 = TextureImporterNPOTScale.ToNearest;
        public OverrideType PowerOf2Override => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.PowerOf2Override : powerOf2Override;
        public TextureImporterNPOTScale PowerOf2 => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.PowerOf2 : powerOf2;
        //--
        [SerializeField] private OverrideType isReadableOverride;
        [SerializeField] private bool isReadable;
        public OverrideType IsReadableOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.IsReadableOverride : isReadableOverride;
        public bool IsReadable => isReadableOverride == OverrideType.Inherit && Parent != null ? Parent.IsReadable : isReadable;
        //--
        [SerializeField] private OverrideType miniMapOverride;
        [SerializeField] private bool miniMap;
        public OverrideType MiniMapOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.MiniMapOverride : miniMapOverride;
        public bool MiniMap => miniMapOverride == OverrideType.Inherit && Parent != null ? Parent.MiniMap : miniMap;
        //--
        [SerializeField] private OverrideType wrapModeOverride;
        [SerializeField] private TextureWrapMode wrapMode;
        public OverrideType WrapModeOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.WrapModeOverride : wrapModeOverride;
        public TextureWrapMode WrapMode => wrapModeOverride == OverrideType.Inherit && Parent != null ? Parent.WrapMode : wrapMode;
        //--
        [SerializeField] private OverrideType filterModeOverride;
        [SerializeField] private FilterMode filterMode;
        public OverrideType FilterModeOverride => filterModeOverride == OverrideType.Inherit && Parent != null ? Parent.FilterModeOverride : filterModeOverride;
        public FilterMode FilterMode => filterModeOverride == OverrideType.Inherit && Parent != null ? Parent.FilterMode: filterMode;

        //通用贴图格式相关
        [SerializeField] private OverrideType maxSizeOverride;
        [SerializeField] private TextureMaxSizeType maxSize = TextureMaxSizeType._2048;
        public OverrideType MaxSizeOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.MaxSizeOverride : maxSizeOverride;
        public TextureMaxSizeType MaxSize => maxSizeOverride == OverrideType.Inherit && Parent != null ? Parent.MaxSize : maxSize;
        //--
        [SerializeField] private OverrideType compressionOverride;
        [SerializeField] private TextureImporterCompression compression = TextureImporterCompression.Compressed;
        public OverrideType CompressionOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.CompressionOverride : compressionOverride;
        public TextureImporterCompression Compression => compressionOverride == OverrideType.Inherit && Parent != null ? Parent.Compression : compression;

        //Sprite
        [SerializeField] private OverrideType spriteImportModeOverride;
        [SerializeField] private SpriteImportMode spriteImportMode;
        public OverrideType SpriteImportModeOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.SpriteImportModeOverride : spriteImportModeOverride;
        public SpriteImportMode SpriteImportMode => spriteImportModeOverride == OverrideType.Inherit && Parent != null ? Parent.SpriteImportMode : spriteImportMode;
        //--
        [SerializeField] private OverrideType spriteMeshTypeOverride;
        [SerializeField] private SpriteMeshType spriteMeshType = SpriteMeshType.Tight;
        public OverrideType SpriteMeshTypeOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.SpriteMeshTypeOverride : spriteMeshTypeOverride;
        public SpriteMeshType SpriteMeshType => spriteMeshTypeOverride == OverrideType.Inherit && Parent != null ? Parent.SpriteMeshType : spriteMeshType;
        //--
        [SerializeField] private OverrideType spritePivotOverride;
        [SerializeField] private Vector2 spritePivot = new Vector2(0.5f, 0.5f);
        public OverrideType SpritePivotOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.SpritePivotOverride : spritePivotOverride;
        public Vector2 SpritePivot => spritePivotOverride == OverrideType.Inherit && Parent != null ? Parent.SpritePivot : spritePivot;

        [SerializeField] private OverrideType defaultPlatformOverride;
        [SerializeField] private TextureImporterPlatformSettings defaultPlatform;
        public OverrideType DefaultPlatformOverride => powerOf2Override == OverrideType.Inherit && Parent != null ? Parent.DefaultPlatformOverride : defaultPlatformOverride;
        public TextureImporterPlatformSettings DefaultPlatform => defaultPlatformOverride == OverrideType.Inherit && Parent != null ? Parent.DefaultPlatform : defaultPlatform;

        //[SerializeField] private OverrideType standaloneSettingsOverride;
        //[SerializeField] private TextureImporterPlatformSettings standaloneSettings;
        //public TextureImporterPlatformSettings StandaloneSettings => standaloneSettingsOverride == OverrideType.Override && Parent != null ? Parent.StandaloneSettings : standaloneSettings;

        public override void Apply(AssetImporter im)
        {
            var importer = im as TextureImporter;
            //通用基础
            if(TextureTypeOverride != OverrideType.Ignore)
            {
                importer.textureType = TextureType;
            }
            if(SRgbColorTextureOverride != OverrideType.Ignore)
            {
                importer.sRGBTexture = SRgbColorTexture;
            }
            if(AlphaIsTransparencyOverride != OverrideType.Ignore)
            {
                importer.alphaIsTransparency = AlphaIsTransparency;
            }
            if(PowerOf2Override != OverrideType.Ignore)
            {
                importer.npotScale = PowerOf2;
            }
            if(IsReadableOverride != OverrideType.Ignore)
            {
                importer.isReadable = IsReadable;
            }
            if(MiniMapOverride != OverrideType.Ignore)
            {
                importer.mipmapEnabled = MiniMap;
            }
            if(WrapModeOverride != OverrideType.Ignore)
            {
                importer.wrapMode = WrapMode;
            }
            if(FilterModeOverride != OverrideType.Ignore)
            {
                importer.filterMode = FilterMode;
            }

            //通用贴图格式相关
            if(MaxSizeOverride != OverrideType.Customize && importer.maxTextureSize != (int)MaxSize)
            {
                importer.maxTextureSize = (int)MaxSize;
            }
            if(CompressionOverride != OverrideType.Ignore)
            {
                importer.textureCompression = Compression;
            }

            //Sprite
            if (importer.textureType == TextureImporterType.Sprite)
            {
                if(SpriteImportModeOverride != OverrideType.Ignore)
                {
                    importer.spriteImportMode = SpriteImportMode;
                }
                if(SpritePivotOverride != OverrideType.Ignore)
                {
                    importer.spritePivot = SpritePivot;
                }
            }
        }
    }
}