using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.SpeedTreeImporter;

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
        public bool ConvertUnits => convertUnitsOverride == OverrideType.Override && Parent != null ? Parent.ConvertUnits : convertUnits;
        //-
        [SerializeField] private OverrideType bakeAxisConversionOverride;
        [SerializeField] private bool bakeAxisConversion = false;
        public bool BakeAxisConversion => bakeAxisConversionOverride == OverrideType.Override && Parent != null ? Parent.BakeAxisConversion : bakeAxisConversion;
        //-
        [SerializeField] private OverrideType importBlendShapesOverride;
        [SerializeField] private bool importBlendShapes = false;
        public bool ImportBlendShapes => importBlendShapesOverride == OverrideType.Override && Parent != null ? Parent.ImportBlendShapes : importBlendShapes;
        //-
        [SerializeField] private OverrideType importDeformPercentOverride;
        [SerializeField] private bool importDeformPercent = false;
        public bool ImportDeformPercent => importDeformPercentOverride == OverrideType.Override && Parent != null ? Parent.ImportDeformPercent : importDeformPercent;
        //-
        [SerializeField] private OverrideType importVisibilityOverride;
        [SerializeField] private bool importVisibility = false;
        public bool ImportVisibility => importVisibilityOverride == OverrideType.Ignore && Parent != null ? Parent.ImportVisibility : importVisibility;
        //-
        [SerializeField] private OverrideType importCameraOverride;
        [SerializeField] private bool importCamera = false;
        public bool ImportCamera => importCameraOverride == OverrideType.Ignore && Parent != null ? Parent.ImportCamera : importCamera;
        //-
        [SerializeField] private OverrideType importLightsOverride;
        [SerializeField] private bool importLights = false;
        public bool ImportLights => importLightsOverride == OverrideType.Ignore && Parent != null ? Parent.ImportLights : importLights;
        //-
        [SerializeField] private OverrideType preserveHierarchyOverride;
        [SerializeField] private bool preserveHierarchy = false;
        public bool PreserveHierarchy => preserveHierarchyOverride == OverrideType.Ignore && Parent != null ? Parent.PreserveHierarchy : preserveHierarchy;

        //Model-Meshes -----------
        [SerializeField] private OverrideType meshCompressionOverride;
        [SerializeField] private ModelImporterMeshCompression meshCompression = ModelImporterMeshCompression.High;
        public ModelImporterMeshCompression MeshCompression => meshCompressionOverride == OverrideType.Ignore && Parent != null ? Parent.MeshCompression : meshCompression;
        //-
        [SerializeField] private OverrideType readWriteOverride;
        [SerializeField] private bool readWrite = false;
        public bool ReadWrite => readWriteOverride == OverrideType.Ignore && Parent != null ? Parent.ReadWrite : readWrite;
        //-
        [SerializeField] private OverrideType optimizeMeshOverride;
        [SerializeField] private MeshOptimizationFlags optimizeMesh = MeshOptimizationFlags.Everything;
        public MeshOptimizationFlags OptimizeMesh => optimizeMeshOverride == OverrideType.Ignore && Parent != null ? Parent.OptimizeMesh : optimizeMesh;

        //Model-Geometry -----------
        [SerializeField] private OverrideType indexFormatOverride;
        [SerializeField] private ModelImporterIndexFormat indexFormat = ModelImporterIndexFormat.Auto;
        public ModelImporterIndexFormat IndexFormat => indexFormatOverride == OverrideType.Ignore && Parent != null ? Parent.IndexFormat : indexFormat;
        //-
        [SerializeField] private OverrideType generateLightmapUvsOverride;
        [SerializeField] private bool generateLightmapUvs = false;
        public bool GenerateLightmapUvs => generateLightmapUvsOverride == OverrideType.Ignore && Parent != null ? Parent.GenerateLightmapUvs : generateLightmapUvs;

        //Rig -----------
        [SerializeField] private OverrideType animationTypeOverride;
        [SerializeField] private ModelImporterAnimationType animationType;
        public ModelImporterAnimationType AnimationType => animationTypeOverride == OverrideType.Ignore && Parent != null ? Parent.AnimationType : animationType;
        //-
        [SerializeField] private OverrideType generateAnimationsOverride;
        [SerializeField] private ModelImporterGenerateAnimations generateAnimations;
        public ModelImporterGenerateAnimations GenerateAnimations => generateAnimationsOverride == OverrideType.Ignore && Parent != null ? Parent.GenerateAnimations : generateAnimations;
        //-
        [SerializeField] private OverrideType stripBonesOverride;
        [SerializeField] private bool stripBones;
        public bool StripBones => stripBonesOverride == OverrideType.Ignore && Parent != null ? Parent.StripBones : stripBones;

        //Animation -----------
        [SerializeField] private OverrideType importAnimationOverride;
        [SerializeField] private bool importAnimation;
        public bool ImportAnimation => importAnimationOverride == OverrideType.Ignore && Parent != null ? Parent.ImportAnimation : importAnimation;
        //-
        [SerializeField] private OverrideType importConstraintsOverride;
        [SerializeField] private bool importConstraints;
        public bool ImportConstraints => importConstraintsOverride == OverrideType.Ignore && Parent != null ? Parent.ImportConstraints : importConstraints;

        //Materials -----------
        [SerializeField] private OverrideType materialImportModeOverride;
        [SerializeField] private ModelImporterMaterialImportMode materialImportMode;
        public ModelImporterMaterialImportMode MaterialImportMode => materialImportModeOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialImportMode : materialImportMode;
        //
        [SerializeField] private OverrideType useSRGBMaterialColorOverride;
        [SerializeField] private bool useSRGBMaterialColor;
        public bool UseSRGBMaterialColor => useSRGBMaterialColorOverride == OverrideType.Ignore && Parent != null ? Parent.UseSRGBMaterialColor : useSRGBMaterialColor;
        //-
        [SerializeField] private OverrideType materialLocationOverride;
        [SerializeField] private ModelImporterMaterialLocation materialLocation;
        public ModelImporterMaterialLocation MaterialLocation => materialLocationOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialLocation : materialLocation;
        //-
        [SerializeField] private OverrideType materialNameOverride;
        [SerializeField] private ModelImporterMaterialName materialName;
        public ModelImporterMaterialName MaterialName => materialNameOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialName : materialName;
        //-
        [SerializeField] private OverrideType materialSearchOverride;
        [SerializeField] private ModelImporterMaterialSearch materialSearch;
        public ModelImporterMaterialSearch MaterialSearch => materialSearchOverride == OverrideType.Ignore && Parent != null ? Parent.MaterialSearch : materialSearch;

        public override void Apply(AssetImporter im)
        {
            var importer = im as ModelImporter;

            //Model--Scene
            if (convertUnitsOverride != OverrideType.Ignore)
            {
                importer.useFileUnits = ConvertUnits;
            }
            if (bakeAxisConversionOverride != OverrideType.Ignore)
            {
                importer.bakeAxisConversion = BakeAxisConversion;
            }
            if (importBlendShapesOverride != OverrideType.Ignore)
            {
                importer.importBlendShapes = ImportBlendShapes;
            }
            if (importDeformPercentOverride != OverrideType.Ignore)
            {
                importer.importBlendShapeDeformPercent = ImportDeformPercent;
            }
            if (importVisibilityOverride != OverrideType.Ignore)
            {
                importer.importVisibility = ImportVisibility;
            }
            if(importCameraOverride!= OverrideType.Ignore)
            {
                importer.importCameras = ImportCamera;
            }
            if(importLightsOverride != OverrideType.Ignore)
            {
                importer.importLights = ImportLights;
            }
            if(preserveHierarchyOverride != OverrideType.Ignore)
            {
                importer.preserveHierarchy = PreserveHierarchy;
            }

            //Model--Meshes
            if(meshCompressionOverride != OverrideType.Ignore)
            {
                importer.meshCompression = MeshCompression;
            }
            if(readWriteOverride != OverrideType.Ignore)
            {
                importer.isReadable = ReadWrite;
            }
            if(optimizeMeshOverride != OverrideType.Ignore)
            {
                importer.meshOptimizationFlags = OptimizeMesh;
            }

            //Model--Geometry
            if(indexFormatOverride != OverrideType.Ignore)
            {
                importer.indexFormat = IndexFormat;
            }
            if(generateLightmapUvsOverride != OverrideType.Ignore)
            {
                importer.generateSecondaryUV = GenerateLightmapUvs;
            }

            //Rig
            if(animationTypeOverride != OverrideType.Ignore)
            {
                importer.animationType = AnimationType;
            }
            if(generateAnimationsOverride != OverrideType.Ignore)
            {
                importer.generateAnimations = GenerateAnimations;
            }
            if(stripBonesOverride != OverrideType.Ignore)
            {
                importer.optimizeBones = StripBones;
            }

            //Animation
            if(importAnimationOverride != OverrideType.Ignore)
            {
                importer.importAnimation = ImportAnimation;
            }
            if(importConstraintsOverride != OverrideType.Ignore)
            {
                importer.importConstraints = ImportConstraints;
            }

            //Materials
            if(materialImportModeOverride != OverrideType.Ignore)
            {
                importer.materialImportMode = MaterialImportMode;
            }
            if(useSRGBMaterialColorOverride != OverrideType.Ignore)
            {
                importer.useSRGBMaterialColor = UseSRGBMaterialColor;
            }
            if(materialLocationOverride != OverrideType.Ignore)
            {
                importer.materialLocation = MaterialLocation;
            }
            if(materialNameOverride != OverrideType.Ignore)
            {
                importer.materialName = MaterialName;
            }
            if(materialSearchOverride != OverrideType.Ignore)
            {
                importer.materialSearch = MaterialSearch;
            }
        }
    }
}