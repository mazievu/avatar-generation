using System;
using System.Collections.Generic;

namespace LifeSim.Core.Domain.Education
{
    [Serializable]
    public class SchoolOption
    {
        public string key;
        public string nameKey;
        public int cost;
        public Dictionary<string, int> effects = new Dictionary<string, int>();
    }
}