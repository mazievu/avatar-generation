using System.Collections.Generic;
using LifeSim.Core.Domain.Game;
using LifeSim.Core.Domain.Characters;

namespace LifeSim.Core.Domain.Businesses
{
    public static class BusinessService
    {
        public static int ComputeNetIncome(GameState s, IReadOnlyDictionary<string, Business> owned)
        {
            int total = 0;
            foreach (var b in owned.Values)
            {
                int productivityBonus = 0;
                foreach (var empId in b.employeeSlots)
                {
                    if (empId != null && s.familyMembers.TryGetValue(empId, out var c) && c.isAlive)
                    {
                        productivityBonus += (b.baseIncome * c.stats.skill) / 10000;
                    }
                }
                total += (b.baseIncome + productivityBonus) - b.upkeep;
            }
            return total;
        }

        public static void Upgrade(Business b)
        {
            b.level++;
            b.baseIncome = (int)(b.baseIncome * 1.25f);
            b.upkeep     = (int)(b.upkeep     * 1.10f);
            if (b.level % 3 == 0) b.employeeSlots.Add(null);
        }
    }
}