using UnityEditor;
using UnityEngine;

namespace GameCore.Unity
{
    public class ModelImporterConfig : AssetImporterConfigBase
    {
        //ITreeNode
        public override ITreeNode ParentNode => parent;
        //-------------------------------------------

        [SerializeField] private ModelImporterConfig parent;
        public ModelImporterConfig Parent => parent;

        //Model--Scene -----------
        [SerializeField] private OverrideType convertUnitsOverride;
        [SerializeField] private bool convertUnits = true;
        public OverrideType ConvertUnitsOverride => convertUnitsOverride  == OverrideType.Inherit && Parent != null ? Parent.ConvertUnitsOverride : convertUnitsOverride;
        public bool ConvertUnits => convertUnitsOverride == OverrideType.Inherit && Parent != null ? Parent.ConvertUnits : convertUnits;
        //-
        [SerializeField] private OverrideType bakeAxisConversionOverride;
        [SerializeField] private bool bakeAxisConversion = false;
        public OverrideType BakeAxisConversionOverride => bakeAxisConversionOverride  == OverrideType.Inherit && Parent != null ? Parent.BakeAxisConversionOverride : bakeAxisConversionOverride;
        public bool BakeAxisConversion => bakeAxisConversionOverride == OverrideType.Inherit && Parent != null ? Parent.BakeAxisConversion : bakeAxisConversion;
        //-
        [SerializeField] private OverrideType importBlendShapesOverride;
        [SerializeField] private bool importBlendShapes = false;
        public OverrideType ImportBlendShapesOverride => importBlendShapesOverride  == OverrideType.Inherit && Parent != null ? Parent.ImportBlendShapesOverride : importBlendShapesOverride;
        public bool ImportBlendShapes => importBlendShapesOverride == OverrideType.Inherit && Parent != null ? Parent.ImportBlendShapes : importBlendShapes;
        //-
        [SerializeField] private OverrideType importDeformPercentOverride;
        [SerializeField] private bool importDeformPercent = false;
        public OverrideType ImportDeformPercentOverride => importDeformPercentOverride  == OverrideType.Inherit && Parent != null ? Parent.ImportDeformPercentOverride : importDeformPercentOverride;
        public bool ImportDeformPercent => importDeformPercentOverride == OverrideType.Inherit && Parent != null ? Parent.ImportDeformPercent : importDeformPercent;
        //-
        [SerializeField] private OverrideType importVisibilityOverride;
        [SerializeField] private bool importVisibility = false;
        public OverrideType ImportVisibilityOverride => importVisibilityOverride  == OverrideType.Inherit && Parent != null ? Parent.ImportVisibilityOverride : importVisibilityOverride;
        public bool ImportVisibility => importVisibilityOverride == OverrideType.Ignore && Parent != null ? Parent.ImportVisibility : importVisibility;
        //-
        [SerializeField] private OverrideType importCameraOverride;
        [SerializeField] private bool importCamera = false;
        public OverrideType ImportCameraOverride => importCameraOverride  == OverrideType.Inherit && Parent != null ? Parent.ImportCameraOverride : importCameraOverride;
        public bool ImportCamera => importCameraOverride == OverrideType.Ignore && Parent != null ? Parent.ImportCamera : importCamera;
        //-
        [SerializeField] private OverrideType importLightsOverride;
        [SerializeField] private bool importLights = false;
        public OverrideType ImportLightsOverride => importLightsOverride  == OverrideType.Inherit && Parent != null ? Parent.ImportLightsOverride : importLightsOverride;
        public bool ImportLights => importLightsOverride == OverrideType.Ignore && Parent != null ? Parent.ImportLights : importLights;
        //-
        [SerializeField] private OverrideType preserveHierarchyOverride;
        [SerializeField] private bool preserveHierarchy = false;
        public OverrideType PreserveHierarchyOverride => preserveHierarchyOverride  == OverrideType.Inherit && Parent != null ? Parent.PreserveHierarchyOverride : preserveHierarchyOverride;
        public bool PreserveHierarchy => preserveHierarchyOverride == OverrideType.Ignore && Parent != null ? Parent.PreserveHierarchy : preserveHierarchy;

        //Model-Meshes -----------
        [SerializeField] private OverrideType meshCompressionOverride;
        [SerializeField] private ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.High;
        public OverrideType MeshCompressionOverride => meshCompressionOverride  == OverrideType.Inherit && Parent != null ? Parent.MeshCompressionOverride : meshCompressionOverride;
        public ModelImporterMeshCompression MeshCompression => meshCompressionOverride == OverrideType.Ignore && Parent != null ? Parent.MeshCompression : meshCompression;
        //-
        [SerializeField] private OverrideType readWriteOverride;
        [SerializeField] private bool readWrite = false;
        public OverrideType ReadWriteOverride => readWriteOverride  == OverrideType.Inherit && Parent != null ? Parent.ReadWriteOverride : readWriteOverride;
        public bool ReadWrite => readWriteOverride == OverrideType.Ignore && Parent != null ? Parent.ReadWrite : readWrite;
        //-
        [SerializeField] private OverrideType optimizeMeshOverride;
        [SerializeField] private MeshOptimizationFlags optimizeMesh = MeshOptimizationFlags.Everything;
        public OverrideType OptimizeMeshOverride => readWriteOverride  == OverrideType.Inherit && Parent != null ? Parent.OptimizeMeshOverride : readWriteOverride;
        public MeshOptimizationFlags OptimizeMesh => optimizeMeshOverride == OverrideType.Ignore && Parent != null ? Parent.OptimizeMesh : optimizeMesh;

