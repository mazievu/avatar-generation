namespace LifeSim.Core.Services
{
    public interface IRandom
    {
        int NextInt(int minInclusive, int maxInclusive);
        float NextFloat();
    }
}