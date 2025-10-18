using LifeSim.Core.Domain.Characters;

namespace LifeSim.Core.Data
{
    public static class Constants
    {
        public const int DaysPerYear = 365;
        public const int StartYear   = 2000;

        public const int MinStat = 0;
        public const int MaxStat = 100;
        public const int MaxIQ   = 200;

        public const int DebtGameOverThreshold = -100_000;

        public const string OkKey = "common.ok";

        // New Economy Constants
        public const int RETIREMENT_PENSION_PER_MONTH = 350; // 4200 / 12

        public static int GetCostOfLiving(LifePhase phase)
        {
            switch (phase)
            {
                case LifePhase.Newborn:
                case LifePhase.ElementarySchool:
                case LifePhase.MiddleSchool:
                case LifePhase.HighSchool:
                    return 100; // Child/Teen expenses
                case LifePhase.University:
                    return 250;
                case LifePhase.WorkingLife:
                    return 400;
                case LifePhase.Retired:
                    return 200;
                default:
                    return 150;
            }
        }
    }
}