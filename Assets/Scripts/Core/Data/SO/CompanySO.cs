using UnityEngine;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewCompany", menuName = "LifeSim/Company")]
    public class CompanySO : ScriptableObject
    {
        [Header("General Info")]
        public string companyNameKey; // For localization
        [Tooltip("Prestige tier of the company, e.g., 1 to 5.")]
        public int prestigeTier = 1;

        [Header("Requirements")]
        [Tooltip("ID of the EducationSO required. Can be empty.")]
        public string requiredEducationId;
        [Tooltip("Multiplier for the career's base stat requirements. E.g., 1.2 for 20% higher stats.")]
        public float requiredStatMultiplier = 1.0f;

        [Header("Salary")]
        [Tooltip("Multiplier for the career's base salary. E.g., 1.5 for 50% higher pay.")]
        public float salaryMultiplier = 1.0f;
    }
}
