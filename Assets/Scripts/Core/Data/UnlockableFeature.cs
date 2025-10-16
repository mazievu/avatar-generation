using System;

namespace LifeSim.Core.Data
{
    public enum FeatureType
    {
        MysteryBox,
        SpecificFeature,
        QoL,
    }

    [Serializable]
    public class UnlockableFeature
    {
        public string id;
        public FeatureType type;
        public string nameKey;
        public string descriptionKey;
        public string iconKey;
    }
}