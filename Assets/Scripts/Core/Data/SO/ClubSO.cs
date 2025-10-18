
using UnityEngine;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewClub", menuName = "LifeSim/Club")]
    public class ClubSO : ScriptableObject
    {
        public string clubId;
        public string clubNameKey; // Localization key
        public string descriptionKey; // Localization key

        [Header("Requirements")]
        public string requiredLifePhase; // e.g., "middle_school", "high_school"
        public int minIq;
        public int minEq;
        public int minHealth;

        [Header("Yearly Stat Boosts")]
        public int iqBoost;
        public int eqBoost;
        public int healthBoost;
        public int happinessBoost;
    }
}
