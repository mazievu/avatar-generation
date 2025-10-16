using LifeSim.Core.Services;

namespace LifeSim.Core.Infra
{
    public class FixedClock : IClock
    {
        public int DaysPerTick { get; private set; }
        public FixedClock(int daysPerTick = 1) { DaysPerTick = daysPerTick; }
    }
}