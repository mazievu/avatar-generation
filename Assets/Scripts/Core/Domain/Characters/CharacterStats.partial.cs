namespace LifeSim.Core.Domain.Characters
{
    public partial class CharacterStats
    {
        public void Clamp(int maxIQ = 200)
        {
            happiness = Clamp01(happiness);
            health    = Clamp01(health);
            eq        = Clamp01(eq);
            skill     = Clamp01(skill);
            if (iq < 0) iq = 0;
            if (iq > maxIQ) iq = maxIQ;
        }

        private int Clamp01(int v)
        {
            if (v < 0) return 0;
            if (v > 100) return 100;
            return v;
        }
    }
}