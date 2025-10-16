using System.Threading.Tasks;


namespace LifeSim.Core.Services
{
public interface ISaveStore
{
Task SaveAsync<T>(string key, T data);
Task<T> LoadAsync<T>(string key, T fallback = default);
}


public interface ILocalization
{
void SetLanguage(string lang);
string T(string key);
}


public interface IRandom
{
int NextInt(int minInclusive, int maxInclusive);
float NextFloat();
}


public interface IClock
{
// returns delta days per tick if you want time-scale based systems
int DaysPerTick { get; }
}
}