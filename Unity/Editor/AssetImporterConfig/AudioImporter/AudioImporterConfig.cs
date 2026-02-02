using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;
using static UnityEditor.SpeedTreeImporter;

namespace GameCore.Unity
{
    public class AudioImporterConfig : AssetImporterConfigBase
    {
        //ITreeNode
        public override ITreeNode ParentNode => parent;
        //-------------------------------------------

        [SerializeField] private AudioImporterConfig parent;
        public AudioImporterConfig Parent => parent;

        [SerializeField] private OverrideType forceToMonoOverride;
        [SerializeField] private bool forceToMono = true;
        public bool ForceToMono => forceToMonoOverride == OverrideType.Override && Parent != null ? Parent.ForceToMono : forceToMono;
        //--
        //[SerializeField] private OverrideType normalizeOverride;
        //--
        [SerializeField] private OverrideType loadInBackgroundOverride;
        [SerializeField] private bool loadInBackground = true;
        public bool LoadInBackground => loadInBackgroundOverride == OverrideType.Override && Parent != null ? Parent.LoadInBackground : loadInBackground;
        //-
        [SerializeField] private OverrideType ambisonicOverride;
        [SerializeField] private bool ambisonic = true;
        public bool Ambisonic => ambisonicOverride == OverrideType.Override && Parent != null ? Parent.ambisonic : Ambisonic;

        //[SerializeField] private OverrideType loadTypeOverride;
        ////--
        //[SerializeField] private OverrideType preloadOverride;
        ////--
        //[SerializeField] private OverrideType compressionFormationOverride;
        ////--
        //[SerializeField] private OverrideType qualityOverride;
        ////--
        //[SerializeField] private OverrideType sampleRateOverride;


        public override void Apply(AssetImporter im)
        {
            var importer = im as AudioImporter;
            if (forceToMonoOverride == OverrideType.Override)
            {
                importer.forceToMono = ForceToMono;
            }
            if (loadInBackgroundOverride == OverrideType.Override)
            {
                importer.loadInBackground = LoadInBackground;
            }
            if(ambisonicOverride == OverrideType.Override)
            {
                importer.ambisonic = Ambisonic;
            }
        }
    }
}