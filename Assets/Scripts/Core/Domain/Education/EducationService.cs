using LifeSim.Core.Domain.Characters;

namespace LifeSim.Core.Domain.Education
{
    public static class EducationService
    {
        public static void ApplySchoolEffects(Character c, SchoolOption opt)
        {
            foreach (var kv in opt.effects)
            {
                switch (kv.Key)
                {
                    case "happiness": c.stats.happiness += kv.Value; break;
                    case "health":    c.stats.health    += kv.Value; break;
                    case "iq":        c.stats.iq        += kv.Value; break;
                    case "eq":        c.stats.eq        += kv.Value; break;
                    case "skill":     c.stats.skill     += kv.Value; break;
                }
            }
            c.stats.Clamp();
        }

        public static void ApplyMajor(Character c, UniversityMajor major)
        {
            c.stats.iq    += major.iqBonus;
            c.stats.skill += major.skillBonus;
            c.stats.Clamp();
        }
    }
}