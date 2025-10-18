
using UnityEngine;
using LifeSim.Core.Domain.Characters;
namespace LifeSim.Core.Data.SO
{
    [CreateAssetMenu(fileName = "NewEducationOption", menuName = "LifeSim/Education Option")]
    public class EducationSO : ScriptableObject
    {public enum EducationType { School, UniversityMajor }
    public EducationType type;
    public LifePhase lifePhase; // Dùng để lọc cho SchoolChoiceModal
    public string NameKey;
    public int Cost;
        public string educationId;
        public string optionNameKey; // Localization key
        public EducationLevel level; // e.g., Elementary, High School, University Major

        public int cost;

        [Header("Stat Changes on Completion")]
        public int iqChange;
        public int eqChange;
        public int skillChange;

        [Header("Requirements")]
        public int minIq;
        public int minEq;

        public enum EducationLevel
        {
            Elementary,
            MiddleSchool,
            HighSchool,
            UniversityMajor,
            VocationalTraining
        }
    }
   }
