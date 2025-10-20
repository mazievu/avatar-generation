
using UnityEngine;
using LifeSim.Core.Domain.Characters;

namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewEducationOption", menuName = "LifeSim/Education Option")]
    public class EducationSO : ScriptableObject
    {
        public enum EducationType { School, UniversityMajor }

        [Header("General Info")]
        public EducationType type;
        public string NameKey; // Localization key for the name of the school or major
        public int cost;
        public int educationTier; // e.g., 1=Normal, 2=Private, 3=Royal
        public LifePhase requiredLifePhase; // Life phase required to attend

        [Header("Career Path Suggestion")]
        [Tooltip("The ID of the career this major is recommended for. Used to show hints in the UI.")]
        public string preferredCareerId;

        [Header("Stat Changes on Completion")]
        public int iqChange;
        public int eqChange;
        public int skillChange;

        [Header("Requirements")]
        public int minIq;
        public int minEq;
    }
}
