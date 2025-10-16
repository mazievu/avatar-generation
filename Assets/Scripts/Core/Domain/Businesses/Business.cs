using System;
using System.Collections.Generic;

namespace LifeSim.Core.Domain.Businesses
{
    [Serializable]
    public class Business
    {
        public string id;
        public string nameKey;
        public int level = 1;
        public int baseIncome = 0;
        public int upkeep = 0;
        public List<string> employeeSlots = new List<string>();
    }
}