        //Model-Geometry -----------
        [SerializeField] private OverrideType indexFormatOverride;
        [SerializeField] private ModelImporterIndexFormat indexFormat = ModelImporterIndexFormat.Auto;
        public OverrideType IndexFormatOverride => indexFormatOverride  == OverrideType.Inherit && Parent != null ? Parent.IndexFormatOverride : indexFormatOverride;
        public ModelImporterIndexFormat IndexFormat => indexFormatOverride == OverrideType.Ignore && Parent != null ? Parent.IndexFormat : indexFormat;
        //-
        [SerializeField] private OverrideType generateLightmapUvsOverride;
        [SerializeField] private bool generateLightmapUvs = false;
        public OverrideType GenerateLightmapUvsOverride => generateLightmapUvsOverride  == OverrideType.Inherit && Parent != null ? Parent.GenerateLightmapUvsOverride : generateLightmapUvsOverride;
        public bool GenerateLightmapUvs => generateLightmapUvsOverride == OverrideType.Ignore && Parent != null ? Parent.GenerateLightmapUvs : generateLightmapUvs;

        //Rig -----------
        [SerializeField] private OverrideType animationTypeOverride;
        [SerializeField] private ModelImporterAnimationType animationType;
        public OverrideType AnimationTypeOverride => animationTypeOverride  == OverrideType.Inherit && Parent != null ? Parent.AnimationTypeOverride : animationTypeOverride;
        public ModelImporterAnimationType AnimationType => animationTypeOverride == OverrideType.Ignore && Parent != null ? Parent.AnimationType : animationType;
        //-
        [SerializeField] private OverrideType generateAnimationsOverride;
        [SerializeField] private ModelImporterGenerateAnimations generateAnimations;
        public OverrideType GenerateAnimationsOverride => generateAnimationsOverride  == OverrideType.Inherit && Parent != null ? Parent.GenerateAnimationsOverride : generateAnimationsOverride;
        public ModelImporterGenerateAnimations GenerateAnimations => generateAnimationsOverride == OverrideType.Ignore && Parent != null ? Parent.GenerateAnimations : generateAnimations;
        //-
        [SerializeField] private OverrideType stripBonesOverride;
        [SerializeField] private bool stripBones;
        public OverrideType StripBonesOverride => stripBonesOverride  == OverrideType.Inherit && Parent != null ? Parent.StripBonesOverride : stripBonesOverride;
        public bool StripBones => stripBonesOverride == OverrideType.Ignore && Parent != null ? Parent.StripBones : stripBones;

        //Animation -----------
        [SerializeField] private OverrideType importAnimationOverride;
        [SerializeField] private bool importAnimation;
        public OverrideType ImportAnimationOverride => importAnimationOverride  == OverrideType.Inherit && Parent != null ? Parent.ImportAnimationOverride : importAnimationOverride;
        public bool ImportAnimation => importAnimationOverride == OverrideType.Ignore && Parent != null ? Parent.ImportAnimation : importAnimation;
        //-
        [SerializeField] private OverrideType importConstraintsOverride;
        [SerializeField] private bool importConstraints;
        public OverrideType ImportConstraintsOverride => importConstraintsOverride  == OverrideType.Inherit && Parent != null ? Parent.ImportConstraintsOverride : importConstraintsOverride;
        public bool ImportConstraints => importConstraintsOverride == OverrideType.Ignore && Parent != null ? Parent.ImportConstraints : importConstraints;

