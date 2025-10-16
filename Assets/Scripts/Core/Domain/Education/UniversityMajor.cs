using System;

namespace LifeSim.Core.Domain.Education
{
    [Serializable]
    public class UniversityMajor
    {
        public string key;
        public string nameKey;
        public string descriptionKey;
        public int cost;
        public int iqBonus;
        public int skillBonus;
    }
}