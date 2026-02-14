using UnityEditor;
using UnityEngine;

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
        public OverrideType ForceToMonoOverride => forceToMonoOverride  == OverrideType.Inherit && Parent != null ? Parent.ForceToMonoOverride : forceToMonoOverride;
        public bool ForceToMono => forceToMonoOverride == OverrideType.Inherit && Parent != null ? Parent.ForceToMono : forceToMono;
        //--
        //[SerializeField] private OverrideType normalizeOverride;
        //--
        [SerializeField] private OverrideType loadInBackgroundOverride;
        [SerializeField] private bool loadInBackground = true;
        public OverrideType LoadInBackgroundOverride => loadInBackgroundOverride  == OverrideType.Inherit && Parent != null ? Parent.LoadInBackgroundOverride : loadInBackgroundOverride;
        public bool LoadInBackground => loadInBackgroundOverride == OverrideType.Inherit && Parent != null ? Parent.LoadInBackground : loadInBackground;
        //-
        [SerializeField] private OverrideType ambisonicOverride;
        [SerializeField] private bool ambisonic = true;
        public OverrideType AmbisonicOverride => ambisonicOverride  == OverrideType.Inherit && Parent != null ? Parent.AmbisonicOverride : ambisonicOverride;
        public bool Ambisonic => ambisonicOverride == OverrideType.Inherit && Parent != null ? Parent.ambisonic : Ambisonic;

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
            if (ForceToMonoOverride == OverrideType.Inherit)
            {
                importer.forceToMono = ForceToMono;
            }
            if (LoadInBackgroundOverride == OverrideType.Inherit)
            {
                importer.loadInBackground = LoadInBackground;
            }
            if(AmbisonicOverride == OverrideType.Inherit)
            {
                importer.ambisonic = Ambisonic;
            }
        }
    }
}