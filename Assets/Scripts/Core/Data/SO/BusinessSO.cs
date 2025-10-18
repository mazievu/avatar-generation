// FILE: Assets/Scripts/Core/Data/SO/BusinessSO.cs

using UnityEngine;
using System.Collections.Generic;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewBusiness", menuName = "LifeSim/Business")]
    public class BusinessSO : ScriptableObject
    {
        public string businessId;
        public string businessNameKey; // Localization key
        public string businessType; // e.g., "tech", "finance", "agriculture"

        public List<BusinessTier> tiers;

        [System.Serializable]
        public class BusinessTier
        {
            public int tierLevel; // 1, 2, 3
            public int purchasePrice;
            public int upgradePrice;

            [Header("Monthly Economics")]
            public int baseRevenue;
            public float costOfGoodsSoldPercent; // e.g., 0.3 for 30%
            public int fixedCosts;
            public int managerUpkeepPerEmployee;

            [Header("Employee Info")]
            public int employeeSlots;
            public int employeeSalaryPerSkillPoint;
            public int robotMonthlyCost;
            public const int ROBOT_SKILL = 30;
        }
    }
}