        //Materials -----------
        [SerializeField] private OverrideType materialImportModeOverride;
        [SerializeField] private ModelImporterMaterialImportMode materialImportMode;
        public OverrideType MaterialImportModeOverride => materialImportModeOverride  == OverrideType.Inherit && Parent != null ? Parent.MaterialImportModeOverride : materialImportModeOverride;
        public ModelImporterMaterialImportMode MaterialImportMode => materialImportModeOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialImportMode : materialImportMode;
        //
        [SerializeField] private OverrideType useSRGBMaterialColorOverride;
        [SerializeField] private bool useSRGBMaterialColor;
        public OverrideType UseSRGBMaterialColorOverride => useSRGBMaterialColorOverride  == OverrideType.Inherit && Parent != null ? Parent.UseSRGBMaterialColorOverride : useSRGBMaterialColorOverride;
        public bool UseSRGBMaterialColor => useSRGBMaterialColorOverride == OverrideType.Ignore && Parent != null ? Parent.UseSRGBMaterialColor : useSRGBMaterialColor;
        //-
        [SerializeField] private OverrideType materialLocationOverride;
        [SerializeField] private ModelImporterMaterialLocation materialLocation;
        public OverrideType MaterialLocationOverride => materialLocationOverride  == OverrideType.Inherit && Parent != null ? Parent.MaterialLocationOverride : materialLocationOverride;
        public ModelImporterMaterialLocation MaterialLocation => materialLocationOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialLocation : materialLocation;
        //-
        [SerializeField] private OverrideType materialNameOverride;
        [SerializeField] private ModelImporterMaterialName materialName;
        public OverrideType MaterialNameOverride => materialNameOverride  == OverrideType.Inherit && Parent != null ? Parent.MaterialNameOverride : materialNameOverride;
        public ModelImporterMaterialName MaterialName => materialNameOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialName : materialName;
        //-
        [SerializeField] private OverrideType materialSearchOverride;
        [SerializeField] private ModelImporterMaterialSearch materialSearch;
        public OverrideType MaterialSearchOverride => materialSearchOverride  == OverrideType.Inherit && Parent != null ? Parent.MaterialSearchOverride : materialSearchOverride;
        public ModelImporterMaterialSearch MaterialSearch => materialSearchOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialSearch : materialSearch;

        public override void Apply(AssetImporter im)
        {
            var importer = im as ModelImporter;

            //Model--Scene
            if (ConvertUnitsOverride != OverrideType.Ignore)
            {
                importer.useFileUnits = ConvertUnits;
            }
            if (BakeAxisConversionOverride != OverrideType.Ignore)
            {
                importer.bakeAxisConversion = BakeAxisConversion;
            }
            if (ImportBlendShapesOverride != OverrideType.Ignore)
            {
                importer.importBlendShapes = ImportBlendShapes;
            }
            if (ImportDeformPercentOverride != OverrideType.Ignore)
            {
                importer.importBlendShapeDeformPercent = ImportDeformPercent;
            }
            if (ImportVisibilityOverride != OverrideType.Ignore)
            {
                importer.importVisibility = ImportVisibility;
            }
            if(ImportCameraOverride!= OverrideType.Ignore)
            {
                importer.importCameras = ImportCamera;
            }
            if(ImportLightsOverride != OverrideType.Ignore)
            {
                importer.importLights = ImportLights;
            }
            if(PreserveHierarchyOverride != OverrideType.Ignore)
            {
                importer.preserveHierarchy = PreserveHierarchy;
            }

            //Model--Meshes
            if(MeshCompressionOverride != OverrideType.Ignore)
            {
                importer.meshCompression = MeshCompression;
            }
            if(ReadWriteOverride != OverrideType.Ignore)
            {
                importer.isReadable = ReadWrite;
            }
            if(OptimizeMeshOverride != OverrideType.Ignore)
            {
                importer.meshOptimizationFlags = OptimizeMesh;
            }

            //Model--Geometry
            if(IndexFormatOverride != OverrideType.Ignore)
            {
                importer.indexFormat = IndexFormat;
            }
            if(GenerateLightmapUvsOverride != OverrideType.Ignore)
            {
                importer.generateSecondaryUV = GenerateLightmapUvs;
            }

            //Rig
            if(AnimationTypeOverride != OverrideType.Ignore)
            {
                importer.animationType = AnimationType;
            }
            if(GenerateAnimationsOverride != OverrideType.Ignore)
            {
                importer.generateAnimations = GenerateAnimations;
            }
            if(StripBonesOverride != OverrideType.Ignore)
            {
                importer.optimizeBones = StripBones;
            }

            //Animation
            if(ImportAnimationOverride != OverrideType.Ignore)
            {
                importer.importAnimation = ImportAnimation;
            }
            if(ImportConstraintsOverride != OverrideType.Ignore)
            {
                importer.importConstraints = ImportConstraints;
            }

            //Materials
            if(MaterialImportModeOverride != OverrideType.Ignore)
            {
                importer.materialImportMode = MaterialImportMode;
            }
            if(UseSRGBMaterialColorOverride != OverrideType.Ignore)
            {
                importer.useSRGBMaterialColor = UseSRGBMaterialColor;
            }
            if(MaterialLocationOverride != OverrideType.Ignore)
            {
                importer.materialLocation = MaterialLocation;
            }
            if(MaterialNameOverride != OverrideType.Ignore)
            {
                importer.materialName = MaterialName;
            }
            if(MaterialSearchOverride != OverrideType.Ignore)
            {
                importer.materialSearch = MaterialSearch;
            }
        }
    }
}