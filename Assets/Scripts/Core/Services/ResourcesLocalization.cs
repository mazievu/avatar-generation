// FILE: Assets/Scripts/Presentation/Services/ResourcesLocalization.cs

using UnityEngine;
using System.Collections.Generic;

namespace LifeSim.Core.Services
{
    public class ResourcesLocalization : ILocalization
    {
        private Dictionary<string, string> _table = new Dictionary<string, string>();
        
        public string CurrentLanguage { get; private set; } = "en";

        [System.Serializable] private class KV { public string key; public string value; }
        [System.Serializable] private class KVFile { public List<KV> items; }

        public void SetLanguage(string lang)
        {
            CurrentLanguage = lang;
            _table.Clear();

            var folders = new []{ "ui", "events", "education", "career", "business", "clubs", "summary", "common" };
            foreach (var name in folders)
            {
                var path = $"Localization/{CurrentLanguage}/{name}";
                var ta = Resources.Load<TextAsset>(path);
                if (ta == null) continue;
                var kv = JsonUtility.FromJson<KVFile>(ta.text);
                if (kv?.items == null) continue;
                foreach (var p in kv.items) _table[p.key] = p.value;
            }
        }
        
        // --- BẮT ĐẦU SỬA LỖI ---

        // Phương thức T() gốc
        public string T(string key) => _table.TryGetValue(key, out var v) ? v : key;

        // THAY THẾ TOÀN BỘ CÁC OVERLOAD CŨ BẰNG HÀM NÀY
        public string T(string key, params object[] args)
        {
            string translation = T(key); // Lấy bản dịch gốc

            // Trường hợp 1: Dùng làm giá trị mặc định
            // Nếu chỉ có 1 tham số và nó là string, thì đó là defaultValue
            if (args.Length == 1 && args[0] is string defaultValue)
            {
                if (translation == key) // Nếu không tìm thấy key
                {
                    return defaultValue; // Trả về giá trị mặc định
                }
            }
            
            // Trường hợp 2: Dùng để thay thế placeholder {0}, {1}, ...
            if (args.Length > 0)
            {
                // Nếu không tìm thấy key, và tham số đầu tiên là string, hãy dùng nó làm chuỗi định dạng
                if (translation == key && args[0] is string formatString)
                {
                     // Bỏ qua tham số đầu tiên, lấy các tham số còn lại để format
                     var formatArgs = new object[args.Length - 1];
                     System.Array.Copy(args, 1, formatArgs, 0, args.Length - 1);
                     return string.Format(formatString, formatArgs);
                }
                
                // Nếu tìm thấy key, dùng chuỗi dịch để format
                if (translation != key)
                {
                    return string.Format(translation, args);
                }
            }

            return translation; // Trả về chuỗi gốc nếu không có tham số
        }
        
        // --- KẾT THÚC SỬA LỖI ---
    }
}