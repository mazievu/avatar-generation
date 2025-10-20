
using UnityEngine;
using System.Collections.Generic;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewCareerTrack", menuName = "LifeSim/Career Track")]
    public class CareerSO : ScriptableObject
    {
        [Header("Metadata")]
        public string trackId;
        public string trackNameKey; // Localization key

        [Header("Requirements")]
        public int requiredIq;
        public int requiredEq;
        public string requiredMajorId; // e.g., "tech", "medicine"

        [Header("Hiring Companies")]
        [Tooltip("A list of prestige tiers (e.g., 1, 2, 3) of companies that can offer this career.")]
        public List<int> companyTiers;

        [Header("Career Levels")]
        public List<CareerLevel> levels;

        [System.Serializable]
        public class CareerLevel
        {
            public string titleKey; // Localization key
            public int salaryPerMonth;
            public int requiredSkill;
        }
    }
}
