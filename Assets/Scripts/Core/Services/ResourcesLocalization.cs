using UnityEngine;
using System.Collections.Generic;

namespace LifeSim.Core.Services
{
    public class ResourcesLocalization : ILocalization
    {
        private Dictionary<string, string> _table = new Dictionary<string, string>();
        private string _lang = "en";

        [System.Serializable] private class KV { public string key; public string value; }
        [System.Serializable] private class KVFile { public List<KV> items; }

        public void SetLanguage(string lang)
        {
            _lang = lang;
            _table.Clear();

            var folders = new []{ "ui", "events", "education", "career", "business", "clubs", "summary", "common" };
            foreach (var name in folders)
            {
                var path = $"Localization/{_lang}/{name}";
                var ta = Resources.Load<TextAsset>(path);
                if (ta == null) continue;
                var kv = JsonUtility.FromJson<KVFile>(ta.text);
                if (kv?.items == null) continue;
                foreach (var p in kv.items) _table[p.key] = p.value;
            }
        }

        public string T(string key) => _table.TryGetValue(key, out var v) ? v : key;
    }
}