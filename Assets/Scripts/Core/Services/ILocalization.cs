namespace LifeSim.Core.Services
{
    public interface ILocalization
    {
        void SetLanguage(string lang);
        string T(string key);
    }
}