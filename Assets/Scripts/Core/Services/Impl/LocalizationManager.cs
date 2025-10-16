using System.Collections.Generic;

namespace LifeSim.Core.Services
{
    public class LocalizationManager : ILocalization
    {
        private Dictionary<string, string> _table = new Dictionary<string, string>();
        private string _lang = "en";

        public void SetLanguage(string lang)
        {
            _lang = lang;
            // TODO: optional, use ResourcesLocalization instead if you want JSON-based merge
        }

        public string T(string key) => _table.TryGetValue(key, out var v) ? v : key;
    }
}