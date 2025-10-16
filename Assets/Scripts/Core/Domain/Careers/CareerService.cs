using LifeSim.Core.Domain.Characters;

namespace LifeSim.Core.Domain.Careers
{
    public static class CareerService
    {
        public static bool IsQualifiedForTrack(Character c, CareerTrack track)
        {
            if (track.ladder.Count == 0) return true;
            var first = track.ladder[0];
            return c.stats.iq >= first.requiredIQ && c.stats.eq >= first.requiredEQ;
        }

        public static int ComputeStepIndex(Character c, CareerTrack track)
        {
            int idx = 0;
            for (int i = 0; i < track.ladder.Count; i++)
            {
                var step = track.ladder[i];
                if (c.stats.iq >= step.requiredIQ && c.stats.eq >= step.requiredEQ) idx = i;
                else break;
            }
            return idx;
        }

        public static int CurrentSalary(Character c, CareerTrack track)
        {
            if (track.ladder.Count == 0) return 0;
            var step = track.ladder[ComputeStepIndex(c, track)];
            return step.baseSalary;
        }
    }
}