
using UnityEngine;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewAsset", menuName = "LifeSim/Asset")]
    public class AssetSO : ScriptableObject
    {
        public string assetId;
        public string assetNameKey; // Localization key
        public string descriptionKey; // Localization key
        public string assetType; // e.g., "real_estate", "vehicle", "electronics"

        public int price;

        [Header("Permanent Buffs (Applied to player character)")]
        public float happinessModifier; // e.g., 1.05 for a 5% boost
        public float healthModifier;
        public float iqModifier;
        public float eqModifier;
    }
}
